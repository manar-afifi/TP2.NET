using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gauniv.WebServer.Data
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, 9999.99)]
        public decimal Price { get; set; }

        // Stocker le chemin du fichier binaire (le contenu du jeu)
        public string PayloadPath { get; set; }  // Cette propriété pourrait être un chemin vers un fichier stocké en dehors de la base de données

        // Relation avec les catégories (un jeu peut avoir plusieurs catégories)
        public ICollection<Category> Categories { get; set; }

        // Relation avec les utilisateurs qui ont acheté ce jeu
        public ICollection<User> Users { get; set; }
    }
}
