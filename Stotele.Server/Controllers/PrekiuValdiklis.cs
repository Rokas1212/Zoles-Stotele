using System.Security.Claims;
using System.Text.Json;
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

        private readonly IConfiguration _configuration;

        public PrekeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                    NuotraukosUrl = p.NuotraukosUrl,
                    Kodas = p.Kodas,
                    Ismatavimai = p.Ismatavimai,
                    Mase = p.Mase,
                    GarantinisLaikotarpis = p.GarantinisLaikotarpis,
                    GaliojimoData = p.GaliojimoData
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            var categories = await _context.PrekesKategorijos
                .Where(pk => pk.PrekeId == id)
                .Include(pk => pk.Kategorija)
                .Select(pk => new { pk.Kategorija.Pavadinimas, pk.Kategorija.Id })
                .ToListAsync();

            if (preke == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Preke = preke,
                Kategorijos = categories
            });
        }


        // POST: api/Preke
        [HttpPost]
        public async Task<ActionResult<Preke>> CreatePreke(KurtiArKoreguotiPrekeDTO dto)
        {
            Console.WriteLine(dto.VadybininkasId);
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("Neteisingas user ID formatas.");
            }

            var vadybininkas = await _context.Vadybininkai.FindAsync(dto.VadybininkasId);
            if (vadybininkas == null)
            {
                return BadRequest($"Vadybininkas su šiuo ID {dto.VadybininkasId} neegzistuoja.");
            }

            if (dto.VadybininkasId != parsedUserId && !User.IsInRole("Administratorius"))
            {
                return Unauthorized("Neturite teisės kurti prekę šiam vadybininkui.");
            }

            var kiekis = dto.PrekiuParduotuves.Sum(pp => pp.Kiekis);

            var preke = new Preke
            {
                Kaina = dto.Kaina,
                Pavadinimas = dto.Pavadinimas,
                Kodas = dto.Kodas,
                GaliojimoData = DateTime.SpecifyKind(dto.GaliojimoData, DateTimeKind.Utc),
                Kiekis = kiekis,
                Ismatavimai = dto.Ismatavimai,
                NuotraukosUrl = dto.NuotraukosUrl,
                GarantinisLaikotarpis = DateTime.SpecifyKind(dto.GarantinisLaikotarpis, DateTimeKind.Utc),
                Aprasymas = dto.Aprasymas,
                RekomendacijosSvoris = dto.RekomendacijosSvoris,
                Mase = dto.Mase,
                VadybininkasId = dto.VadybininkasId
            };
            _context.Prekes.Add(preke);
            _context.SaveChanges();
            var prekiuParduotuves = dto.PrekiuParduotuves.Select(pp => new PrekesParduotuve
            {
                Kiekis = pp.Kiekis,
                ParduotuveId = pp.ParduotuveId,
                PrekeId = preke.Id
            }).ToList();

            await _context.PrekesParduotuve.AddRangeAsync(prekiuParduotuves);
            await _context.SaveChangesAsync();


            var prekiuKategorijos = dto.PrekiuKategorijos.Select(pk => new PrekesKategorija
            {
                KategorijaId = pk.KategorijaId,
                PrekeId = preke.Id
            })
            .GroupBy(pk => new { pk.KategorijaId, pk.PrekeId })
            .Select(g => g.First())
            .ToList();

            await _context.PrekesKategorijos.AddRangeAsync(prekiuKategorijos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreke), new { id = preke.Id }, preke);
        }


        // PUT: api/Preke/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePreke(int id, RedaguotiPrekeDTO dto)
        {
            var UserId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            if (!int.TryParse(UserId, out int parsedUserId))
            {
                return BadRequest("Neteisingas user ID formatas.");
            }

            var vadybininkas = await _context.Vadybininkai.FindAsync(int.Parse(UserId));
            if (vadybininkas == null)
            {
                return BadRequest("Vadybininkas su šiuo ID neegzistuoja.");
            }

            if (!User.IsInRole("Administratorius") && vadybininkas.Id != parsedUserId)
            {
                return Unauthorized("Neturite teisės redaguoti prekių.");
            }

            var preke = await _context.Prekes.FindAsync(id);

            if (preke == null)
            {
                return NotFound();
            }

            preke.Kaina = dto.Kaina;
            preke.Pavadinimas = dto.Pavadinimas;
            preke.GaliojimoData = dto.GaliojimoData.ToUniversalTime();
            preke.NuotraukosUrl = dto.NuotraukosUrl;
            preke.GarantinisLaikotarpis = dto.GarantinisLaikotarpis.ToUniversalTime();
            preke.Aprasymas = dto.Aprasymas;
            preke.Ismatavimai = preke.Ismatavimai;

            if (dto.PrekiuKategorijos != null)
            {
                var naujosKategorijos = dto.PrekiuKategorijos
                    .Select(pk => new PrekesKategorija
                    {
                        KategorijaId = pk.KategorijaId,
                        PrekeId = preke.Id
                    })
                    .ToList();

                var senosKategorijos = await _context.PrekesKategorijos
                    .Where(pk => pk.PrekeId == preke.Id)
                    .ToListAsync();

                _context.PrekesKategorijos.RemoveRange(senosKategorijos);
                await _context.PrekesKategorijos.AddRangeAsync(naujosKategorijos);
            }

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

        // DELETE: api/Preke/
        [HttpDelete]
        public async Task<IActionResult> DeletePreke([FromQuery] int id)
        {
            var UserId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized("Nežinomas user ID.");
            }

            if (!User.IsInRole("Administratorius"))
            {
                return Unauthorized("Neturite teisės trinti prekių.");
            }

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

        // GET List of Parduotuves
        [HttpGet("ParduotuvesList")]
        public async Task<ActionResult<IEnumerable<Parduotuve>>> GetParduotuvesList()
        {
            return await _context.Parduotuve.ToListAsync();
        }

        // GET: api/Preke/{id}/stores
        [HttpGet("{id}/addresses")]
        public async Task<ActionResult<IEnumerable<string>>> GetStoreAddressesForPreke(int id)
        {
            var addresses = await _context.PrekesParduotuve
                .Where(pp => pp.PrekeId == id)
                .Include(pp => pp.Parduotuve)
                .Select(pp => pp.Parduotuve.Adresas)
                .ToListAsync();

            return Ok(addresses);
        }
    }
}
