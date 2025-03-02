namespace Gauniv.WebServer.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Games { get; set; } // Liste des noms des jeux, ou une liste d'IDs si nécessaire
    }
}
