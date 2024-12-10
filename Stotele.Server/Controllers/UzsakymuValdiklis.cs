// Uzsakymu valdiklis - order controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.Collections.Generic;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UzsakymuController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StripeSettings _stripeSettings;

        public UzsakymuController(ApplicationDbContext dbContext, IOptions<StripeSettings> stripeSettings)
        {
            _dbContext = dbContext;
            _stripeSettings = stripeSettings.Value;
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            Console.WriteLine(_stripeSettings.SecretKey);
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 2000, // Amount in cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Sample Product",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:5173/success",
                CancelUrl = "https://localhost:5173/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }

    }
}