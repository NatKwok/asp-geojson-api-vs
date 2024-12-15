using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp_geojson_api_vs;
using asp_geojson_api_vs.Models;
using GeoJSON.Text.Feature;
using Newtonsoft.Json;
using GeoJSON.Text.Geometry;
using NetTopologySuite.Geometries;
using System.Drawing;

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
            // Fetch data from the database
            var feature = await _context.MilieuxHumides.Take(2).ToListAsync();

            // Map data to GeoJSON Features
            var features = feature.Select(record =>
            {
                if (record.Geom == null)
                    return null;

                if (record.Geom is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                {
                    // Extract coordinates for each polygon in the MultiPolygon
                    var multiPolygonCoordinates = multiPolygon.Geometries
                        .OfType<NetTopologySuite.Geometries.Polygon>()
                        .Select(polygon =>
                        {
                            // Extract the exterior ring
                            var exteriorRing = polygon.ExteriorRing.Coordinates
                                .Select(coord => new[] { coord.X, coord.Y }) // Flip to [latitude, longitude]
                                .ToList();

                            // Extract interior rings (holes)
                            var interiorRings = polygon.InteriorRings
                                .Select(ring => ring.Coordinates
                                .Select(coord => new[] { coord.X, coord.Y })
                                .ToList())
                                .ToList();

                            // Combine exterior and interior rings
                            var polygonCoordinates = new List<List<double[]>> { exteriorRing };
                            polygonCoordinates.AddRange(interiorRings);

                            return polygonCoordinates; // Return as List<List<Position>>
                        })
                        .ToList();

                    // Create a GeoJSON MultiPolygon geometry
                    var geoJsonMultiPolygon = new
                    {
                        type = "MultiPolygon",
                        coordinates = multiPolygonCoordinates
                    };
                    // Add additional properties from your model
                    var properties = new Dictionary<string, object>
                    {
                        { "Id", record.Id },
                        { "mhID", record.MhId },
                        { "ConscClDv", record.ConsClDv }
                    };

                    // Create a GeoJSON Feature
                    return new
                    {
                        type = "Feature",
                        geometry = geoJsonMultiPolygon,
                        properties
                    };
                }

                return null; // Skip if not a MultiPolygon
            })
            .Where(feature => feature != null) // Filter out null features
            .ToList();

            var featureCollection = new
            {
                type = "FeatureCollection",
                features = features
            };

            // Serialize to GeoJSON
            var geoJson = JsonConvert.SerializeObject(featureCollection);

            // Return GeoJSON with the appropriate content type
            return Content(geoJson, "application/json");
        }

        //public async Task<ActionResult<IEnumerable<MilieuxHumide>>> GetMilieuxHumides()
        //{
        //    var feature = await _context.MilieuxHumides.ToListAsync();

        //    var features = feature.Select(record =>
        //    {
        //        if (record.Geom == null)
        //        {
        //            return null;
        //        }

        //        var multiPolygonCoordinates = multiPolygon.Geometries
        //        .OfType<NetTopologySuite.Geometries.Polygon>()
        //        .Select(polygon =>
        //        {
        //            // Extract the exterior ring
        //            var exteriorRing = polygon.ExteriorRing.Coordinates
        //                .Select(coord => new NetTopologySuite.Geometries.Position(coord.Y, coord.X)) // Flip to [latitude, longitude]
        //                .ToList();

        //            // Extract interior rings (holes)
        //            var interiorRings = polygon.InteriorRings
        //                .Select(ring => ring.Coordinates
        //                    .Select(coord => new Position(coord.Y, coord.X))
        //                    .ToList())
        //                .ToList();

        //            // Combine exterior and interior rings
        //            var polygonCoordinates = new List<List<Position>> { exteriorRing };
        //            polygonCoordinates.AddRange(interiorRings);

        //            return polygonCoordinates; // Return as List<List<Position>>
        //        })
        //        .ToList();

        //        var polygon = new Polygon(polygonCoordinates);
        //        var properties = new Dictionary<string, object>
        //    {
        //        { "Id", record.Id },
        //        { "humid_id", record.MhId }, // Example field
        //        { "conclDV", record.ConsClDv } // Example field
        //    };

        //        return new Feature(polygon, properties);
        //    })
        //    .Where(feature => feature != null) // Filter out null features
        //    .ToList();

        //    var featureCollection = new FeatureCollection(features);
        //    var geoJson = JsonConvert.SerializeObject(featureCollection);

        //    return Content(geoJson, "application/json");
        //}

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
