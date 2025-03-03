using backend.Data;
using backend.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RecipeController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(ICareServerContext context, ILogger<RecipeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<string>>> Get()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var recipes = await _context.Recipes.Include(i => i.RecipeIngredients)
                                                    .ThenInclude(ri => ri.Ingredient)
                                                    .Select(i => new { i.Name, i.Picture, calories = i.RecipeIngredients.Sum(ri => ri.Ingredient.Kcal * (ri.Quantity / 100.0)) })
                                                    .ToListAsync();

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ingredients");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{recipeName}")]
        public async Task<ActionResult<PublicIngredient>> Get(string recipeName)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var recipe = await _context.Recipes.Where(i => i.Name == recipeName)
                                                   .Include(r => r.RecipeIngredients)
                                                   .ThenInclude(ri => ri.Ingredient)
                                                   .Select(r => new
                                                    {
                                                         r.Name,
                                                        r.Picture,
                                                        Ingredients = r.RecipeIngredients.Select(ri => new
                                                        {
                                                            ri.Ingredient.Name,
                                                            ri.Quantity,
                                                            ri.Unit
                                                        }).ToList()
                                                    })
                                                   .FirstOrDefaultAsync();
                if (recipe == null) return NotFound($"Ingredient '{recipe}' not found.");

                // Falta deixar receita so a retornar dados publicos (PublicRecipe)

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ingredient");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
