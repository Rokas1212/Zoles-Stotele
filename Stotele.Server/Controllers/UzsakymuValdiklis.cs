// Uzsakymu valdiklis - order controller

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
    public class UzsakymuController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StripeSettings _stripeSettings;

        public UzsakymuController(ApplicationDbContext dbContext, IOptions<StripeSettings> stripeSettings)
        {
            _dbContext = dbContext;
            _stripeSettings = stripeSettings.Value;
        }

        [HttpGet("uzsakymai")]
        public ActionResult GetOrders()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nežinomas user id.");
            }

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("Neteisingas user id formatas.");
            }

            // Fetch orders along with related payments
            var ordersWithPayments = _dbContext.Uzsakymai
                .Where(n => n.NaudotojasId == parsedUserId)
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .Select(o => new
                {
                    Order = o,
                    Payments = _dbContext.Apmokejimai
                        .Where(a => a.UzsakymasId == o.Id)
                        .ToList()
                })
                .ToList();

            return Ok(ordersWithPayments);
        }

        [HttpDelete("uzsakymas/{orderId}")]
        public ActionResult DeleteOrder(int orderId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("Neteisingas user ID formatas.");
            }

            var order = _dbContext.Uzsakymai.Find();
            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {orderId} nerastas.");
            }

            if (order.NaudotojasId != parsedUserId && !User.IsInRole("Admin"))
            {
                return Unauthorized("Neturite teisės ištrinti šio užsakymo.");
            }

            var payment = _dbContext.Apmokejimai.FirstOrDefault(a => a.UzsakymasId == orderId);
            if (payment != null)
            {
                _dbContext.Apmokejimai.Remove(payment);
            }

            _dbContext.Uzsakymai.Remove(order);
            _dbContext.SaveChanges();

            return Ok($"Užsakymas su ID: {orderId} sėkmingai ištrintas.");
        }

        [HttpGet("uzsakymas/{id}")]
        public ActionResult GetOrder(int id)
        {
            var order = _dbContext.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {id} nerastas.");
            }

            return Ok(order);
        }

        [HttpPost("sukurti-uzsakyma")]
        public ActionResult CreateOrder([FromBody] List<CartItem> cartItems)
        {

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("Neteisingas user ID formatas.");
            }

            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("Krepšelis yra tuščias.");
            }

            var order = new Uzsakymas
            {
                Data = DateTime.UtcNow,
                NaudotojasId = parsedUserId,
                Suma = cartItems.Sum(i => i.kaina * i.kiekis)
            };

            _dbContext.Uzsakymai.Add(order);
            _dbContext.SaveChanges();

            var orderItems = new List<PrekesUzsakymas>();

            foreach (var item in cartItems)
            {
                var preke = _dbContext.Prekes.Find(int.Parse(item.id));
                if (preke == null)
                {
                    // Log the issue and continue with the rest of the items
                    Console.WriteLine($"Preke {item.id} neegzistuoja.");
                    continue; // Skip invalid items
                }

                var orderItem = new PrekesUzsakymas
                {
                    Kiekis = item.kiekis,
                    PrekeId = preke.Id,
                    UzsakymasId = order.Id,
                    Preke = preke,
                    Kaina = preke.Kaina
                };

                orderItems.Add(orderItem);
            }

            if (orderItems.Any())
            {
                _dbContext.PrekesUzsakymai.AddRange(orderItems);
                _dbContext.SaveChanges(); // Save all order items
            }
            else
            {
                return BadRequest("Užsakymas negali būti sukurtas, nes prekės yra negalimos.");
            }

            return Ok(new
            {
                Message = "Užsakymas sėkmingai sukurtas.",
                OrderId = order.Id
            });
        }

        [HttpGet("is-confirmed")]
        public ActionResult IsOrderConfirmed([FromQuery] int orderId)
        {
            var userId = User.FindFirstValue("UserId");
            Console.WriteLine($"User ID: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            var order = _dbContext.Uzsakymai.Find(orderId);
            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {orderId} nerastas.");
            }

            if (order.NaudotojasId != int.Parse(userId) && !User.IsInRole("Admin"))
            {
                return Unauthorized("Neturite teisės patikrinti šio užsakymo.");
            }

            return Ok(order.Patvirtintas);
        }

        [HttpPut("confirm-order")]
        public ActionResult ConfirmOrder([FromQuery] int orderId)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            var order = _dbContext.Uzsakymai.Find(orderId);
            if (order == null)
            {
                return NotFound($"Užsakymas su ID: {orderId} nerastas.");
            }

            if (order.NaudotojasId != int.Parse(userId))
            {
                return Unauthorized("Neturite teisės patvirtinti šio užsakymo.");
            }

            order.Patvirtintas = true;
            _dbContext.SaveChanges();

            return Ok($"Užsakymas su ID: {orderId} sėkmingai patvirtintas.");
        }
    }
}