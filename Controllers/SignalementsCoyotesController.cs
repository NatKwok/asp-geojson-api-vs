using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp_geojson_api_vs;
using asp_geojson_api_vs.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using NetTopologySuite.Geometries;

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
            var feature = await _context.SignalementsCoyotes.Take(2).ToListAsync();

            //Map data to GeoJSON Features
            var features = feature.Select(record =>
            {

                // Check if Geom is not null
                if (record.Geom == null)
                {
                    // Handle cases where Geom is null (e.g., skip the record or provide a default value)
                    return null;
                }

                if (record.Geom is NetTopologySuite.Geometries.Point Point)

                {
                    // Replace with the actual fields for latitude and longitude in your model
                    var latitude = record.Geom.X;
                    var longitude = record.Geom.Y;

                    // Create a GeoJSON Point geometry
                    var point = new Point(latitude, longitude);

                    // Add additional properties from your model
                    var properties = new Dictionary<string, object>
                    {
                        { "Id", record.Id },
                        { "Date Observed", record.DatObs }, // Example field
                        { "Area", record.Territoire } // Example field
                    };

                    // Create a GeoJSON Feature
                    return new
                    {
                        type = "Feature",
                        geometry = point,
                        properties
                    };
                }

                return null;
            })
            .Where(feature => feature != null)
            .ToList();

            //Create a FeatureCollection
            var featureCollection = new
            {
                type = "FeatureCollection",
                features
            };
            //Serialize to GeoJSON
            var geoJson = JsonConvert.SerializeObject(featureCollection, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    });

            // Return GeoJSON with appropriate content type
            return Content(geoJson, "application/json");
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
