using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Models
{
    public class CategoryViewModel
    {
        [Required]
        public int Id { get; set; }  // Identifiant unique de la catégorie

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }  // Nom de la catégorie

        // Liste des noms des jeux associés à cette catégorie
        public List<string> Games { get; set; }  // Liste des noms des jeux de la catégorie

 
    }
}
