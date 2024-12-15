// Kategoriju valdiklis - category controller
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
    public class KategorijaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public KategorijaController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Gauti visas prekiu kategorijas
        [HttpGet("kategorijos")]
        public async Task<ActionResult<IEnumerable<Kategorija>>> GautiKategorijas()
        {
            return await _context.Kategorijos.ToListAsync();
        }

        // Istrinti prekes kategorija
        [HttpDelete("kategorija/istrinti/{naudotojoId}/{kategorijosId}")]
        public async Task<IActionResult> IstrintiKategorija(int naudotojoId, int kategorijosId)
        {
            var kategorija = await _context.Kategorijos.FindAsync(kategorijosId);
            if (kategorija == null)
            {
                return NotFound(new { message = "Kategorija nerasta." });
            }

            var vadybininkas = await _context.Vadybininkai.FindAsync(naudotojoId);
            if (vadybininkas == null)
            {
                return NotFound(new { message = "Vadybininkas duotu naudotojo id neegzistuoja. Negalite istrinti prekes" });
            }

            _context.Kategorijos.Remove(kategorija);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategorija istrinta.", kategorija = kategorija });
        }

        // Prideti nauja prekes kategorija
        [HttpPost("kategorija/prideti/{naudotojoId}/")]
        public async Task<IActionResult> PridetiKategorija(int naudotojoId, PridetiKategorijaDTO dto)
        {
            var vadybininkas = await _context.Vadybininkai.FindAsync(naudotojoId);
            if (vadybininkas == null)
            {
                return NotFound(new { message = "Vadybininkas duotu naudotojo id neegzistuoja. Negalite prideti kategorijos" });
            }

            var kategorijosPavadinimas = await _context.Kategorijos
                .Where(k => k.Pavadinimas == dto.Pavadinimas)
                .FirstOrDefaultAsync();
            if (kategorijosPavadinimas != null)
            {
                return BadRequest(new { message = "Kategorija su tokiu pavadinimu jau egzistuoja." });
            }

            var kategorija = new Kategorija
            {
                Pavadinimas = dto.Pavadinimas,
                Aprasymas = dto.Aprasymas,
                VadybininkasId = naudotojoId,
                Vadybininkas = vadybininkas
            };

            Console.WriteLine(kategorija.Id);

            _context.Kategorijos.Add(kategorija);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategorija prideta.", kategorija = kategorija });
        }
    }
}