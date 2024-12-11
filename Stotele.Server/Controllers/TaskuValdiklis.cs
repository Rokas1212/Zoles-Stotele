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
            var klientas = await _context.Klientai.FindAsync(dto.KlientasId);
            if (klientas == null)
            {
                return BadRequest("Invalid KlientasId.");
            }

            Taskai taskai = new()
            {
                PabaigosData = dto.PabaigosData,
                Kiekis = dto.Kiekis,
                KlientasId = dto.KlientasId,
                ApmokejimasId = dto.ApmokejimasId ?? 0
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

            var klientas = await _context.Klientai.FindAsync(dto.KlientasId);
            if (klientas == null)
            {
                return BadRequest("Invalid KlientasId.");
            }

            taskai.PabaigosData = dto.PabaigosData;
            taskai.Kiekis = dto.Kiekis;
            taskai.KlientasId = dto.KlientasId;
            taskai.ApmokejimasId = dto.ApmokejimasId ?? 0;

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
