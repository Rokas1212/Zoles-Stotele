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
            // Fetch the naudotojas using its ID
            var naudotojas = await _context.Naudotojai
                .FirstOrDefaultAsync(n => n.Id == id);

            if (naudotojas == null)
            {
                return NotFound("Naudotojas nerastas.");
            }

            var klientas = await _context.Klientai
                .FirstOrDefaultAsync(k => k.Id == naudotojas.Id);

            // Return combined data with Klientas info if available
            return new
            {
                naudotojas.Id,
                naudotojas.Vardas,
                naudotojas.Pavarde,
                naudotojas.ElektroninisPastas,
                naudotojas.Lytis,
                Klientas = klientas != null
                    ? new
                    {
                        klientas.Miestas,
                        klientas.Adresas,
                        klientas.PastoKodas,
                        klientas.GimimoData
                    }
                    : null
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

        // GET: api/Profilis
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


        [HttpPost("add-vadybininkas")]
        public async Task<IActionResult> MakeVadybininkas([FromBody] PridetiVadybininkaDTO dto)
        {
            var naudotojas = await _context.Naudotojai.FindAsync(dto.NaudotojasId);
            if (naudotojas == null)
            {
                return NotFound("Naudotojas nerastas.");
            }

            var existingVadybininkas = await _context.Vadybininkai
                .FirstOrDefaultAsync(v => v.NaudotojasId == dto.NaudotojasId);

            if (existingVadybininkas != null)
            {
                return BadRequest("Naudotojas jau yra vadybininkas.");
            }

            var parduotuve = await _context.Parduotuve.FindAsync(dto.ParduotuveId);
            if (parduotuve == null)
            {
                return NotFound("Parduotuvė nerasta.");
            }

            var vadybininkas = new Vadybininkas
            {
                Id = dto.NaudotojasId,
                NaudotojasId = dto.NaudotojasId,
                Skyrius = dto.Skyrius,
                ParduotuveId = dto.ParduotuveId
            };

            _context.Vadybininkai.Add(vadybininkas);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Naudotojas sėkmingai pridėtas kaip vadybininkas." });
        }

        [HttpGet("all-parduotuves")]
        public async Task<IActionResult> GetAllParduotuves()
        {
            var parduotuves = await _context.Parduotuve
                .Select(p => new
                {
                    p.Id,
                    p.Adresas
                })
                .ToListAsync();

            return Ok(parduotuves);
        }

        // GET: api/Profilis/is-vadybininkas/{id}
        [HttpGet("is-vadybininkas/{id}")]
        public async Task<IActionResult> IsVadybininkas(int id)
        {
            var isVadybininkas = await _context.Vadybininkai.AnyAsync(v => v.NaudotojasId == id);

            if (isVadybininkas)
            {
                return Ok(new { Message = "Naudotojas yra vadybininkas." });
            }

            return NotFound(new { Message = "Naudotojas nėra vadybininkas." });
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

        // DELETE: api/Profilis/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                // Log the deletion attempt
                Console.WriteLine($"Attempting to delete user with ID: {id}");

                // Find the user in the database
                var naudotojas = await _context.Naudotojai.FindAsync(id);
                if (naudotojas == null)
                {
                    Console.WriteLine("User not found.");
                    return NotFound("Naudotojas nerastas.");
                }

                // Delete associated "Klientas" data if it exists
                var klientas = await _context.Klientai.FirstOrDefaultAsync(k => k.NaudotojasId == id);
                if (klientas != null)
                {
                    Console.WriteLine($"Deleting associated client data for user ID: {id}");
                    _context.Klientai.Remove(klientas);
                }

                // If the user is a Vadybininkas, remove the record from the Vadybininkai table
                var vadybininkas = await _context.Vadybininkai.FirstOrDefaultAsync(v => v.NaudotojasId == id);
                if (vadybininkas != null)
                {
                    Console.WriteLine($"Deleting associated Vadybininkas data for user ID: {id}");
                    _context.Vadybininkai.Remove(vadybininkas);
                }

                // Finally, remove the user
                Console.WriteLine($"Deleting user ID: {id}");
                _context.Naudotojai.Remove(naudotojas);

                // Save changes to the database
                await _context.SaveChangesAsync();
                Console.WriteLine("User and related data successfully deleted.");

                return Ok(new { Message = "Naudotojo paskyra ir susiję duomenys sėkmingai ištrinti." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during deletion: {ex.Message}");
                return StatusCode(500, new { Message = "Įvyko serverio klaida.", Details = ex.Message });
            }
        }



    }
}
