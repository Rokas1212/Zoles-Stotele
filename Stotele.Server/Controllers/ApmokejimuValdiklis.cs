// ApmokejimuValdiklis - Payment controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApmokejimuController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StripeSettings _stripeSettings;

        private readonly EmailService _emailService;
        private readonly string _webhookSecret;



        public ApmokejimuController(ApplicationDbContext dbContext, IOptions<StripeSettings> stripeSettings, EmailService emailService)
        {
            _dbContext = dbContext;
            _stripeSettings = stripeSettings.Value;
            _webhookSecret = _stripeSettings.WebhookSecret ?? throw new InvalidOperationException("Webhook secret nenustatytas.");
            _emailService = emailService;
        }


        [HttpGet("is-paid")]
        public ActionResult GetIsPaid([FromQuery] int orderId)
        {
            var payment = _dbContext.Apmokejimai.FirstOrDefault(p => p.UzsakymasId == orderId);

            if (payment == null)
            {
                return NotFound($"Apmokėjimas su užsakymo ID: {orderId} nerastas.");
            }

            return Ok(payment.MokejimoStatusas == MokejimoStatusas.Apmoketa);
        }

        private bool IsPaid(int orderId)
        {
            var payment = _dbContext.Apmokejimai.FirstOrDefault(p => p.UzsakymasId == orderId);

            if (payment == null)
            {
                return false;
            }

            return payment.MokejimoStatusas == MokejimoStatusas.Apmoketa;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                // Verify the webhook signature
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _webhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session?.ClientReferenceId == null)
                    {
                        Console.WriteLine("ClientReferenceId is null.");
                        return BadRequest("ClientReferenceId is required.");
                    }

                    var orderId = int.Parse(session.ClientReferenceId);

                    var payment = _dbContext.Apmokejimai.FirstOrDefault(p => p.UzsakymasId == orderId);

                    var order = _dbContext.Uzsakymai
                        .Include(o => o.PrekesUzsakymai)
                        .ThenInclude(pu => pu.Preke)
                        .FirstOrDefault(o => o.Id == orderId);

                    if (payment != null)
                    {
                        // Update item quantities
                        foreach (var item in order.PrekesUzsakymai)
                        {
                            var product = _dbContext.Prekes.Find(item.PrekeId);
                            if (product != null)
                            {
                                product.Kiekis -= item.Kiekis;
                            }
                            _dbContext.SaveChanges();
                        }
                        // Add 5 percent of the order's total to the user's account as loyalty points
                        var pointsToAdd = (int)(order.Suma * 0.05);

                        // Add points to the user's account
                        var user = _dbContext.Klientai.FirstOrDefault(k => k.NaudotojasId == order.NaudotojasId);
                        if (user != null)
                        {
                            var taskai = new Taskai
                            {
                                Kiekis = pointsToAdd,
                                KlientasId = user.Id,
                                PabaigosData = DateTime.UtcNow.AddMonths(1)
                            };
                            _dbContext.Taskai.Add(taskai);
                        }

                        var htmlMessage = $@"
                        <html>
                            <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                <!-- Logo -->
                                <div style='text-align: center; margin-bottom: 20px;'>
                                    <img src='https://cdn3.iconfinder.com/data/icons/nature-emoji/50/Marijuana-512.png' alt='Stotele Logo' style='max-width: 150px;'>
                                </div>

                                <!-- Header -->
                                <h2 style='color: #28a745; text-align: center;'>Užsakymo Patvirtinimas</h2>
                                <p style='text-align: center;'>Užsakymas su ID: <strong>{orderId}</strong> sėkmingai apmokėtas.</p>
                                <p style='text-align: center;'>Kurjeris susisieks telefonu. Ačiū, kad perkate pas mus!</p>

                                <!-- Product Table -->
                                <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                                    <thead>
                                        <tr>
                                            <th style='border-bottom: 2px solid #ccc; text-align: left; padding: 8px;'>Prekė</th>
                                            <th style='border-bottom: 2px solid #ccc; text-align: right; padding: 8px;'>Kaina (€)</th>
                                            <th style='border-bottom: 2px solid #ccc; text-align: center; padding: 8px;'>Kiekis</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {string.Join("", payment.Uzsakymas.PrekesUzsakymai.Select(pu => $@"
                                            <tr>
                                                <td style='border-bottom: 1px solid #eee; padding: 8px;'>{pu.Preke.Pavadinimas}</td>
                                                <td style='border-bottom: 1px solid #eee; padding: 8px; text-align: right;'>
                                                    {(pu.Kaina.HasValue ? pu.Kaina.Value.ToString("0.00") : "0.00")}
                                                </td>
                                                <td style='border-bottom: 1px solid #eee; padding: 8px; text-align: center;'>{pu.Kiekis}</td>
                                            </tr>
                                        "))}
                                    </tbody>
                                </table>
                                <div style='margin-top: 20px; text-align: right;'>
                                    <p><strong>Bendra Suma:</strong> €{order.Suma.ToString("0.00")}</p>
                                    <p><strong>Taškai už šį užsakymą:</strong> {pointsToAdd}</p>
                                    <p><strong>Sąskaitos Faktūros Numeris:</strong> {payment.SaskaitosFakturosNumeris}</p>
                                </div>
                                <div style='text-align: center; margin-top: 20px;'>
                                    <a href='https://localhost:5173/uzsakymas/{orderId}' 
                                    style='background-color: #28a745; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-weight: bold;'>
                                        Peržiūrėti Užsakymą
                                    </a>
                                </div>
                                <footer style='margin-top: 30px; text-align: center; font-size: 12px; color: #aaa;'>
                                    <p>&copy; {DateTime.Now.Year} Žolės Stotelė. Visos teisės saugomos.</p>
                                    <p>Šis laiškas buvo sugeneruotas automatiškai, prašome neatsakyti.</p>
                                </footer>
                            </body>
                        </html>";


                        payment.PridetiTaskai = pointsToAdd;
                        payment.MokejimoStatusas = MokejimoStatusas.Apmoketa;
                        _dbContext.SaveChanges();
                        // Send email notification
                        var userEmail = _dbContext.Naudotojai.Find(payment.KlientasId).ElektroninisPastas;
                        var emailSent = await _emailService.SendEmailAsync(
                            recipientEmail: userEmail,
                            subject: "Užsakymo Patvirtinimas",
                            message: htmlMessage
                        );
                    }
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Stripe Klaida: {ex.Message}");
                return BadRequest();
            }
        }

        [HttpPost("create-checkout-transfer")]
        public async Task<ActionResult> CreateCheckoutBank([FromQuery] int orderId, [FromQuery] string PvmMoketojoKodas)
        {
            if (!CheckUser(orderId, int.Parse(User.FindFirstValue("UserId"))))
            {
                return Unauthorized("Neturite teisės apmokėti šio užsakymo.");
            }
            if (IsPaid(orderId))
            {
                //Return that order is paid for
                return BadRequest("Užsakymas jau apmokėtas.");
            }

            var order = _dbContext.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {orderId} nerastas.");
            }

            foreach (var item in order.PrekesUzsakymai)
            {
                var product = _dbContext.Prekes.Find(item.PrekeId);
                if (product != null)
                {
                    product.Kiekis -= item.Kiekis;
                }
                _dbContext.SaveChanges();
            }

            var payment = CreatePayment(orderId, ApmokejimoMetodas.Pavedimas, PvmMoketojoKodas);
            var pointsToAdd = (int)(order.Suma * 0.05);
            // Send email with payment details
            var htmlMessage = $@"
            <html>
                <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <img src='https://cdn3.iconfinder.com/data/icons/nature-emoji/50/Marijuana-512.png' alt='Stotele Logo' style='max-width: 150px;'>
                    </div>

                    <h2 style='color: #28a745; text-align: center;'>Užsakymo Patvirtinimas</h2>
                    <p style='text-align: center;'>Užsakymas su ID: <strong>{orderId}</strong> sėkmingai pateiktas.</p>
                    <p style='text-align: center;'>Pasirinktas mokėjimo būdas: Bankiniu pavedimu.</p>

                    <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                        <thead>
                            <tr>
                                <th style='border-bottom: 2px solid #ccc; text-align: left; padding: 8px;'>Prekė</th>
                                <th style='border-bottom: 2px solid #ccc; text-align: right; padding: 8px;'>Kaina (€)</th>
                                <th style='border-bottom: 2px solid #ccc; text-align: center; padding: 8px;'>Kiekis</th>
                            </tr>
                        </thead>
                        <tbody>
                            {string.Join("", payment.Uzsakymas.PrekesUzsakymai.Select(pu => $@"
                                <tr>
                                    <td style='border-bottom: 1px solid #eee; padding: 8px;'>{pu.Preke.Pavadinimas}</td>
                                    <td style='border-bottom: 1px solid #eee; padding: 8px; text-align: right;'>
                                        {(pu.Kaina.HasValue ? pu.Kaina.Value.ToString("0.00") : "0.00")}
                                    </td>
                                    <td style='border-bottom: 1px solid #eee; padding: 8px; text-align: center;'>{pu.Kiekis}</td>
                                </tr>
                            "))}
                        </tbody>
                    </table>

                    <div style='margin-top: 20px; text-align: right;'>
                        <p><strong>Bendra Suma:</strong> €{order.Suma.ToString("0.00")}</p>
                        <p><strong>Taškai už šį užsakymą:</strong> {pointsToAdd}</p>
                    </div>

                    <!-- Bank Transfer Instructions -->
                    <div style='margin-top: 30px; padding: 15px; background-color: #f8f9fa; border: 1px solid #ddd; border-radius: 5px;'>
                        <h3 style='color: #28a745; text-align: center;'>Bankinio Pavedimo Instrukcijos</h3>
                        <p>Norėdami apmokėti užsakymą bankiniu pavedimu, atlikite pervedimą naudodami šią informaciją:</p>
                        <ul style='list-style-type: none; padding: 0;'>
                            <li><strong>Gavėjas:</strong> Žolės Stotelė</li>
                            <li><strong>Banko sąskaita:</strong> LT12 3456 7890 1234 5678</li>
                            <li><strong>Banko pavadinimas:</strong> Swedbank</li>
                            <li><strong>Mokėjimo paskirtis:</strong> Užsakymo ID: {orderId}</li>
                            <li><strong>Suma:</strong> €{order.Suma.ToString("0.00")}</li>
                        </ul>
                        <p style='color: #666; font-size: 14px; text-align: center;'>
                            Jūsų užsakymas bus pradėtas vykdyti, kai tik gausime mokėjimą.
                        </p>
                    </div>

                    <div style='text-align: center; margin-top: 20px;'>
                        <a href='https://localhost:5173/uzsakymas/{orderId}' 
                            style='background-color: #28a745; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-weight: bold;'>
                            Peržiūrėti Užsakymą
                        </a>
                    </div>

                    <!-- Footer -->
                    <footer style='margin-top: 30px; text-align: center; font-size: 12px; color: #aaa;'>
                        <p>&copy; {DateTime.Now.Year} Žolės Stotelė. Visos teisės saugomos.</p>
                        <p>Šis laiškas buvo sugeneruotas automatiškai, prašome neatsakyti.</p>
                    </footer>
                </body>
            </html>";

            var userEmail = _dbContext.Naudotojai.Find(payment.KlientasId).ElektroninisPastas;
            await _emailService.SendEmailAsync(
                recipientEmail: userEmail,
                subject: "Užsakymo Patvirtinimas",
                message: htmlMessage
            );

            return Ok();
        }

        [HttpPost("create-checkout-cash")]
        public async Task<ActionResult> CreateCheckoutCashAsync([FromQuery] int orderId, [FromQuery] string PvmMoketojoKodas)
        {
            Console.WriteLine("PVM: " + PvmMoketojoKodas);
            Console.WriteLine("Order ID: " + orderId);
            var UserId = int.Parse(User.FindFirstValue("UserId"));
            if (!CheckUser(orderId, UserId))
            {
                return Unauthorized("Neturite teisės apmokėti šio užsakymo.");
            }

            if (IsPaid(orderId))
            {
                //Return that order is paid for
                return BadRequest("Užsakymas jau apmokėtas.");
            }

            var order = _dbContext.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {orderId} nerastas.");
            }
            foreach (var item in order.PrekesUzsakymai)
            {
                var product = _dbContext.Prekes.Find(item.PrekeId);
                if (product != null)
                {
                    product.Kiekis -= item.Kiekis;
                }
                _dbContext.SaveChanges();
            }

            var payment = CreatePayment(orderId, ApmokejimoMetodas.Grynaisiais, PvmMoketojoKodas);

            payment.MokejimoStatusas = MokejimoStatusas.Apmoketa;
            _dbContext.SaveChanges();

            // Send email notification
            var userEmail = _dbContext.Naudotojai.Find(UserId).ElektroninisPastas;
            var emailSent = await _emailService.SendEmailAsync(
                recipientEmail: userEmail,
                subject: "Užsakymo Patvirtinimas",
                message: $"Užsakymas su ID: {orderId} sėkmingai pateiktas. Kurjeris susisieks telefonu. Ačiū, kad perkate pas mus!"
            );
            return Ok();
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromQuery] int orderId, [FromQuery] string PvmMoketojoKodas)
        {
            if (!CheckUser(orderId, int.Parse(User.FindFirstValue("UserId"))))
            {
                return Unauthorized("Neturite teisės apmokėti šio užsakymo.");
            }

            if (IsPaid(orderId))
            {
                //Return that order is paid for
                return BadRequest("Užsakymas jau apmokėtas.");
            }

            var order = _dbContext.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {orderId} nerastas.");
            }

            CreatePayment(orderId, ApmokejimoMetodas.BankoKortele, PvmMoketojoKodas);

            // Create line items for each product in the order
            var lineItems = order.PrekesUzsakymai.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Kaina * 100),
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Preke.Pavadinimas,
                    },
                },
                Quantity = item.Kiekis,
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:5173/success",
                CancelUrl = "https://localhost:5173/cancel",
                ClientReferenceId = orderId.ToString()
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }


        //Helperiai

        private bool CheckUser(int orderId, int userId)
        {
            var order = _dbContext.Uzsakymai.Find(orderId);
            if (order == null)
            {
                return false;
            }

            return order.NaudotojasId == userId;
        }

        private Apmokejimas CreatePayment(int orderId, ApmokejimoMetodas apmokejimoMetodas, string PvmMoketojoKodas)
        {
            var order = _dbContext.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            var check = _dbContext.Apmokejimai.FirstOrDefault(a => a.UzsakymasId == orderId);
            if (check != null)
            {
                if (check.MokejimoStatusas == MokejimoStatusas.Apmoketa)
                {
                    return check;
                }

                //update
                check.ApmokejimoMetodas = apmokejimoMetodas;
                check.PvmMoketojoKodas = PvmMoketojoKodas;
                _dbContext.SaveChanges();
                return check;
            }

            var payment = new Apmokejimas
            {
                GalutineSuma = order.Suma,
                MokejimoStatusas = MokejimoStatusas.Neapmoketa,
                ApmokejimoMetodas = apmokejimoMetodas,
                SaskaitosFakturosNumeris = orderId,
                PvmMoketojoKodas = PvmMoketojoKodas,
                PanaudotiTaskai = 0, // Logic to calculate points if needed
                PridetiTaskai = 0,
                KlientasId = order.NaudotojasId,
                UzsakymasId = order.Id
            };

            _dbContext.Apmokejimai.Add(payment);
            _dbContext.SaveChanges();

            return payment;
        }
    }
}