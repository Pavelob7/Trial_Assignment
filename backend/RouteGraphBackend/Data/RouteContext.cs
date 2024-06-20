using Microsoft.EntityFrameworkCore;
using RouteGraphBackend.Models; // Импортируем модели, чтобы использовать сущности

namespace RouteGraphBackend.Data
{
    // Контекст базы данных для вашего приложения
    public class RouteContext : DbContext
    {
        // DbSet для работы с сущностью Upload в базе данных
        public DbSet<Upload> Uploads { get; set; }

        // DbSet для работы с сущностью Point в базе данных
        public DbSet<Point> Points { get; set; }

        // DbSet для работы с сущностью Track в базе данных
        public DbSet<Track> Tracks { get; set; }

        // Конструктор контекста базы данных
        public RouteContext(DbContextOptions<RouteContext> options) : base(options) { }

        // Метод для настройки отношений между сущностями в базе данных
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Определяем связь один ко многим между Upload и Point
            modelBuilder.Entity<Upload>()
                .HasMany(u => u.Points)
                .WithOne(p => p.Upload)
                .HasForeignKey(p => p.UploadId);

            // Определяем связь один ко многим между Upload и Track
            modelBuilder.Entity<Upload>()
                .HasMany(u => u.Tracks)
                .WithOne(t => t.Upload)
                .HasForeignKey(t => t.UploadId);

            base.OnModelCreating(modelBuilder); // Вызываем базовый метод конфигурации модели
        }
    }
}
