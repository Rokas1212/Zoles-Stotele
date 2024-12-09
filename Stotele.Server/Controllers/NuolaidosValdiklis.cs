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
        public async Task<ActionResult<IEnumerable<Nuolaida>>> GetNuolaidos()
        {
            return await _context.Nuolaidos
                .Include(n => n.Uzsakymas)
                .Include(n => n.Preke)
                .ToListAsync();
        }

        // GET: api/Nuolaida/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Nuolaida>> GetNuolaida(int id)
        {
            var nuolaida = await _context.Nuolaidos
                .Include(n => n.Uzsakymas)
                .Include(n => n.Preke)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nuolaida == null)
            {
                return NotFound();
            }

            return nuolaida;
        }



        // POST: api/Nuolaida
        [HttpPost]
        public async Task<ActionResult<Nuolaida>> CreateNuolaida(Nuolaida nuolaida)
        {
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
