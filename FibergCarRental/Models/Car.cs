namespace FibergCarRental.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public bool IsAvailable { get; set; }
    }
}
