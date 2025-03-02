using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Services
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les jeux
        public async Task<List<GameDto>> GetGamesAsync()
        {
            var games = await _context.Games
                .Include(g => g.Categories) // Inclure les catégories associées
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    PayloadPath = g.PayloadPath,
                    Categories = g.Categories.Select(c => c.Name).ToList() // Liste des catégories par nom
                })
                .ToListAsync();

            return games;
        }

        // Récupérer un jeu par son ID
        public async Task<GameDto> GetGameByIdAsync(int id)
        {
            var game = await _context.Games
                .Include(g => g.Categories)
                .Where(g => g.Id == id)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    PayloadPath = g.PayloadPath,
                    Categories = g.Categories.Select(c => c.Name).ToList()
                })
                .FirstOrDefaultAsync();

            return game;
        }

        // Créer un nouveau jeu
        public async Task<GameDto> CreateGameAsync(GameDto gameDto)
        {
            var game = new Game
            {
                Name = gameDto.Name,
                Description = gameDto.Description,
                Price = gameDto.Price,
                PayloadPath = gameDto.PayloadPath,
                Categories = await _context.Categories
                    .Where(c => gameDto.Categories.Contains(c.Name))
                    .ToListAsync()
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            gameDto.Id = game.Id;  // Assigner l'ID généré
            return gameDto;
        }

        public async Task<double> GetAverageTimePlayedByGame(int gameId)
        {
            var sessions = await _context.GameSessions
                .Where(gs => gs.GameId == gameId)
                .ToListAsync();

            if (sessions.Any())
            {
                return sessions.Average(gs => gs.Duration); // Moyenne des durées de jeu
            }

            return 0;
        }

        public async Task<int> GetMaxSimultaneousPlayersForGame(int gameId)
        {
            // Récupérer toutes les sessions pour ce jeu
            var sessions = await _context.GameSessions
                .Where(gs => gs.GameId == gameId)
                .OrderBy(gs => gs.StartTime)
                .ToListAsync();

            int maxPlayers = 0;
            int currentPlayers = 0;

            // Itérer sur les sessions pour simuler les joueurs connectés à chaque instant
            foreach (var session in sessions)
            {
                currentPlayers++;

                // Calculer le nombre de joueurs simultanés à chaque instant
                if (currentPlayers > maxPlayers)
                {
                    maxPlayers = currentPlayers;
                }

                // Réduire le nombre de joueurs à la fin de la session
                var endTime = session.EndTime;
                currentPlayers = sessions.Count(gs => gs.StartTime < endTime && gs.EndTime > session.StartTime);
            }

            return maxPlayers;
        }



        // Mettre à jour un jeu existant
        public async Task<GameDto> UpdateGameAsync(int id, GameDto gameDto)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return null;
            }

            // Mise à jour des propriétés du jeu
            game.Name = gameDto.Name;
            game.Description = gameDto.Description;
            game.Price = gameDto.Price;
            game.PayloadPath = gameDto.PayloadPath;

            // Mettre à jour les catégories du jeu
            game.Categories = await _context.Categories
                .Where(c => gameDto.Categories.Contains(c.Name))
                .ToListAsync();

            _context.Games.Update(game);
            await _context.SaveChangesAsync();

            return gameDto;
        }

        // Supprimer un jeu
        public async Task<bool> DeleteGameAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return false;
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        // Ajouter un jeu à un utilisateur (si nécessaire)
        public async Task AddGameToUserAsync(int gameId, string userId)
        {
            var game = await _context.Games.FindAsync(gameId);
            var user = await _context.Users.FindAsync(userId);

            if (game != null && user != null)
            {
                if (user.OwnedGames == null)
                {
                    user.OwnedGames = new List<Game>();
                }

                user.OwnedGames.Add(game);
                await _context.SaveChangesAsync();
            }
        }

        // Ajouter un jeu à une catégorie
        public async Task AddGameToCategoryAsync(int gameId, int categoryId)
        {
            var game = await _context.Games.FindAsync(gameId);
            var category = await _context.Categories.FindAsync(categoryId);

            if (game != null && category != null)
            {
                if (game.Categories == null)
                {
                    game.Categories = new List<Category>();
                }

                game.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
