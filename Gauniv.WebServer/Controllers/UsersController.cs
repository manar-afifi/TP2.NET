using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Gauniv.WebServer.Controllers
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/1.0.0/users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    OwnedGames = u.OwnedGames.Select(g => g.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/1.0.0/users/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _context.Users
                .Include(u => u.OwnedGames) // Inclut les jeux possédés par l'utilisateur
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    OwnedGames = u.OwnedGames.Select(g => g.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/1.0.0/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("L'utilisateur ne peut pas être nul.");
            }

            var user = new User
            {
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email
            };

            var result = await _userManager.CreateAsync(user, "DefaultPassword123!"); // Crée un mot de passe par défaut pour l'utilisateur

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            userDto.Id = user.Id;
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        // PUT: api/1.0.0/users/{id}
        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("L'utilisateur ne peut pas être nul.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Mise à jour des informations de l'utilisateur
            user.UserName = userDto.UserName;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/1.0.0/users/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Supprimer l'utilisateur
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/1.0.0/users/{userId}/friends/{friendId}
        [Authorize]
        [HttpPost("{userId}/friends/{friendId}")]
        public async Task<IActionResult> AddFriend(string userId, string friendId)
        {
            var user = await _context.Users.FindAsync(userId);
            var friend = await _context.Users.FindAsync(friendId);

            if (user == null || friend == null)
            {
                return NotFound("Utilisateur ou ami introuvable.");
            }

            
            if (!user.Friends.Contains(friend))
            {
                user.Friends.Add(friend);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
