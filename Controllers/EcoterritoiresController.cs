using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp_geojson_api_vs;
using asp_geojson_api_vs.Models;
using Newtonsoft.Json;

namespace asp_geojson_api_vs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcoterritoiresController : ControllerBase
    {
        private readonly MvcWebmapContext _context;

        public EcoterritoiresController(MvcWebmapContext context)
        {
            _context = context;
        }

        // GET: api/Ecoterritoires
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ecoterritoire>>> GetEcoterritoires()
        {
            var feature = await _context.Ecoterritoires.Take(2).ToListAsync();
            var features = feature.Select(record =>
            {
                if (record.Geom == null)
                    return null;

                if (record.Geom is NetTopologySuite.Geometries.Polygon polygon)
                {
                    var polygonCoord = new List<List<Double[]>>()
                    {
                        polygon.ExteriorRing.Coordinates
                                .Select(coord => new[] { coord.X, coord.Y }) // Flip to [latitude, longitude]
                                .ToList()
                    };

                    var geoJsonPolygon = new
                    {
                        type = "Polygon",
                        coordinates = polygonCoord
                    };

                    // Add additional properties from your model
                    var properties = new Dictionary<string, object>
                    {
                        { "Id", record.Id },
                        { "Description", record.Text },
                        { "Area", record.ShapeArea }
                    };

                    // Create a GeoJSON Feature
                    return new
                    {
                        type = "Feature",
                        geometry = polygonCoord,
                        properties
                    };
                }

                return null;
            })
                .Where(feature => feature != null) // Filter out null features
                .ToList();

            var featureCollection = new
            {
                type = "FeatureCollection",
                features
            };

            // Serialize to GeoJSON
            var geoJson = JsonConvert.SerializeObject(featureCollection);

            // Return GeoJSON with the appropriate content type
            return Content(geoJson, "application/json");
        }

        // GET: api/Ecoterritoires/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ecoterritoire>> GetEcoterritoire(int id)
        {
            var ecoterritoire = await _context.Ecoterritoires.FindAsync(id);

            if (ecoterritoire == null)
            {
                return NotFound();
            }

            return ecoterritoire;
        }

        // PUT: api/Ecoterritoires/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEcoterritoire(int id, Ecoterritoire ecoterritoire)
        {
            if (id != ecoterritoire.Id)
            {
                return BadRequest();
            }

            _context.Entry(ecoterritoire).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EcoterritoireExists(id))
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

        // POST: api/Ecoterritoires
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Ecoterritoire>> PostEcoterritoire(Ecoterritoire ecoterritoire)
        {
            _context.Ecoterritoires.Add(ecoterritoire);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEcoterritoire", new { id = ecoterritoire.Id }, ecoterritoire);
        }

        // DELETE: api/Ecoterritoires/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEcoterritoire(int id)
        {
            var ecoterritoire = await _context.Ecoterritoires.FindAsync(id);
            if (ecoterritoire == null)
            {
                return NotFound();
            }

            _context.Ecoterritoires.Remove(ecoterritoire);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EcoterritoireExists(int id)
        {
            return _context.Ecoterritoires.Any(e => e.Id == id);
        }
    }
}
