using backend.Data;
using backend.Models.Data_Transfer_Objects;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models.Ingredients;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
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
        /// <param name="context">The database context for accessing user data.</param>
        /// <param name="logger">The logger instance for logging application activity.</param>
        public IngredientController(ICareServerContext context, ILogger<IngredientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all ingredient names.
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
        /// Retrieves information about a specific ingredient.
        /// </summary>
        /// <param name="ingredientName">The name of the ingredient to retrieve.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the ingredient if found, or <c>NotFound</c> if not found.
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
    }
}
