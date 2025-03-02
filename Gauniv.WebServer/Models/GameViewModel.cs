using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Models
{
    public class GameViewModel
    {
        [Required]
        public int Id { get; set; }  // Identifiant unique du jeu

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }  // Nom du jeu

        [StringLength(500)]  // La description peut être longue
        public string Description { get; set; }  // Description du jeu

        [Required]
        [Range(0, 9999.99)]
        public decimal Price { get; set; }  // Prix du jeu

        public string PayloadPath { get; set; }  // Le chemin vers le fichier binaire du jeu

        // Liste des catégories auxquelles ce jeu appartient
        public List<int> CategoryIds { get; set; }  // Utilisation d'IDs pour simplifier

        // Optionnel: Liste des jeux associés (si tu veux afficher des jeux associés dans le même jeu)
        public List<string> Categories { get; set; }  // Noms des catégories associées
    }
}
