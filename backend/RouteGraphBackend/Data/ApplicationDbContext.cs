using Microsoft.EntityFrameworkCore;

namespace RouteGraphBackend.Data
{
    // Контекст базы данных для вашего приложения
    public class ApplicationDbContext : DbContext
    {
        // Конструктор контекста базы данных
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet для работы с сущностями FileRecord в базе данных
        public DbSet<FileRecord> FileRecords { get; set; }
    }

    // Сущность FileRecord, представляющая запись о загруженном файле
    public class FileRecord
    {
        public int Id { get; set; }         // Идентификатор записи
        public string FileName { get; set; } // Имя файла
        public string FilePath { get; set; } // Путь к файлу
        public DateTime UploadDate { get; set; } // Дата загрузки файла
    }
}
