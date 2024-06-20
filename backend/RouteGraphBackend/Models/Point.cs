using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RouteGraphBackend.Models
{
    public class Point
    {
        public int PointId { get; set; }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public int UploadId { get; set; }
        public Upload Upload { get; set; }
    }
}

