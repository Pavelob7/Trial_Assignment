using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Trial_Assignment.Models;

namespace Trial_Assignment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouteController : ControllerBase
    {
        private static readonly List<Point> Points = new List<Point>
        {
            new Point { Id = 1, Name = "Start", Height = 100 },
            new Point { Id = 2, Name = "Checkpoint 1", Height = 150 },
            // Добавь больше точек
        };

        private static readonly List<Track> Tracks = new List<Track>
        {
            new Track { FirstId = 1, SecondId = 2, Distance = 5, Surface = Surface.Asphalt, MaxSpeed = MaxSpeed.Fast },
            // Добавь больше отрезков
        };

        [HttpGet("points")]
        public IEnumerable<Point> GetPoints()
        {
            return Points;
        }

        [HttpGet("tracks")]
        public IEnumerable<Track> GetTracks()
        {
            return Tracks;
        }
    }
}
