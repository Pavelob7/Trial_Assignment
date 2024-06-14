namespace Trial_Assignment.Models
{
    public class Track
    {
        public int FirstId { get; set; }
        public int SecondId { get; set; }
        public int Distance { get; set; }
        public Surface Surface { get; set; }
        public MaxSpeed MaxSpeed { get; set; }
    }

    public enum Surface
    {
        Sand,
        Asphalt,
        Ground
    }

    public enum MaxSpeed
    {
        Fast,
        Normal,
        Slow
    }
}
