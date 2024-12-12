using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProfilisController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GenerateJwtToken(Naudotojas naudotojas)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, naudotojas.Slapyvardis),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, naudotojas.Id.ToString()),
                new Claim(ClaimTypes.Role, naudotojas.Administratorius ? "Administratorius" : "Vartotojas")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST: api/Profilis/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistruotiNaudotojaDTO dto)
        {
            var existingUser = await _context.Naudotojai.FirstOrDefaultAsync(n => n.ElektroninisPastas == dto.ElektroninisPastas);
            if (existingUser != null)
            {
                return BadRequest("Naudotojas su tokiu el. paštu jau egzistuoja.");
            }

            var naudotojas = new Naudotojas
            {
                Lytis = dto.Lytis,
                ElektroninisPastas = dto.ElektroninisPastas,
                Slaptazodis = HashPassword(dto.Slaptazodis),
                Vardas = dto.Vardas,
                Slapyvardis = dto.Slapyvardis,
                Pavarde = dto.Pavarde,
                Administratorius = false
            };

            _context.Naudotojai.Add(naudotojas);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfile), new { id = naudotojas.Id }, new
            {
                naudotojas.Id,
                naudotojas.Vardas,
                naudotojas.Pavarde,
                naudotojas.ElektroninisPastas
            });
        }

        // POST: api/Profilis/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(PrisijungtiDTO dto)
        {
            var naudotojas = await _context.Naudotojai
                .FirstOrDefaultAsync(n => n.Slapyvardis == dto.Slapyvardis);

            if (naudotojas == null || naudotojas.Slaptazodis != HashPassword(dto.Slaptazodis))
            {
                return Unauthorized("Neteisingas naudotojo vardas arba slaptažodis.");
            }

            var token = GenerateJwtToken(naudotojas);

            return Ok(new
            {
                Token = token,
                naudotojas.Id,
                naudotojas.Vardas,
                naudotojas.Pavarde,
                naudotojas.ElektroninisPastas,
                naudotojas.Administratorius
            });
        }

        // GET: api/Profilis/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProfile(int id)
        {
            var naudotojas = await _context.Naudotojai.FirstOrDefaultAsync(n => n.Id == id);

            if (naudotojas == null)
            {
                return NotFound("Naudotojas nerastas.");
            }

            return new
            {
                naudotojas.Id,
                naudotojas.Vardas,
                naudotojas.Pavarde,
                naudotojas.ElektroninisPastas,
                naudotojas.Administratorius
            };
        }
    }
}
