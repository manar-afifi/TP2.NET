using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Models
{
    public class UserViewModel
    {
        [Required]
        [StringLength(256, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Liste des jeux possédés par l'utilisateur (par noms ou IDs)
        public List<int> OwnedGameIds { get; set; } // Liste des IDs de jeux possédés

        // Optionnel: Si tu veux également gérer le mot de passe et sa confirmation (par exemple, lors de l'édition de l'utilisateur)
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Le mot de passe et sa confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }
}
