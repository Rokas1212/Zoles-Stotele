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

        private readonly string _webhookSecret;

        public ApmokejimuController(ApplicationDbContext dbContext, IOptions<StripeSettings> stripeSettings)
        {
            _dbContext = dbContext;
            _stripeSettings = stripeSettings.Value;
            _webhookSecret = _stripeSettings.WebhookSecret ?? throw new InvalidOperationException("Webhook secret nenustatytas.");
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

                    if (payment != null)
                    {
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


        [HttpPost("create-checkout-session/{orderId}&{PvmMoketojoKodas}")]
        public ActionResult CreateCheckoutSession(int orderId, string PvmMoketojoKodas)
        {
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

        private Apmokejimas CreatePayment(int orderId, ApmokejimoMetodas apmokejimoMetodas, string pvmMoketojoKodas)
        {
            var order = _dbContext.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }

            var payment = new Apmokejimas
            {
                GalutineSuma = order.Suma,
                MokejimoStatusas = MokejimoStatusas.Neapmoketa,
                ApmokejimoMetodas = apmokejimoMetodas,
                SaskaitosFakturosNumeris = orderId,
                PvmMoketojoKodas = pvmMoketojoKodas,
                PanaudotiTaskai = 0, // Logic to calculate points if needed
                PridetiTaskai = 0,  // Logic to calculate points if needed
                KlientasId = order.NaudotojasId,
                UzsakymasId = order.Id
            };

            _dbContext.Apmokejimai.Add(payment);
            _dbContext.SaveChanges();

            return payment;
        }
    }
}