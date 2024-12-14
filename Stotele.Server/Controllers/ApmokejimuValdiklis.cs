// ApmokejimuValdiklis - Payment controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.Collections.Generic;
using Stotele.Server.Models.SessionModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

                        payment.PridetiTaskai = pointsToAdd;
                        payment.MokejimoStatusas = MokejimoStatusas.Apmoketa;
                        _dbContext.SaveChanges();
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
        public ActionResult CreateCheckoutBank([FromQuery] int orderId, [FromQuery] string PvmMoketojoKodas)
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

            var payment = CreatePayment(orderId, ApmokejimoMetodas.Pavedimas, PvmMoketojoKodas);

            // Send email with payment details



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