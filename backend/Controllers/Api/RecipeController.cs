/// <summary>
/// This file defines the <c>RecipeController</c> class, responsible for managing recipe-related API endpoints.
/// It provides endpoints to retrieve all recipes and fetch details of a specific recipe.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-03</date>

using backend.Data;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;
using backend.Models.Recipes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
    /// <summary>
    /// The <c>RecipeController</c> class provides API endpoints for retrieving recipes.
    /// It allows authenticated users to fetch a list of recipes and details of a specific recipe.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RecipeController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<RecipeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>RecipeController</c> class.
        /// </summary>
        /// <param name="context">The database context used for accessing recipe data.</param>
        /// <param name="logger">The logger instance used for logging application activity.</param>
        public RecipeController(ICareServerContext context, ILogger<RecipeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all available recipes.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing a list of <c>RecipeDTO</c> objects 
        /// if recipes are found, or an error response otherwise.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<List<RecipeDTO>>> Get()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var recipes = await _context.Recipes.AsNoTracking()
                                                    .Include(r => r.RecipeIngredients)
                                                    .ThenInclude(ri => ri.Ingredient)
                                                    .Include(r => r.UserRecipes)
                                                    .Select(r => new RecipeDTO(r, id, false))
                                                    .ToListAsync();
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recipes");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves details of a specific recipe by name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe to retrieve.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>RecipeDTO</c> object if found, 
        /// or a <c>NotFound</c> response if the recipe does not exist.
        /// </returns>
        [HttpGet("{recipeName}")]
        public async Task<ActionResult<RecipeDTO>> Get(string recipeName)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var recipe = await _context.Recipes.Include(r => r.RecipeIngredients)
                                                   .ThenInclude(ri => ri.Ingredient)
                                                   .Include(r => r.UserRecipes)
                                                   .FirstOrDefaultAsync(r => r.Name == recipeName);
                if (recipe == null) return NotFound($"Recipe '{recipe}' not found.");

                var publicRecipe = new RecipeDTO(recipe!, id, true);

                return Ok(publicRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recipe {RecipeName}", recipeName);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates the list of recipes in the database, linking them with existing ingredients.
        /// </summary>
        /// <param name="recipes">The list of recipes to update.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRecipes([FromBody] List<RecipeDTO> recipes)
        {
            if (recipes == null || recipes.Count == 0)
                return BadRequest("No recipes provided.");

            try
            {
                var existingIngredients = await _context.Ingredients.ToListAsync();
                var existingRecipes = await _context.Recipes.Include(r => r.RecipeIngredients).ToListAsync();

                foreach (var recipeDto in recipes)
                {
                    if (recipeDto == null) continue;

                    var existingRecipe = existingRecipes.FirstOrDefault(r => r.Name == recipeDto.Name);

                    if (existingRecipe == null)
                    {
                        var newRecipe = new Recipe
                        {
                            Name = recipeDto.Name,
                            Category = recipeDto.Category,
                            Area = recipeDto.Area,
                            Picture = recipeDto.Picture,
                            UrlVideo = recipeDto.UrlVideo,
                            Instructions = recipeDto.Instructions,
                            RecipeIngredients = new List<RecipeIngredient>(),
                            Calories = 0
                        };

                        foreach (var ingredientDto in recipeDto.Ingredients!)
                            ProcessIngredient(existingIngredients, newRecipe, ingredientDto);

                        _context.Recipes.Add(newRecipe);
                    }
                    else
                    {
                        existingRecipe.Category = recipeDto.Category;
                        existingRecipe.Area = recipeDto.Area;
                        existingRecipe.Picture = recipeDto.Picture;
                        existingRecipe.UrlVideo = recipeDto.UrlVideo;
                        existingRecipe.Instructions = recipeDto.Instructions;
                        existingRecipe.Calories = 0;
                        existingRecipe.Proteins = 0     ;
                        existingRecipe.Carbohydrates = 0;
                        existingRecipe.Lipids = 0;
                        existingRecipe.Fibers = 0;

                        existingRecipe.RecipeIngredients.Clear();
                        foreach (var ingredientDto in recipeDto.Ingredients)
                            ProcessIngredient(existingIngredients, existingRecipe, ingredientDto); 
                        _context.Recipes.Update(existingRecipe);

                    }
                }

                await _context.SaveChangesAsync();
                return Ok( new { message = "Recipes updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipes.");
                return StatusCode(500, "An error occurred while updating the recipes");
            }
        }

        /// <summary>
        /// Deletes a specific recipe by name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe to delete.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpDelete("{recipeName}")]
        public async Task<IActionResult> Delete(string recipeName)
        {
            try
            {
                var recipe = await _context.Recipes
                    .Include(r => r.RecipeIngredients)
                    .FirstOrDefaultAsync(r => r.Name == recipeName);

                if (recipe == null)
                    return NotFound($"Recipe '{recipeName}' not found.");

                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Recipe '{recipeName}' deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe {RecipeName}", recipeName);
                return StatusCode(500, "An error occurred while deleting the recipe.");
            }
        }

        /// <summary>
        /// Deletes all recipes from the database.
        /// </summary>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                var recipes = await _context.Recipes.Include(r => r.RecipeIngredients).ToListAsync();

                if (recipes.Count == 0)
                    return NotFound("No recipes found to delete.");

                foreach (var recipe in recipes)
                {
                    _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
                }
                _context.Recipes.RemoveRange(recipes);
                await _context.SaveChangesAsync();

                return Ok(new { message = "All recipes deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all recipes.");
                return StatusCode(500, "An error occurred while deleting all recipes.");
            }
        }


        public static void ProcessIngredient(List<Ingredient> existingIngredients, Recipe recipe, RecipeIngredientDTO ingredientDto)
        {
            var existingIngredient = existingIngredients.FirstOrDefault(i => i.Name == ingredientDto.Name);

            existingIngredient ??= FindBestMatch(ingredientDto.Name!, existingIngredients);

            if (existingIngredient != null)
            {
                var existingRecipeIngredient = recipe.RecipeIngredients
                    .FirstOrDefault(ri => ri.Ingredient.Name == existingIngredient.Name);

                if (existingRecipeIngredient == null)
                {
                    var recipeIngredient = new RecipeIngredient
                    {
                        Recipe = recipe,
                        Ingredient = existingIngredient,
                        Measure = ingredientDto.Measure,
                        Grams = ingredientDto.Grams
                    };

                    recipe.RecipeIngredients.Add(recipeIngredient);
                }

                recipe.Calories += existingIngredient.Kcal * (ingredientDto.Grams ?? 0) / 100;
                recipe.Proteins += existingIngredient.Protein * (ingredientDto.Grams ?? 0) / 100;
                recipe.Carbohydrates += existingIngredient.Carbohydrates * (ingredientDto.Grams ?? 0) / 100;
                recipe.Lipids += existingIngredient.Lipids * (ingredientDto.Grams ?? 0) / 100;
                recipe.Fibers += existingIngredient.Fibers * (ingredientDto.Grams ?? 0) / 100;
                
            }
        }


        private static Ingredient? FindBestMatch(string ingredientName, List<Ingredient> existingIngredients)
        {
            int minDistance = int.MaxValue;
            Ingredient? bestMatch = null;

            foreach (var ingredient in existingIngredients)
            {
                int distance = LevenshteinDistance(ingredientName.ToLower(), ingredient.Name!.ToLower());
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestMatch = ingredient;
                }
            }

            return bestMatch;
        }

        private static int LevenshteinDistance(string s1, string s2)
        {
            int len1 = s1.Length, len2 = s2.Length;
            var dp = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++)
                for (int j = 0; j <= len2; j++)
                {
                    if (i == 0) dp[i, j] = j;
                    else if (j == 0) dp[i, j] = i;
                    else dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + (s1[i - 1] == s2[j - 1] ? 0 : 1)
                    );
                }

            return dp[len1, len2];
        }
    }

}
