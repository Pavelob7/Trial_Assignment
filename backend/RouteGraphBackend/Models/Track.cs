using System.Collections.Generic;
using System.Text.Json.Serialization;


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
        public int TrackId { get; set; }
        public int UploadId { get; set; }
        public int FirstId { get; set; }
        public int SecondId { get; set; }
        public int Distance { get; set; }
        public Surface Surface { get; set; }
        public MaxSpeed MaxSpeed { get; set; }

        [JsonIgnore]
        public Upload Upload { get; set; }
    }

}
