using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Data
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        // Liste des jeux achetés par l'utilisateur (relation avec Game)
        public ICollection<Game> OwnedGames { get; set; }

        // Liste des amis de l'utilisateur
        public ICollection<User> Friends { get; set; }
    }
}
