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
    }
}