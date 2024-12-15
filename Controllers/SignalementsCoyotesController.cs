using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp_geojson_api_vs;
using asp_geojson_api_vs.Models;

namespace asp_geojson_api_vs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalementsCoyotesController : ControllerBase
    {
        private readonly MvcWebmapContext _context;

        public SignalementsCoyotesController(MvcWebmapContext context)
        {
            _context = context;
        }

        // GET: api/SignalementsCoyotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignalementsCoyote>>> GetSignalementsCoyotes()
        {
            return await _context.SignalementsCoyotes.ToListAsync();
        }

        // GET: api/SignalementsCoyotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SignalementsCoyote>> GetSignalementsCoyote(int id)
        {
            var signalementsCoyote = await _context.SignalementsCoyotes.FindAsync(id);

            if (signalementsCoyote == null)
            {
                return NotFound();
            }

            return signalementsCoyote;
        }

        // PUT: api/SignalementsCoyotes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSignalementsCoyote(int id, SignalementsCoyote signalementsCoyote)
        {
            if (id != signalementsCoyote.Id)
            {
                return BadRequest();
            }

            _context.Entry(signalementsCoyote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SignalementsCoyoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SignalementsCoyotes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SignalementsCoyote>> PostSignalementsCoyote(SignalementsCoyote signalementsCoyote)
        {
            _context.SignalementsCoyotes.Add(signalementsCoyote);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSignalementsCoyote", new { id = signalementsCoyote.Id }, signalementsCoyote);
        }

        // DELETE: api/SignalementsCoyotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSignalementsCoyote(int id)
        {
            var signalementsCoyote = await _context.SignalementsCoyotes.FindAsync(id);
            if (signalementsCoyote == null)
            {
                return NotFound();
            }

            _context.SignalementsCoyotes.Remove(signalementsCoyote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SignalementsCoyoteExists(int id)
        {
            return _context.SignalementsCoyotes.Any(e => e.Id == id);
        }
    }
}
