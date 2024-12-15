using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
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
                new Claim("UserId", naudotojas.Id.ToString()), // Custom claim for user ID
                new Claim(JwtRegisteredClaimNames.Sub, naudotojas.Slapyvardis),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, naudotojas.Administratorius ? "Administratorius" : "Naudotojas")
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

            Console.WriteLine("JWT Token Claims:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST: api/Profilis/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistruotiNaudotojaDTO dto)
        {
            var existingUser = await _context.Naudotojai.FirstOrDefaultAsync(n => n.ElektroninisPastas == dto.ElektroninisPastas);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Naudotojas su tokiu el. paštu jau egzistuoja." });
            }

            var existingUsername = await _context.Naudotojai.FirstOrDefaultAsync(n => n.Slapyvardis == dto.Slapyvardis);
            if (existingUsername != null)
            {
                return BadRequest(new { message = "Naudotojas su tokiu slapyvardžiu jau egzistuoja." });
            }

            var naudotojas = new Naudotojas
            {
                Lytis = dto.Lytis.Trim(),
                ElektroninisPastas = dto.ElektroninisPastas.Trim(),
                Slaptazodis = HashPassword(dto.Slaptazodis.Trim()),
                Vardas = dto.Vardas.Trim(),
                Slapyvardis = dto.Slapyvardis.Trim(),
                Pavarde = dto.Pavarde.Trim(),
                Administratorius = false
            };

            _context.Naudotojai.Add(naudotojas);
            await _context.SaveChangesAsync();

            var klientas = new Klientas
            {
                Id = naudotojas.Id,
                NaudotojasId = naudotojas.Id,
                Miestas = dto.Miestas.Trim(),
                Adresas = dto.Adresas.Trim(),
                PastoKodas = dto.PastoKodas,
                GimimoData = dto.GimimoData
            };

            _context.Klientai.Add(klientas);
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

        [HttpGet("klientai")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllClientProfiles()
        {
            var klientai = await _context.Klientai
                .Include(k => k.Naudotojas)
                .Select(k => new
                {
                    k.Naudotojas.Id,
                    k.Naudotojas.Vardas,
                    k.Naudotojas.Pavarde,
                    k.Naudotojas.ElektroninisPastas,
                    k.Naudotojas.Administratorius
                })
                .ToListAsync();

            return Ok(klientai);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllProfiles()
        {
            var naudotojai = await _context.Naudotojai
                .Select(n => new
                {
                    n.Id,
                    n.Vardas,
                    n.Pavarde,
                    n.ElektroninisPastas,
                    n.Administratorius
                })
                .ToListAsync();

            return Ok(naudotojai);
        }


        // PUT: api/Profilis/{id}
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateProfile(int id, [FromBody] RedaguotiNaudotojaDTO dto)
        // {
        //     var naudotojas = await _context.Naudotojai.FirstOrDefaultAsync(n => n.Id == id);
        //     if (naudotojas == null)
        //     {
        //         return NotFound("Naudotojas nerastas.");
        //     }

        //     naudotojas.Vardas = dto.Vardas ?? naudotojas.Vardas;
        //     naudotojas.Pavarde = dto.Pavarde ?? naudotojas.Pavarde;
        //     naudotojas.ElektroninisPastas = dto.ElektroninisPastas ?? naudotojas.ElektroninisPastas;

        //     if (!string.IsNullOrWhiteSpace(dto.Slaptazodis))
        //     {
        //         naudotojas.Slaptazodis = HashPassword(dto.Slaptazodis);
        //     }

        //     if (dto.Administratorius.HasValue)
        //     {
        //         naudotojas.Administratorius = dto.Administratorius.Value;
        //     }

        //     _context.Naudotojai.Update(naudotojas);
        //     await _context.SaveChangesAsync();

        //     return Ok(new
        //     {
        //         naudotojas.Id,
        //         naudotojas.Vardas,
        //         naudotojas.Pavarde,
        //         naudotojas.ElektroninisPastas,
        //         naudotojas.Administratorius
        //     });
        // }

        // PUT: api/Profilis/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] RedaguotiNaudotojasDTO dto)
        {
            try
            {
                Console.WriteLine($"Received Payload: {JsonConvert.SerializeObject(dto)}");

                var naudotojas = await _context.Naudotojai.FirstOrDefaultAsync(n => n.Id == id);
                if (naudotojas == null)
                {
                    return NotFound("Naudotojas nerastas.");
                }

                // Update fields safely
                if (!string.IsNullOrWhiteSpace(dto.Vardas))
                    naudotojas.Vardas = dto.Vardas;

                if (!string.IsNullOrWhiteSpace(dto.ElektroninisPastas))
                    naudotojas.ElektroninisPastas = dto.ElektroninisPastas;

                if (!string.IsNullOrWhiteSpace(dto.Slaptazodis))
                    naudotojas.Slaptazodis = HashPassword(dto.Slaptazodis);

                if (!string.IsNullOrWhiteSpace(dto.Lytis))
                    naudotojas.Lytis = dto.Lytis;

                if (dto.Klientas != null)
                {
                    var klientas = await _context.Klientai.FirstOrDefaultAsync(k => k.NaudotojasId == id);
                    if (klientas == null)
                    {
                        klientas = new Klientas { NaudotojasId = id };
                        _context.Klientai.Add(klientas);
                    }

                    if (!string.IsNullOrWhiteSpace(dto.Klientas.Miestas))
                        klientas.Miestas = dto.Klientas.Miestas;

                    if (!string.IsNullOrWhiteSpace(dto.Klientas.Adresas))
                        klientas.Adresas = dto.Klientas.Adresas;

                    if (dto.Klientas.PastoKodas.HasValue)
                        klientas.PastoKodas = dto.Klientas.PastoKodas.Value;

                    if (dto.Klientas.GimimoData.HasValue)
                    {
                        klientas.GimimoData = DateTime.SpecifyKind(dto.Klientas.GimimoData.Value, DateTimeKind.Utc);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Profilis sėkmingai atnaujintas!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { Message = "Serverio klaida. Bandykite dar kartą.", Details = ex.Message });
            }
        }










        // POST: api/Profilis/register-admin
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegistruotiVadybininkaDTO dto)
        {
            var existingUser = await _context.Naudotojai.FirstOrDefaultAsync(n => n.ElektroninisPastas == dto.ElektroninisPastas);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Naudotojas su tokiu el. paštu jau egzistuoja." });
            }

            var existingUsername = await _context.Naudotojai.FirstOrDefaultAsync(n => n.Slapyvardis == dto.Slapyvardis);
            if (existingUsername != null)
            {
                return BadRequest(new { message = "Naudotojas su tokiu slapyvardžiu jau egzistuoja." });
            }

            var naudotojas = new Naudotojas
            {
                Lytis = dto.Lytis.Trim(),
                ElektroninisPastas = dto.ElektroninisPastas.Trim(),
                Slaptazodis = HashPassword(dto.Slaptazodis.Trim()),
                Vardas = dto.Vardas.Trim(),
                Slapyvardis = dto.Slapyvardis.Trim(),
                Pavarde = dto.Pavarde.Trim(),
                Administratorius = true
            };

            _context.Naudotojai.Add(naudotojas);
            await _context.SaveChangesAsync();

            var klientas = new Klientas
            {
                Id = naudotojas.Id,
                NaudotojasId = naudotojas.Id,
                Miestas = dto.Miestas.Trim(),
                Adresas = dto.Adresas.Trim(),
                PastoKodas = dto.PastoKodas,
                GimimoData = dto.GimimoData
            };

            _context.Klientai.Add(klientas);
            await _context.SaveChangesAsync();

            var parduotuve = await _context.Parduotuve.FirstOrDefaultAsync(p => p.Id == dto.ParduotuveId);
            if (parduotuve == null)
            {
                return BadRequest(new { message = "Parduotuvė nerasta." });
            }

            parduotuve.DarbuotojuKiekis++;
            _context.Parduotuve.Update(parduotuve);
            await _context.SaveChangesAsync();

            var vadybininkas = new Vadybininkas
            {
                Id = naudotojas.Id,
                NaudotojasId = naudotojas.Id,
                Naudotojas = naudotojas,
                ParduotuveId = dto.ParduotuveId,
                Parduotuve = parduotuve,
                Skyrius = dto.Skyrius
            };

            _context.Vadybininkai.Add(vadybininkas);
            await _context.SaveChangesAsync();



            return CreatedAtAction(nameof(GetProfile), new { id = naudotojas.Id }, new
            {
                naudotojas.Id,
                naudotojas.Vardas,
                naudotojas.Pavarde,
                naudotojas.ElektroninisPastas
            });
        }

        // POST: api/Profilis/register-shop
        [HttpPost("register-shop")]
        public async Task<IActionResult> RegisterShop(RegistruotiParduotuveDTO dto)
        {
            var parduotuve = new Parduotuve
            {
                DarboLaikoPradzia = dto.DarboLaikoPradzia,
                DarboLaikoPabaiga = dto.DarboLaikoPabaiga,
                Kvadratura = dto.Kvadratura,
                DarbuotojuKiekis = dto.DarbuotojuKiekis,
                TelNumeris = dto.TelNumeris,
                ElPastas = dto.ElPastas,
                Faksas = dto.Faksas,
                Adresas = dto.Adresas
            };

            _context.Parduotuve.Add(parduotuve);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Parduotuvė sėkmingai užregistruota.", parduotuve.Id });
        }
    }
}
