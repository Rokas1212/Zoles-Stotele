using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models;
using Stotele.Server.Models.ApplicationDbContexts;

namespace Stotele.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskaiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Taskai
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Taskai>>> GetTaskai()
        {
            return await _context.Taskai
                .Include(t => t.Klientas)
                .Include(t => t.Apmokejimas)
                .ToListAsync();
        }

        // GET: api/Taskai/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Taskai>> GetTaskai(int id)
        {
            var taskai = await _context.Taskai
                .Include(t => t.Klientas)
                .Include(t => t.Apmokejimas)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskai == null)
            {
                return NotFound();
            }

            return taskai;
        }

        // POST: api/Taskai
        [HttpPost]
        public async Task<ActionResult<Taskai>> CreateTaskai(Taskai taskai)
        {
            _context.Taskai.Add(taskai);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskai), new { id = taskai.Id }, taskai);
        }

        // PUT: api/Taskai/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskai(int id, Taskai taskai)
        {
            if (id != taskai.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskai).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskaiExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Taskai/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskai(int id)
        {
            var taskai = await _context.Taskai.FindAsync(id);

            if (taskai == null)
            {
                return NotFound();
            }

            _context.Taskai.Remove(taskai);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskaiExists(int id)
        {
            return _context.Taskai.Any(t => t.Id == id);
        }
    }
}
