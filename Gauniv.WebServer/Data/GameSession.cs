using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Data
{
    public class GameSession
    {
        [Key]
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Durée du jeu en minutes
        public double Duration => (EndTime - StartTime).TotalMinutes;
    }

}
