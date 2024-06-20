using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteGraphBackend.Data;
using RouteGraphBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteGraphBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteDataController : ControllerBase
    {
        private readonly Data.RouteContext _context;

        public RouteDataController(Data.RouteContext context)
        {
            _context = context;
        }

        // GET: api/RouteData/PointsAndTracks
        [HttpGet("PointsAndTracks")]
        public async Task<IActionResult> GetPointsAndTracks()
        {
            var uploads = await _context.Uploads
                .Include(u => u.Points)
                .Include(u => u.Tracks)
                .ToListAsync();

            var response = new Dictionary<long, object>();

            foreach (var upload in uploads)
            {
                var points = upload.Points.Select(p => new
                {
                    id = p.PointId,
                    name = p.Name,
                    height = p.Height
                }).ToList();

                var tracks = upload.Tracks.Select(t => new
                {
                    firstId = t.FirstId,
                    secondId = t.SecondId,
                    distance = t.Distance,
                    surface = t.Surface.ToString().ToUpper(),
                    maxSpeed = t.MaxSpeed.ToString().ToUpper()
                }).ToList();

                response[upload.UploadTime] = new { points, tracks };
            }

            return Ok(response);
        }
    }
}
