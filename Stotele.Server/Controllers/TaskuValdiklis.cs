using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;

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
            return BadRequest("Request body is null.");
        }

        var klientas = await _context.Klientai
            .Include(k => k.Naudotojas)
            .FirstOrDefaultAsync(k => k.NaudotojasId == dto.NaudotojasId);

        if (klientas == null)
        {
            return BadRequest("Invalid NaudotojasId.");
        }

        Taskai taskai = new()
        {
            PabaigosData = DateTime.UtcNow.AddMonths(1),
            Kiekis = dto.Kiekis,
            KlientasId = klientas.Id,
        };

        _context.Taskai.Add(taskai);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTaskai), new { id = taskai.Id }, taskai);
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
    }
}
