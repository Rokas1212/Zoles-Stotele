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

        // GET: api/Rekomendacija/{klientasId}/megstamos-kategorijos
        [HttpGet("{klientasId}/megstamos-kategorijos")]
        public async Task<ActionResult<IEnumerable<MegstamaKategorijaDTO>>> GetMegstamosKategorijos(int klientasId)
        {
            var klientas = await _context.Klientai.FindAsync(klientasId);
            if (klientas == null)
            {
                return NotFound(new { Message = "Klientas nerastas" });
            }

            var megstamosKategorijos = await _context.MegstamosKategorijos
                .Where(mk => mk.KlientasId == klientasId)
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

            if (megstamosKategorijos == null)
            {
                return NotFound(new { Message = "Megstamos kategorijos nerastos" });
            }

            return Ok(megstamosKategorijos);
        }
    }
}