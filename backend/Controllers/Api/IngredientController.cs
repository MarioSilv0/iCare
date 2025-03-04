/// <summary>
/// This file defines the <c>IngredientController</c> class, responsible for managing ingredient data.
/// It provides API endpoints for retrieving all ingredient names and detailed information about specific ingredients.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date> 

using backend.Data;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
    /// <summary>
    /// Controller <c>IngredientController</c> manages ingredients for authenticated users.
    /// It provides endpoints for retrieving ingredient names and detailed information about individual ingredients.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IngredientController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<IngredientController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>IngredientController</c> class.
        /// </summary>
        /// <param name="context">The database context for accessing ingredient data.</param>
        /// <param name="logger">The logger instance for logging application activity.</param>
        public IngredientController(ICareServerContext context, ILogger<IngredientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all ingredient names from the database.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing a list of ingredient names, or an error response otherwise.
        /// </returns>
        [HttpGet("")]
        public async Task<ActionResult<List<string>>> Get()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var ingredients = await _context.Ingredients.Select(i => i.Name)
                                                            .ToListAsync();

                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ingredients");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific ingredient.
        /// </summary>
        /// <param name="ingredientName">The name of the ingredient to retrieve information for.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the ingredient details if found, or <c>NotFound</c> if the ingredient is not found.
        /// </returns>
        [HttpGet("{ingredientName}")]
        public async Task<ActionResult<PublicIngredient>> Get(string ingredientName)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Name == ingredientName);
                if (ingredient == null) return NotFound($"Ingredient '{ingredientName}' not found.");

                var publicIngredient = new PublicIngredient
                {
                    Name = ingredient.Name,
                    Kcal = ingredient.Kcal,
                    KJ = ingredient.KJ,
                    Protein = ingredient.Protein,
                    Carbohydrates = ingredient.Carbohydrates,
                    Lipids = ingredient.Lipids,
                    Fibers = ingredient.Fibers,
                    Category = ingredient.Category
                };

                return Ok(publicIngredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ingredient");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates the database with missing ingredients based on the provided list.
        /// </summary>
        /// <param name="ingredients">An array of ingredient objects to update the database.</param>
        /// <returns>An <c>ActionResult</c> indicating success or failure.</returns>
        /// <author> Mário </author>
        [HttpPost("update")]
        public async Task<ActionResult> UpdateDB([FromBody] List<PublicIngredient> ingredients)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                if (ingredients == null || ingredients.Count == 0)
                    return BadRequest("Ingredient list cannot be empty.");

                foreach (var ingredient in ingredients)
                {
                    if (string.IsNullOrEmpty(ingredient.Name) || ingredient.Kcal < 0 || ingredient.KJ < 0 || ingredient.Protein < 0 ||
                        ingredient.Carbohydrates < 0 || ingredient.Lipids < 0 || ingredient.Fibers < 0)
                    {
                        return BadRequest($"Invalid data for ingredient: {ingredient.Name}");
                    }
                }

                var existingIngredients = await _context.Ingredients
                    .ToDictionaryAsync(i => i.Name, i => i);

                var newIngredients = ingredients
                    .Where(ing => !existingIngredients.ContainsKey(ing.Name))
                    .Select(ing => new Ingredient
                    {
                        Name = ing.Name,
                        Kcal = ing.Kcal,
                        KJ = ing.KJ,
                        Protein = ing.Protein,
                        Carbohydrates = ing.Carbohydrates,
                        Lipids = ing.Lipids,
                        Fibers = ing.Fibers,
                        Category = ing.Category
                    })
                    .ToList();

                if (newIngredients.Count > 0)
                {
                    await _context.Ingredients.AddRangeAsync(newIngredients);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Message = $"{newIngredients.Count} new ingredients added.", Ingredients = newIngredients });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ingredients database");
                return StatusCode(500, "An error occurred while updating the database.");
            }
        }


    }
}
