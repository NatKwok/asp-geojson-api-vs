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
    public class MilieuxHumidesController : ControllerBase
    {
        private readonly MvcWebmapContext _context;

        public MilieuxHumidesController(MvcWebmapContext context)
        {
            _context = context;
        }

        // GET: api/MilieuxHumides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MilieuxHumide>>> GetMilieuxHumides()
        {
            return await _context.MilieuxHumides.ToListAsync();
        }

        // GET: api/MilieuxHumides/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MilieuxHumide>> GetMilieuxHumide(int id)
        {
            var milieuxHumide = await _context.MilieuxHumides.FindAsync(id);

            if (milieuxHumide == null)
            {
                return NotFound();
            }

            return milieuxHumide;
        }

        // PUT: api/MilieuxHumides/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMilieuxHumide(int id, MilieuxHumide milieuxHumide)
        {
            if (id != milieuxHumide.Id)
            {
                return BadRequest();
            }

            _context.Entry(milieuxHumide).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MilieuxHumideExists(id))
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

        // POST: api/MilieuxHumides
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MilieuxHumide>> PostMilieuxHumide(MilieuxHumide milieuxHumide)
        {
            _context.MilieuxHumides.Add(milieuxHumide);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMilieuxHumide", new { id = milieuxHumide.Id }, milieuxHumide);
        }

        // DELETE: api/MilieuxHumides/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMilieuxHumide(int id)
        {
            var milieuxHumide = await _context.MilieuxHumides.FindAsync(id);
            if (milieuxHumide == null)
            {
                return NotFound();
            }

            _context.MilieuxHumides.Remove(milieuxHumide);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MilieuxHumideExists(int id)
        {
            return _context.MilieuxHumides.Any(e => e.Id == id);
        }
    }
}
