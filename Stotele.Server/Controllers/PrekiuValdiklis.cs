using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // DTO for fetching a simplified Preke
        public class PrekeDTO
        {
            public int Id { get; set; }
            public string Pavadinimas { get; set; }
            public double Kaina { get; set; }
        }

        // DTO for creating/updating Preke
        public class CreateOrUpdatePrekeDTO
        {
            public double Kaina { get; set; }
            public string Pavadinimas { get; set; }
            public int Kodas { get; set; }
            public DateTime GaliojimoData { get; set; }
            public int Kiekis { get; set; }
            public string Ismatavimai { get; set; }
            public string NuotraukosUrl { get; set; }
            public DateTime GarantinisLaikotarpis { get; set; }
            public string Aprasymas { get; set; }
            public double RekomendacijosSvoris { get; set; }
            public double Mase { get; set; }
            public int VadybininkasId { get; set; } // Only ID of the Vadybininkas
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
                    Kaina = p.Kaina
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
        public async Task<ActionResult<Preke>> CreatePreke(CreateOrUpdatePrekeDTO dto)
        {
            // Check if the related Vadybininkas exists
            var vadybininkas = await _context.Vadybininkai.FindAsync(dto.VadybininkasId);
            if (vadybininkas == null)
            {
                return BadRequest($"Vadybininkas with ID {dto.VadybininkasId} does not exist.");
            }

            // Create a new Preke instance
            var preke = new Preke
            {
                Kaina = dto.Kaina,
                Pavadinimas = dto.Pavadinimas,
                Kodas = dto.Kodas,
                GaliojimoData = dto.GaliojimoData,
                Kiekis = dto.Kiekis,
                Ismatavimai = dto.Ismatavimai,
                NuotraukosUrl = dto.NuotraukosUrl,
                GarantinisLaikotarpis = dto.GarantinisLaikotarpis,
                Aprasymas = dto.Aprasymas,
                RekomendacijosSvoris = dto.RekomendacijosSvoris,
                Mase = dto.Mase,
                VadybininkasId = dto.VadybininkasId // Only set the ID
            };

            _context.Prekes.Add(preke);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreke), new { id = preke.Id }, preke);
        }

        // PUT: api/Preke/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePreke(int id, CreateOrUpdatePrekeDTO dto)
        {
            var preke = await _context.Prekes.FindAsync(id);

            if (preke == null)
            {
                return NotFound();
            }

            // Update fields
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
            preke.VadybininkasId = dto.VadybininkasId; // Only set the ID

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

        private bool PrekeExists(int id)
        {
            return _context.Prekes.Any(p => p.Id == id);
        }
    }
}
