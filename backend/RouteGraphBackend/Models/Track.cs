using System.ComponentModel.DataAnnotations;

namespace RouteGraphBackend.Models
{
    public enum Surface
    {
        SAND,
        ASPHALT,
        GROUND
    }

    public enum MaxSpeed
    {
        FAST,
        NORMAL,
        SLOW
    }

    public class Track
    {
        public int Id { get; set; }
        public int FirstId { get; set; } // Первая точка
        public int SecondId { get; set; } // Вторая точка
        public int Distance { get; set; }
        public Surface Surface { get; set; }
        public MaxSpeed MaxSpeed { get; set; }

        // Внешний ключ на маршрут
        public int RouteId { get; set; }
        public Route Route { get; set; }
    }

}
