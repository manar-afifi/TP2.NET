using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Liste des jeux appartenant à cette catégorie
        public ICollection<Game> Games { get; set; }
    }
}
