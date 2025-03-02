using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer toutes les catégories
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Games = c.Games.Select(g => g.Name).ToList() // Liste des noms des jeux de la catégorie
                })
                .ToListAsync();

            return categories;
        }

        // Récupérer une catégorie par son ID
        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Games = c.Games.Select(g => g.Name).ToList() // Liste des jeux de la catégorie
                })
                .FirstOrDefaultAsync();

            return category;
        }

        // Créer une nouvelle catégorie
        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.Id = category.Id; // Assigner l'ID généré
            return categoryDto;
        }

        // Mettre à jour une catégorie existante
        public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            // Mise à jour des propriétés de la catégorie
            category.Name = categoryDto.Name;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return categoryDto;
        }

        // Supprimer une catégorie
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        // Ajouter un jeu à une catégorie
        public async Task AddGameToCategoryAsync(int gameId, int categoryId)
        {
            var game = await _context.Games.FindAsync(gameId);
            var category = await _context.Categories.FindAsync(categoryId);

            if (game != null && category != null)
            {
                if (category.Games == null)
                {
                    category.Games = new List<Game>();
                }

                category.Games.Add(game);
                await _context.SaveChangesAsync();
            }
        }

        // Retirer un jeu d'une catégorie
        public async Task RemoveGameFromCategoryAsync(int gameId, int categoryId)
        {
            var game = await _context.Games.FindAsync(gameId);
            var category = await _context.Categories.FindAsync(categoryId);

            if (game != null && category != null && category.Games.Contains(game))
            {
                category.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }
    }
}
