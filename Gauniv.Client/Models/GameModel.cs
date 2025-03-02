namespace Gauniv.Client.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool IsDownloaded { get; set; } 
        public string Status { get; set; } 
    }
}
