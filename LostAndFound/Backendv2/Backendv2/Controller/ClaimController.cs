using LostAndFound.WPF.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.EF_Core;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]s")] // → /api/Claims
    public class ClaimController : ControllerBase
    {
        private readonly Context _context;

        public ClaimController(Context context)
        {
            _context = context;
        }

        // GET /api/Claims
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> Get()
        {
            return Ok(await _context.Claims.ToListAsync());
        }

        // GET /api/Claims/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> Get(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            return Ok(claim);
        }

        // GET /api/Claims/ByItem/3   ← wird von ClaimsViewModel benötigt
        [HttpGet("ByItem/{itemId}")]
        public async Task<ActionResult<IEnumerable<Claim>>> GetByItem(int itemId)
        {
            var claims = await _context.Claims
                .Where(c => c.ItemId == itemId)
                .ToListAsync();
            return Ok(claims);
        }

        // POST /api/Claims
        [HttpPost]
        public async Task<ActionResult<Claim>> Post([FromBody] Claim value)
        {
            _context.Claims.Add(value);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
        }

        // PUT /api/Claims/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Claim value)
        {
            var existing = await _context.Claims.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(value);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/Claims/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
