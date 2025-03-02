using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Controllers
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/1.0.0/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Games = c.Games.Select(g => g.Name).ToList() // Liste des jeux associés à la catégorie
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/1.0.0/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Games = c.Games.Select(g => g.Name).ToList() // Liste des jeux associés à la catégorie
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // POST: api/1.0.0/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("La catégorie ne peut pas être nulle.");
            }

            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.Id = category.Id;
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
        }

        // PUT: api/1.0.0/categories/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("La catégorie ne peut pas être nulle.");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Mise à jour de la catégorie
            category.Name = categoryDto.Name;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/1.0.0/categories/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
