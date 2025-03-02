using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Récupérer tous les utilisateurs
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    OwnedGames = u.OwnedGames.Select(g => g.Name).ToList() // Liste des noms des jeux possédés
                })
                .ToListAsync();

            return users;
        }

        // Récupérer un utilisateur par son ID
        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _context.Users
                .Include(u => u.OwnedGames) // Inclut les jeux possédés
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    OwnedGames = u.OwnedGames.Select(g => g.Name).ToList() // Liste des jeux possédés
                })
                .FirstOrDefaultAsync();

            return user;
        }

        // Inscription de l'utilisateur
        public async Task<IdentityResult> RegisterUserAsync(UserDto userDto, string password)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email
            };

            // Création de l'utilisateur avec mot de passe
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        // Connexion de l'utilisateur
        public async Task<SignInResult> LoginUserAsync(UserDto userDto, string password)
        {
            // Connexion de l'utilisateur avec le nom d'utilisateur et le mot de passe
            var result = await _signInManager.PasswordSignInAsync(userDto.UserName, password, isPersistent: false, lockoutOnFailure: false);
            return result;
        }

        // Mettre à jour un utilisateur existant
        public async Task<IdentityResult> UpdateUserAsync(string id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            // Mise à jour des informations de l'utilisateur
            user.UserName = userDto.UserName;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        // Supprimer un utilisateur
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
