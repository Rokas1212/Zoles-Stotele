using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using Newtonsoft.Json;
using Stotele.Server.Models.SessionModels;
using System.Security.Claims;
using System.Text;


namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskaiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        private const string CartSessionKey = "ShoppingCart";

        // GET: api/Taskai
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskaiDTO>>> GetTaskai()
        {
            var taskai = await _context.Taskai
                .Include(t => t.Klientas)
                    .ThenInclude(k => k.Naudotojas)
                .Include(t => t.Apmokejimas)
                .Select(t => new TaskaiDTO
                {
                    Id = t.Id,
                    PabaigosData = t.PabaigosData,
                    Kiekis = t.Kiekis,
                    KlientasId = t.KlientasId,
                    KlientasVardas = t.Klientas.Naudotojas.Vardas,
                    KlientasPavarde = t.Klientas.Naudotojas.Pavarde,
                    ApmokejimasId = t.ApmokejimasId,
                    GalutineSuma = t.Apmokejimas != null ? (decimal?)t.Apmokejimas.GalutineSuma : null
                })
                .ToListAsync();

            return Ok(taskai);
        }

        // GET: api/Taskai/Naudotojas/{naudotojasId}
        [HttpGet("Naudotojas/{naudotojasId}")]
        public async Task<ActionResult<IEnumerable<TaskaiDTO>>> GetTaskaiByNaudotojasId(int naudotojasId)
        {
            var taskai = await _context.Taskai
                .Include(t => t.Klientas)
                    .ThenInclude(k => k.Naudotojas)
                .Include(t => t.Apmokejimas)
                .Where(t => t.Klientas.NaudotojasId == naudotojasId)
                .Select(t => new TaskaiDTO
                {
                    Id = t.Id,
                    PabaigosData = t.PabaigosData,
                    Kiekis = t.Kiekis,
                    KlientasId = t.KlientasId,
                    KlientasVardas = t.Klientas.Naudotojas.Vardas,
                    KlientasPavarde = t.Klientas.Naudotojas.Pavarde,
                    ApmokejimasId = t.ApmokejimasId,
                    GalutineSuma = t.Apmokejimas != null ? (decimal?)t.Apmokejimas.GalutineSuma : null
                })
                .ToListAsync();

            return Ok(taskai);
        }

        // GET: api/Taskai/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskaiDTO>> GetTaskai(int id)
        {
            var taskai = await _context.Taskai
                .Include(t => t.Klientas)
                    .ThenInclude(k => k.Naudotojas)
                .Include(t => t.Apmokejimas)
                .Where(t => t.Id == id)
                .Select(t => new TaskaiDTO
                {
                    Id = t.Id,
                    PabaigosData = t.PabaigosData,
                    Kiekis = t.Kiekis,
                    KlientasId = t.KlientasId,
                    KlientasVardas = t.Klientas.Naudotojas.Vardas,
                    KlientasPavarde = t.Klientas.Naudotojas.Pavarde,
                    ApmokejimasId = t.ApmokejimasId,
                    GalutineSuma = t.Apmokejimas != null ? (decimal?)t.Apmokejimas.GalutineSuma : null
                })
                .FirstOrDefaultAsync();

            if (taskai == null)
            {
                return NotFound();
            }

            return Ok(taskai);
        }

        // POST: api/Taskai
        [HttpPost]
        public async Task<ActionResult<Taskai>> SukurtiTaskus(SukurtiTaskusDTO dto)
        {
            var klientas = await _context.Klientai.Include(k => k.Naudotojas).FirstOrDefaultAsync(k => k.NaudotojasId == dto.KlientasId);
            if (klientas == null)
            {
                return BadRequest("Invalid NaudotojasId.");
            }

            Taskai taskai = new()
            {
                PabaigosData = dto.PabaigosData,
                Kiekis = dto.Kiekis,
                KlientasId = klientas.Id,
                ApmokejimasId = dto.ApmokejimasId // Nullable, can be null
            };

            _context.Taskai.Add(taskai);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskai), new { id = taskai.Id }, taskai);
        }

        // POST: api/Taskai/AddPoints
        [HttpPost("AddPoints")]
        public async Task<ActionResult<Taskai>> AddPointsToUser(PridetiTaskusDTO dto)
        {
            if (dto == null)
            {
                Console.WriteLine("Request body is null.");
                return BadRequest("Request body is null.");
            }

            Console.WriteLine($"Received request to add points: NaudotojasId = {dto.NaudotojasId}, Kiekis = {dto.Kiekis}");

            var klientas = await _context.Klientai
                .Include(k => k.Naudotojas)
                .FirstOrDefaultAsync(k => k.NaudotojasId == dto.NaudotojasId);

            if (klientas == null)
            {
                Console.WriteLine($"No Klientas found for NaudotojasId: {dto.NaudotojasId}");
                return BadRequest("Invalid NaudotojasId.");
            }

            try
            {
                Taskai taskai = new()
                {
                    PabaigosData = DateTime.UtcNow.AddMonths(1),
                    Kiekis = dto.Kiekis,
                    KlientasId = klientas.Id,
                };

                _context.Taskai.Add(taskai);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully added points: {taskai.Kiekis} for KlientasId: {klientas.Id}");
                return CreatedAtAction(nameof(GetTaskai), new { id = taskai.Id }, taskai);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding points: {ex.Message}");
                return StatusCode(500, "Internal server error occurred.");
            }
        }





        // PUT: api/Taskai/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskai(int id, SukurtiTaskusDTO dto)
        {
            var taskai = await _context.Taskai.FindAsync(id);
            if (taskai == null)
            {
                return NotFound();
            }

            var klientas = await _context.Klientai.Include(k => k.Naudotojas).FirstOrDefaultAsync(k => k.NaudotojasId == dto.KlientasId);
            if (klientas == null)
            {
                return BadRequest("Invalid NaudotojasId.");
            }

            taskai.PabaigosData = dto.PabaigosData;
            taskai.Kiekis = dto.Kiekis;
            taskai.KlientasId = klientas.Id;
            taskai.ApmokejimasId = dto.ApmokejimasId;

            _context.Entry(taskai).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Taskai/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskai(int id)
        {
            var taskai = await _context.Taskai.FindAsync(id);
            if (taskai == null)
            {
                return NotFound();
            }

            _context.Taskai.Remove(taskai);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // POST: api/Taskai/GenerateQR
        [HttpPost("GenerateQR")]
        public IActionResult GenerateQRCode(ProcessQRDTO request)
        {
            if (request == null || request.OrderId <= 0)
            {
                return BadRequest("Invalid Order ID.");
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            if (int.TryParse(userId, out int userIdInt) == false)
            {
                return Unauthorized("Invalid user ID.");
            }

            var qrData = $"http://74.234.45.63:8080/api/Taskai/ApplyDiscounts?orderId={request.OrderId}&userId={userIdInt}";

            var baseUrl = "https://api.qrserver.com/v1/create-qr-code/";
            var size = "200x200";
            var qrCodeUrl = $"{baseUrl}?data={Uri.EscapeDataString(qrData)}&size={size}&format=png";

            var response = new QRResponseDTO
            {
                OrderId = request.OrderId,
                QRCodeUrl = qrCodeUrl
            };

            return Ok(response);
        }

        [HttpGet("ApplyDiscounts")]
        public IActionResult ApplyDiscounts([FromQuery] int orderId, [FromQuery] string userId)
        {
            var user = _context.Naudotojai.FirstOrDefault(n => n.Id == int.Parse(userId));
            if (user == null)
                return Unauthorized("User not authenticated.");

            var order = _context.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");

            bool discountAlreadyApplied = order.PrekesUzsakymai.All(item =>
                    item.Kaina != null && item.Kaina < item.Preke.Kaina);

            if (discountAlreadyApplied)
            {
                return BadRequest(new
                {
                    message = "Discounts have already been applied for this order."
                });
            }

            foreach (var orderItem in order.PrekesUzsakymai)
            {
                var basePrice = orderItem.Preke.Kaina;
                var adjustedPrice = orderItem.Kaina ?? basePrice;

                var discount = _context.Nuolaidos
                    .Where(n => n.PrekeId == orderItem.PrekeId && n.PabaigosData > DateTime.UtcNow)
                    .FirstOrDefault();

                if (discount != null)
                {
                    adjustedPrice *= (1 - (double)discount.Procentai / 100);
                }

                orderItem.Kaina = Math.Max(0, adjustedPrice);
                _context.Entry(orderItem).State = EntityState.Modified;
            }

            order.Suma = order.PrekesUzsakymai.Sum(item => (item.Kaina ?? item.Preke.Kaina) * item.Kiekis);

            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(new
            {
                message = "Discounts applied successfully.",
                updatedOrder = order
            });
        }

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        // POST: api/Taskai/UsePoints
        [HttpPost("UsePoints")]
        public async Task<IActionResult> UsePoints([FromBody] NaudotiTaskusDTO dto)
        {
            var user = await _context.Klientai
                .Include(k => k.Naudotojas)
                .FirstOrDefaultAsync(k => k.Naudotojas.Id == dto.UserId);

            if (user == null)
                return BadRequest("Invalid user.");

            var order = await _context.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
                return BadRequest("Order not found.");

            var totalPoints = _context.Taskai
                .Where(t => t.KlientasId == user.Id)
                .Sum(t => t.Kiekis);

            if (dto.PointsToUse > totalPoints)
                return BadRequest("Insufficient points.");

            var discount = dto.PointsToUse / 10.0; // Total discount value
            var totalOrderPrice = order.PrekesUzsakymai.Sum(item => item.Preke.Kaina * item.Kiekis);

            foreach (var item in order.PrekesUzsakymai)
            {
                var itemOriginalPrice = item.Preke.Kaina * item.Kiekis;
                var itemProportionalDiscount = discount * (itemOriginalPrice / totalOrderPrice);

                // Adjust the item's price dynamically without overwriting
                var discountPerUnit = itemProportionalDiscount / item.Kiekis;

                item.Kaina = Math.Max(0, (item.Kaina ?? item.Preke.Kaina) - discountPerUnit);
                _context.Entry(item).State = EntityState.Modified;
            }

            // Deduct points
            var remainingPoints = dto.PointsToUse;
            var userPoints = _context.Taskai.Where(t => t.KlientasId == user.Id).ToList();

            foreach (var taskai in userPoints)
            {
                if (remainingPoints <= 0) break;

                if (taskai.Kiekis <= remainingPoints)
                {
                    remainingPoints -= taskai.Kiekis;
                    _context.Taskai.Remove(taskai);
                }
                else
                {
                    taskai.Kiekis -= remainingPoints;
                    remainingPoints = 0;
                    _context.Taskai.Update(taskai);
                }
            }

            order.Suma = order.PrekesUzsakymai.Sum(item => (item.Kaina ?? 0) * item.Kiekis);

            _context.Uzsakymai.Update(order);
            await _context.SaveChangesAsync();

            var updatedTotalPoints = _context.Taskai
                .Where(t => t.KlientasId == user.Id)
                .Sum(t => t.Kiekis);

            return Ok(new
            {
                updatedOrder = order,
                usedPoints = dto.PointsToUse,
                remainingPoints = updatedTotalPoints
            });
        }





    }
}
