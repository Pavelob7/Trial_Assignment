using Microsoft.EntityFrameworkCore;
using RouteGraphBackend.Models;

namespace RouteGraphBackend.Data
{
    public class RouteContext : DbContext
    {
        public RouteContext(DbContextOptions<RouteContext> options)
            : base(options)
        {
        }

        public DbSet<Point> Points { get; set; }

        public DbSet<Track> Tracks { get; set; }
    }
}
