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

        public int FirstId { get; set; }

        public int SecondId { get; set; }

        public int Distance { get; set; }

        public Surface Surface { get; set; }

        public MaxSpeed MaxSpeed { get; set; }
    }
}
