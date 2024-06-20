using Microsoft.EntityFrameworkCore;
using RouteGraphBackend.Models;

namespace RouteGraphBackend.Data // Добавьте пространство имён
{
    public class RouteContext : DbContext
    {
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Track> Tracks { get; set; }

        public RouteContext(DbContextOptions<RouteContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Upload>()
                .HasMany(u => u.Points)
                .WithOne(p => p.Upload)
                .HasForeignKey(p => p.UploadId);

            modelBuilder.Entity<Upload>()
                .HasMany(u => u.Tracks)
                .WithOne(t => t.Upload)
                .HasForeignKey(t => t.UploadId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
