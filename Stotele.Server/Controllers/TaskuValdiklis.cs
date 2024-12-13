using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using Newtonsoft.Json;
using Stotele.Server.Models.SessionModels;


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
            taskai.ApmokejimasId = dto.ApmokejimasId; // Nullable, can be null

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

            var qrData = $"https://localhost:5210/api/Taskai/ApplyDiscounts?orderId={request.OrderId}";

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

        // POST: api/Taskai/ApplyDiscounts
        [HttpPost("ApplyDiscounts")]
        public IActionResult ApplyDiscounts([FromQuery] int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("Invalid Order ID.");
            }

            Console.WriteLine($"Processing discounts for Order ID: {orderId}");

            var order = _context.Uzsakymai
                .Include(o => o.PrekesUzsakymai)
                .ThenInclude(pu => pu.Preke)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            decimal totalUpdatedPrice = 0;

            foreach (var orderItem in order.PrekesUzsakymai)
            {
                var discount = _context.Nuolaidos
                    .Where(n => n.PrekeId == orderItem.PrekeId && n.PabaigosData > DateTime.UtcNow)
                    .FirstOrDefault();

                if (discount != null)
                {
                    Console.WriteLine($"Applying discount: {discount.Procentai}% to item: {orderItem.Preke.Pavadinimas}");

                    var originalPrice = orderItem.Preke.Kaina;
                    var discountedPrice = (decimal)originalPrice * (1 - ((decimal)discount.Procentai / 100));
                    orderItem.Preke.Kaina = (double)discountedPrice;

                    _context.Entry(orderItem.Preke).State = EntityState.Modified;
                }

                totalUpdatedPrice += (decimal)orderItem.Preke.Kaina * orderItem.Kiekis;
            }

            order.Suma = (double)totalUpdatedPrice;
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Discounts applied successfully.",
                UpdatedOrder = order,
                TotalPrice = totalUpdatedPrice
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


    }
}
