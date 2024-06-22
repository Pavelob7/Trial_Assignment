using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RouteGraphBackend.Models
{
    public class Upload
    {
        public int UploadId { get; set; }
        public long UploadTime { get; set; }
 
        [JsonIgnore]
        public ICollection<Point> Points { get; set; }

        [JsonIgnore]
        public ICollection<Track> Tracks { get; set; }
    }
}
