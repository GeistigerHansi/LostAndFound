using LostAndFound.WPF.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.EF_Core;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]s")] // → /api/Items
    public class ItemController : ControllerBase
    {
        private readonly Context _context;

        public ItemController(Context context)
        {
            _context = context;
        }

        // GET /api/Items
        // GET /api/Items?search=Schlüssel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> Get([FromQuery] string? search)
        {
            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i =>
                    i.Title.Contains(search) ||
                    i.Location.Contains(search) ||
                    i.Category.Contains(search));
            }

            return Ok(await query.ToListAsync());
        }

        // GET /api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> Get(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST /api/Items
        [HttpPost]
        public async Task<ActionResult<Item>> Post([FromBody] Item value)
        {
            _context.Items.Add(value);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
        }

        // PUT /api/Items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Item value)
        {
            var existing = await _context.Items.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(value);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            // Zugehörige Claims mitlöschen
            var claims = _context.Claims.Where(c => c.ItemId == id);
            _context.Claims.RemoveRange(claims);

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
