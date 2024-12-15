using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrekeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrekeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Preke
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrekeDTO>>> GetPrekes()
        {
            return await _context.Prekes
                .Select(preke => new PrekeDTO
                {
                    Id = preke.Id,
                    Pavadinimas = preke.Pavadinimas,
                    Kaina = preke.Kaina
                })
                .ToListAsync();
        }

        // GET: api/Preke/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PrekeDTO>> GetPreke(int id)
        {
            var preke = await _context.Prekes
                .Select(p => new PrekeDTO
                {
                    Id = p.Id,
                    Pavadinimas = p.Pavadinimas,
                    Kaina = p.Kaina,
                    Aprasymas = p.Aprasymas,
                    NuotraukosUrl = p.NuotraukosUrl
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preke == null)
            {
                return NotFound();
            }

            return preke;
        }


        // POST: api/Preke
        [HttpPost]
        public async Task<ActionResult<Preke>> CreatePreke(KurtiArKoreguotiPrekeDTO dto)
        {
            var vadybininkas = await _context.Vadybininkai.FindAsync(dto.VadybininkasId);
            if (vadybininkas == null)
            {
                return BadRequest($"Vadybininkas with ID {dto.VadybininkasId} does not exist.");
            }

            var preke = new Preke
            {
                Kaina = dto.Kaina,
                Pavadinimas = dto.Pavadinimas,
                Kodas = dto.Kodas,
                GaliojimoData = DateTime.SpecifyKind(dto.GaliojimoData, DateTimeKind.Utc),
                Kiekis = dto.Kiekis,
                Ismatavimai = dto.Ismatavimai,
                NuotraukosUrl = dto.NuotraukosUrl,
                GarantinisLaikotarpis = DateTime.SpecifyKind(dto.GarantinisLaikotarpis, DateTimeKind.Utc),
                Aprasymas = dto.Aprasymas,
                RekomendacijosSvoris = dto.RekomendacijosSvoris,
                Mase = dto.Mase,
                VadybininkasId = dto.VadybininkasId
            };

            _context.Prekes.Add(preke);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreke), new { id = preke.Id }, preke);
        }


        // PUT: api/Preke/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePreke(int id, KurtiArKoreguotiPrekeDTO dto)
        {
            var preke = await _context.Prekes.FindAsync(id);

            if (preke == null)
            {
                return NotFound();
            }

            preke.Kaina = dto.Kaina;
            preke.Pavadinimas = dto.Pavadinimas;
            preke.Kodas = dto.Kodas;
            preke.GaliojimoData = dto.GaliojimoData;
            preke.Kiekis = dto.Kiekis;
            preke.Ismatavimai = dto.Ismatavimai;
            preke.NuotraukosUrl = dto.NuotraukosUrl;
            preke.GarantinisLaikotarpis = dto.GarantinisLaikotarpis;
            preke.Aprasymas = dto.Aprasymas;
            preke.RekomendacijosSvoris = dto.RekomendacijosSvoris;
            preke.Mase = dto.Mase;
            preke.VadybininkasId = dto.VadybininkasId;

            _context.Entry(preke).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrekeExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Preke/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreke(int id)
        {
            var preke = await _context.Prekes.FindAsync(id);

            if (preke == null)
            {
                return NotFound();
            }

            _context.Prekes.Remove(preke);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET List of Prekes: api/Preke/PrekesList
        [HttpGet("PrekesList")]
        public async Task<ActionResult<IEnumerable<Preke>>> GetPrekesList()
        {
            return await _context.Prekes.ToListAsync();
        }

        private bool PrekeExists(int id)
        {
            return _context.Prekes.Any(p => p.Id == id);
        }

    }
}
