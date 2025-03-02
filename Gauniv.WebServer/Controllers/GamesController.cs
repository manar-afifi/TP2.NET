using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Gauniv.WebServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Controllers
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GameService _gameService;

        public GamesController(ApplicationDbContext context, GameService gameService)
        {
            _context = context;
            _gameService = gameService;
            _gameService = gameService;
        }

        // GET: api/1.0.0/games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames([FromQuery] string categoryFilter = "", [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null, [FromQuery] string userId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Games.AsQueryable();

            // Filtrer par jeux possédés par un utilisateur
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(g => g.Users.Any(u => u.Id == userId));
            }

            // Filtrer par catégorie
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(g => g.Categories.Any(c => c.Name.Contains(categoryFilter)));
            }

            // Filtrer par prix
            if (minPrice.HasValue)
            {
                query = query.Where(g => g.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(g => g.Price <= maxPrice.Value);
            }

            // Filtrer par jeux possédés par un utilisateur
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(g => g.Users.Any(u => u.Id == userId));
            }

            var games = await query
                .Include(g => g.Categories)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    Categories = g.Categories.Select(c => c.Name).ToList(),
                    PayloadPath = g.PayloadPath
                })
                .ToListAsync();

            return Ok(games);
        }

        // GET: api/1.0.0/categories/{id}/games
        [HttpGet("{id}/games")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Games)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            var games = category.Games.Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Price = g.Price,
                PayloadPath = g.PayloadPath,
                Categories = g.Categories.Select(c => c.Name).ToList()
            }).ToList();

            return Ok(games);
        }



        // GET: api/1.0.0/games/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
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
                    Categories = g.Categories.Select(c => c.Name).ToList(),
                    PayloadPath = g.PayloadPath
                })
                .FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        // POST: api/1.0.0/games
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(GameDto gameDto)
        {
            if (gameDto == null)
            {
                return BadRequest("Le jeu ne peut pas être nul.");
            }

            var game = new Game
            {
                Name = gameDto.Name,
                Description = gameDto.Description,
                Price = gameDto.Price,
                PayloadPath = gameDto.PayloadPath
            };

            game.Categories = await _context.Categories
                .Where(c => gameDto.Categories.Contains(c.Name))
                .ToListAsync();

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            gameDto.Id = game.Id;
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, gameDto);
        }

        // POST: api/1.0.0/users/{userId}/games/{gameId}
        [Authorize]
        [HttpPost("users/{userId}/games/{gameId}")]
        public async Task<IActionResult> AddGameToUser(string userId, int gameId)
        {
            var user = await _context.Users.FindAsync(userId);
            var game = await _context.Games.FindAsync(gameId);

            if (user == null || game == null)
            {
                return NotFound();
            }

            if (!user.OwnedGames.Contains(game))
            {
                user.OwnedGames.Add(game);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }


        // PUT: api/1.0.0/games/{id}
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            if (gameDto == null)
            {
                return BadRequest("Le jeu ne peut pas être nul.");
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            game.Name = gameDto.Name;
            game.Description = gameDto.Description;
            game.Price = gameDto.Price;
            game.PayloadPath = gameDto.PayloadPath;

            // Mettre à jour les catégories
            game.Categories = await _context.Categories
                .Where(c => gameDto.Categories.Contains(c.Name))
                .ToListAsync();

            _context.Games.Update(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/1.0.0/games/{id}
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            // Supprimer le fichier binaire si nécessaire
            if (System.IO.File.Exists(game.PayloadPath))
            {
                System.IO.File.Delete(game.PayloadPath); // Suppression du fichier
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/1.0.0/stats
        [HttpGet("stats/advanced")]
        public async Task<ActionResult<Dictionary<string, double>>> GetAdvancedStats()
        {
            var totalGames = await _context.Games.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var averageGamesPerUser = totalUsers > 0 ? (double)await _context.Users.AverageAsync(u => u.OwnedGames.Count) : 0;

            // Calcul du temps moyen joué par chaque jeu
            var games = await _context.Games.ToListAsync();
            var averageTimes = new Dictionary<int, double>();
            foreach (var game in games)
            {
                averageTimes[game.Id] = await _gameService.GetAverageTimePlayedByGame(game.Id);
            }

            // Calcul du nombre maximum de joueurs simultanés pour chaque jeu
            var maxSimultaneousPlayers = new Dictionary<int, int>();
            foreach (var game in games)
            {
                maxSimultaneousPlayers[game.Id] = await _gameService.GetMaxSimultaneousPlayersForGame(game.Id);
            }

            var stats = new Dictionary<string, double>
    {
        { "totalGames", totalGames },
        { "totalCategories", totalCategories },
        { "averageGamesPerUser", averageGamesPerUser },
    };

            return Ok(new { stats, averageTimes, maxSimultaneousPlayers });
        }




        // GET: api/1.0.0/games/download/{id}
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var filePath = game.PayloadPath;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Le fichier du jeu n'existe pas.");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "application/octet-stream")
            {
                FileDownloadName = game.Name + ".bin"
            };
        }



    }
}
