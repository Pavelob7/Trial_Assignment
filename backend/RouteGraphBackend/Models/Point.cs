using System.ComponentModel.DataAnnotations;

namespace RouteGraphBackend.Models
{
    public class Point
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Height { get; set; }
    }
}
