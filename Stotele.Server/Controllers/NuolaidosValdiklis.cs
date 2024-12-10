using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NuolaidaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NuolaidaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Nuolaida
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NuolaidaDTO>>> GetNuolaidos()
        {
            return await _context.Nuolaidos
                .Include(n => n.Preke)
                .Select(n => new NuolaidaDTO
                {
                    Id = n.Id,
                    Procentai = n.Procentai,
                    GaliojimoPabaiga = n.PabaigosData,
                    PrekesPavadinimas = n.Preke.Pavadinimas,
                    PrekesKaina = n.Preke.Kaina
                })
                .ToListAsync();
        }

        // GET: api/Nuolaida/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NuolaidaDTO>> GetNuolaida(int id)
        {
            var nuolaida = await _context.Nuolaidos
                .Include(n => n.Preke)
                .Where(n => n.Id == id)
                .Select(n => new NuolaidaDTO
                {
                    Id = n.Id,
                    Procentai = n.Procentai,
                    GaliojimoPabaiga = n.PabaigosData,
                    PrekesPavadinimas = n.Preke.Pavadinimas,
                    PrekesKaina = n.Preke.Kaina
                })
                .FirstOrDefaultAsync();

            if (nuolaida == null)
            {
                return NotFound();
            }

            return nuolaida;
        }

        // POST: api/Nuolaida
        [HttpPost]
        public async Task<ActionResult<Nuolaida>> CreateNuolaida(SukurtiNuolaidaDTO dto)
        {
            var preke = await _context.Prekes.FindAsync(dto.PrekeId);

            if (preke == null)
            {
                return BadRequest($"Preke with ID {dto.PrekeId} does not exist.");
            }

            var nuolaida = new Nuolaida
            {
                Procentai = dto.Procentai,
                PabaigosData = DateTime.SpecifyKind(dto.GaliojimoPabaiga, DateTimeKind.Utc),
                PrekeId = dto.PrekeId,
                UzsakymasId = dto.UzsakymasId
            };

            _context.Nuolaidos.Add(nuolaida);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNuolaida), new { id = nuolaida.Id }, nuolaida);
        }


        // PUT: api/Nuolaida/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNuolaida(int id, Nuolaida nuolaida)
        {
            if (id != nuolaida.Id)
            {
                return BadRequest();
            }

            _context.Entry(nuolaida).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NuolaidaExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Nuolaida/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNuolaida(int id)
        {
            var nuolaida = await _context.Nuolaidos.FindAsync(id);

            if (nuolaida == null)
            {
                return NotFound();
            }

            _context.Nuolaidos.Remove(nuolaida);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool NuolaidaExists(int id)
        {
            return _context.Nuolaidos.Any(n => n.Id == id);
        }
    }
}
