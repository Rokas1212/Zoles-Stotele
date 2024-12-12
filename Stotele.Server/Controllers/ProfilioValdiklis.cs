using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;
using System.Security.Cryptography;
using System.Text;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfilisController(ApplicationDbContext context)
        {
            _context = context;
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

        // POST: api/Profilis/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistruotiNaudotojaDTO dto)
        {
            // Check if email already exists
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


        // GET: api/Profilis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfilisDTO>>> GetAllProfiles()
        {
            var administrators = await _context.Naudotojai
                .Where(n => n.Administratorius)
                .Select(n => new ProfilisDTO
                {
                    Id = n.Id,
                    Lytis = n.Lytis,
                    ElektroninisPastas = n.ElektroninisPastas,
                    Vardas = n.Vardas,
                    Slapyvardis = n.Slapyvardis,
                    Pavarde = n.Pavarde,
                })
                .ToListAsync();

            var clients = await _context.Klientai
                .Include(k => k.Naudotojas)
                .Select(k => new ProfilisDTO
                {
                    Id = k.Naudotojas.Id,
                    Lytis = k.Naudotojas.Lytis,
                    ElektroninisPastas = k.Naudotojas.ElektroninisPastas,
                    Vardas = k.Naudotojas.Vardas,
                    Slapyvardis = k.Naudotojas.Slapyvardis,
                    Pavarde = k.Naudotojas.Pavarde,
                    PastoKodas = k.PastoKodas,
                    Miestas = k.Miestas,
                    GimimoData = k.GimimoData,
                    Adresas = k.Adresas,
                })
                .ToListAsync();

            var managers = await _context.Vadybininkai
                .Include(v => v.Naudotojas)
                .Include(v => v.Parduotuve)
                .Select(v => new ProfilisDTO
                {
                    Id = v.Naudotojas.Id,
                    Lytis = v.Naudotojas.Lytis,
                    ElektroninisPastas = v.Naudotojas.ElektroninisPastas,
                    Vardas = v.Naudotojas.Vardas,
                    Slapyvardis = v.Naudotojas.Slapyvardis,
                    Pavarde = v.Naudotojas.Pavarde,
                    Skyrius = v.Skyrius,
                    ParduotuveId = v.ParduotuveId,
                })
                .ToListAsync();

            // Add users who are not administrators, clients, or managers
            var others = await _context.Naudotojai
                .Where(n => !n.Administratorius && !_context.Klientai.Any(k => k.NaudotojasId == n.Id) 
                            && !_context.Vadybininkai.Any(v => v.NaudotojasId == n.Id))
                .Select(n => new ProfilisDTO
                {
                    Id = n.Id,
                    Lytis = n.Lytis,
                    ElektroninisPastas = n.ElektroninisPastas,
                    Vardas = n.Vardas,
                    Slapyvardis = n.Slapyvardis,
                    Pavarde = n.Pavarde,
                })
                .ToListAsync();

            var allProfiles = administrators
                .Concat(clients)
                .Concat(managers)
                .Concat(others)
                .GroupBy(p => p.Id)
                .Select(g => g.First()) // Ensure unique profiles
                .ToList();

            return Ok(allProfiles);
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

            if (naudotojas.Administratorius)
            {
                return new
                {
                    naudotojas.Id,
                    naudotojas.Lytis,
                    naudotojas.ElektroninisPastas,
                    naudotojas.Vardas,
                    naudotojas.Slapyvardis,
                    naudotojas.Pavarde,
                    Administratorius = true
                };
            }

            var klientas = await _context.Klientai
                .Include(k => k.Naudotojas)
                .FirstOrDefaultAsync(k => k.NaudotojasId == id);

            if (klientas != null)
            {
                return new
                {
                    klientas.Naudotojas.Id,
                    klientas.Naudotojas.Lytis,
                    klientas.Naudotojas.ElektroninisPastas,
                    klientas.Naudotojas.Vardas,
                    klientas.Naudotojas.Slapyvardis,
                    klientas.Naudotojas.Pavarde,
                    klientas.PastoKodas,
                    klientas.Miestas,
                    klientas.GimimoData,
                    klientas.Adresas
                };
            }

            var vadybininkas = await _context.Vadybininkai
                .Include(v => v.Naudotojas)
                .Include(v => v.Parduotuve)
                .FirstOrDefaultAsync(v => v.NaudotojasId == id);

            if (vadybininkas != null)
            {
                return new
                {
                    vadybininkas.Naudotojas.Id,
                    vadybininkas.Naudotojas.Lytis,
                    vadybininkas.Naudotojas.ElektroninisPastas,
                    vadybininkas.Naudotojas.Vardas,
                    vadybininkas.Naudotojas.Slapyvardis,
                    vadybininkas.Naudotojas.Pavarde,
                    vadybininkas.Skyrius,
                    vadybininkas.ParduotuveId
                };
            }

            return NotFound("Naudotojo profilio tipas nerastas.");
        }

        // POST: api/Profilis
        [HttpPost]
        public async Task<ActionResult> CreateProfile(CreateNaudotojasDTO dto)
        {
            var naudotojas = new Naudotojas
            {
                Lytis = dto.Lytis,
                ElektroninisPastas = dto.ElektroninisPastas,
                Slaptazodis = HashPassword(dto.Slaptazodis),
                Vardas = dto.Vardas,
                Slapyvardis = dto.Slapyvardis,
                Pavarde = dto.Pavarde,
                Administratorius = dto.Administratorius
            };

            _context.Naudotojai.Add(naudotojas);
            await _context.SaveChangesAsync();

            if (dto.IsKlientas)
            {
                var klientas = new Klientas
                {
                    NaudotojasId = naudotojas.Id,
                    PastoKodas = dto.KlientasPastoKodas,
                    Miestas = dto.KlientasMiestas,
                    GimimoData = dto.KlientasGimimoData,
                    Adresas = dto.KlientasAdresas
                };

                _context.Klientai.Add(klientas);
                await _context.SaveChangesAsync();
            }

            if (dto.IsVadybininkas)
            {
                var vadybininkas = new Vadybininkas
                {
                    NaudotojasId = naudotojas.Id,
                    Skyrius = dto.VadybininkasSkyrius,
                    ParduotuveId = dto.VadybininkasParduotuveId
                };

                _context.Vadybininkai.Add(vadybininkas);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetProfile), new { id = naudotojas.Id }, naudotojas);
        }

        // PUT: api/Profilis/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, CreateNaudotojasDTO dto)
        {
            var naudotojas = await _context.Naudotojai.FindAsync(id);

            if (naudotojas == null)
            {
                return NotFound("Naudotojas nerastas.");
            }

            naudotojas.Lytis = dto.Lytis;
            naudotojas.ElektroninisPastas = dto.ElektroninisPastas;
            naudotojas.Vardas = dto.Vardas;
            naudotojas.Slapyvardis = dto.Slapyvardis;
            naudotojas.Pavarde = dto.Pavarde;

            if (!string.IsNullOrEmpty(dto.Slaptazodis))
            {
                naudotojas.Slaptazodis = HashPassword(dto.Slaptazodis);
            }

            _context.Entry(naudotojas).State = EntityState.Modified;

            if (dto.IsKlientas)
            {
                var klientas = await _context.Klientai.FirstOrDefaultAsync(k => k.NaudotojasId == id);

                if (klientas == null)
                {
                    klientas = new Klientas
                    {
                        NaudotojasId = naudotojas.Id
                    };

                    _context.Klientai.Add(klientas);
                }

                klientas.PastoKodas = dto.KlientasPastoKodas;
                klientas.Miestas = dto.KlientasMiestas;
                klientas.GimimoData = dto.KlientasGimimoData;
                klientas.Adresas = dto.KlientasAdresas;
            }

            if (dto.IsVadybininkas)
            {
                var vadybininkas = await _context.Vadybininkai.FirstOrDefaultAsync(v => v.NaudotojasId == id);

                if (vadybininkas == null)
                {
                    vadybininkas = new Vadybininkas
                    {
                        NaudotojasId = naudotojas.Id
                    };

                    _context.Vadybininkai.Add(vadybininkas);
                }

                vadybininkas.Skyrius = dto.VadybininkasSkyrius;
                vadybininkas.ParduotuveId = dto.VadybininkasParduotuveId;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Profilis/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var naudotojas = await _context.Naudotojai.FindAsync(id);

            if (naudotojas == null)
            {
                return NotFound("Naudotojas nerastas.");
            }

            var klientas = await _context.Klientai.FirstOrDefaultAsync(k => k.NaudotojasId == id);
            if (klientas != null)
            {
                _context.Klientai.Remove(klientas);
            }

            var vadybininkas = await _context.Vadybininkai.FirstOrDefaultAsync(v => v.NaudotojasId == id);
            if (vadybininkas != null)
            {
                _context.Vadybininkai.Remove(vadybininkas);
            }

            _context.Naudotojai.Remove(naudotojas);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
