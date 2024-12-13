// Rekomentacijų valdiklis - recommendation controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text; // Add this line to include the namespace for ApplicationDbContext
using System.Linq;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RekomendacijaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public RekomendacijaController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // Endpointai skirti megstamom kategorijom
        // GET: api/Rekomendacija/{naudotojasId}/megstamos-kategorijos
        [HttpGet("/megstamos-kategorijos/{naudotojasId}")]
        public async Task<ActionResult<IEnumerable<MegstamaKategorijaDTO>>> GetMegstamosKategorijos(int naudotojasId)
        {
            var klientas = await _context.Klientai.FindAsync(naudotojasId);
            if (klientas == null)
            {
                return NotFound(new { Message = "Klientas nerastas" });
            }

            var megstamosKategorijos = await _context.MegstamosKategorijos
                .Where(mk => mk.KlientasId == naudotojasId)
                .Include(mk => mk.Kategorija)
                .Select(mk => new MegstamaKategorijaDTO
                {
                    Id = mk.Id,
                    PridejimoData = mk.PridejimoData,
                    KategorijaId = mk.KategorijaId,
                    KategorijaPavadinimas = mk.Kategorija.Pavadinimas,
                    KategorijaAprasymas = mk.Kategorija.Aprasymas
                })
                .ToListAsync();

            if (megstamosKategorijos.Count == 0)
            {
                return NotFound(new { Message = "Klientas neturi megstamu kategoriju." });
            }

            return Ok(megstamosKategorijos);
        }

        [HttpPut("/megstamos-kategorijos/prideti/{naudotojasId}/{kategorijosId}")]
        public async Task<IActionResult> AddMegstamaKategorija(int naudotojasId, int kategorijosId)
        {
            var klientas = await _context.Klientai.FindAsync(naudotojasId);
            if (klientas == null)
            {
                return NotFound(new { Message = "Klientas nerastas" });
            }

            var rekomendacija = await _context.Kategorijos.FindAsync(kategorijosId);
            if (rekomendacija == null)
            {
                return NotFound(new { Message = "Kategorija nerasta" });
            }

            var egzistuojaKategorija = await _context.MegstamosKategorijos
                .Where(mk => mk.KlientasId == naudotojasId && mk.KategorijaId == kategorijosId)
                .FirstOrDefaultAsync();

            if (egzistuojaKategorija != null)
            {
                return BadRequest(new { Message = "Megstama kategorija jau prideta" });
            }

            var megstamaKategorija = new MegstamaKategorija
            {
                KlientasId = naudotojasId,
                KategorijaId = kategorijosId,
                PridejimoData = DateTime.UtcNow,
                Kategorija = rekomendacija,
                Klientas = klientas
            };

            _context.MegstamosKategorijos.Add(megstamaKategorija);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Megstama kategorija prideta", MegstamaKategorija = megstamaKategorija });
        }

        [HttpDelete("/megstamos-kategorijos/istrinti/{naudotojasId}/{kategorijosId}")]
        public async Task<IActionResult> DeleteMegstamaKategorija(int naudotojasId, int kategorijosId)
        {
            var megstamaKategorija = await _context.MegstamosKategorijos
                .Where(mk => mk.KlientasId == naudotojasId && mk.KategorijaId == kategorijosId)
                .FirstOrDefaultAsync();

            if (megstamaKategorija == null)
            {
                return NotFound(new { Message = "Nurodytas neteisignas megstamos kategorijosId arba naudotojoId" });
            }

            _context.MegstamosKategorijos.Remove(megstamaKategorija);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Megstama kategorija istrinta", MegstamaKategorija = megstamaKategorija });
        }



        // Endpointai skirti kategorijom
        [HttpGet("/blokuotos-rekomendacijos/{naudotojasId}")]
        public async Task<ActionResult<IEnumerable<UzblokuotaRekomendacijaDTO>>> GetBlokuotosRekomendacijos(int naudotojasId)
        {
            var klientas = await _context.Klientai.FindAsync(naudotojasId);
            if (klientas == null)
            {
                return NotFound(new { Message = "Klientas nerastas" });
            }

            var blokuotosRekomendacijos = await _context.UzblokuotosRekomendacijos
                .Where(ur => ur.KlientasId == naudotojasId)
                .Include(ur => ur.Preke)
                .Select(ur => new UzblokuotaRekomendacijaDTO
                {
                    Id = ur.Id,
                    KlientasId = ur.KlientasId,
                    PrekeId = ur.PrekeId,
                    NuotraukosUrl = ur.Preke.NuotraukosUrl,
                    Pavadinimas = ur.Preke.Pavadinimas,
                    Aprasymas = ur.Preke.Aprasymas
                })
                .ToListAsync();

            if (blokuotosRekomendacijos.Count == 0)
            {
                return NotFound(new { Message = "Klientas neturi blokuotu rekomendaciju." });
            }

            return Ok(blokuotosRekomendacijos);
        }

        [HttpPut("/blokuotos-rekomendacijos/uzblokuoti/{naudotojasId}/{prekesId}")]
        public async Task<IActionResult> AddBlokuotaRekomendacija(int naudotojasId, int prekesId)
        {
            var klientas = await _context.Klientai.FindAsync(naudotojasId);
            if (klientas == null)
            {
                return NotFound(new { Message = "Klientas nerastas" });
            }

            var preke = await _context.Prekes.FindAsync(prekesId);
            if (preke == null)
            {
                return NotFound(new { Message = "Preke nerasta" });
            }

            var egzistuojaRekomendacija = await _context.UzblokuotosRekomendacijos
                .Where(ur => ur.KlientasId == naudotojasId && ur.PrekeId == prekesId)
                .FirstOrDefaultAsync();

            if (egzistuojaRekomendacija != null)
            {
                return BadRequest(new { Message = "Rekomendacija jau uzblokuota" });
            }

            var uzblokuotaRekomendacija = new UzblokuotaRekomendacija
            {
                PridejimoData = DateTime.UtcNow,
                KlientasId = naudotojasId,
                Klientas = klientas,
                PrekeId = prekesId,
                Preke = preke
            };

            _context.UzblokuotosRekomendacijos.Add(uzblokuotaRekomendacija);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Rekomendacija uzblokuota", UzblokuotaRekomendacija = uzblokuotaRekomendacija });
        }

        [HttpDelete("/blokuotos-rekomendacijos/atblokuoti/{naudotojasId}/{prekesId}")]
        public async Task<IActionResult> DeleteBlokuotaRekomendacija(int naudotojasId, int prekesId)
        {
            var uzblokuotaRekomendacija = await _context.UzblokuotosRekomendacijos
                .Where(ur => ur.KlientasId == naudotojasId && ur.PrekeId == prekesId)
                .FirstOrDefaultAsync();

            if (uzblokuotaRekomendacija == null)
            {
                return NotFound(new { Message = "Nurodytas neteisingas prekesId arba naudotojoId" });
            }

            _context.UzblokuotosRekomendacijos.Remove(uzblokuotaRekomendacija);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Rekomendacija atblokuota", UzblokuotaRekomendacija = uzblokuotaRekomendacija });
        }
    }
}