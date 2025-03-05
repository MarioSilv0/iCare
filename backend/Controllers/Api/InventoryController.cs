/// <summary>
/// This file defines the <c>InventoryController</c> class, responsible for managing the inventory of ingredients for authenticated users.
/// It provides API endpoints for retrieving, updating, and deleting user ingredients based on authentication tokens.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

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
    /// The <c>InventoryController</c> class provides endpoints for managing a user's ingredient inventory.
    /// It allows authenticated users to view, update, and delete ingredients linked to their profile.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InventoryController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<InventoryController> _logger;

        /// <summary>
        /// Initializes the <c>InventoryController</c> with necessary dependencies.
        /// </summary>
        /// <param name="context">The database context used for accessing user data and ingredients.</param>
        /// <param name="logger">The logger instance used for logging errors and events.</param>
        public InventoryController(ICareServerContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the list of ingredients associated with the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing a list of <see cref="ItemDTO"/> objects representing the user's ingredients, 
        /// or an error response if authentication fails.
        /// </returns>
        [HttpGet("")]
        public async Task<ActionResult<List<ItemDTO>>> Get()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var ingredients = await _context.UserIngredients
                                                .Where(ui => ui.UserId == id)
                                                .Include(ui => ui.Ingredient)
                                                .Select(ui => new ItemDTO(ui))
                                                .ToListAsync();

                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user ingredients");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Adds or updates the ingredients for the authenticated user.
        /// If an ingredient does not exist, it will be added. Otherwise, its quantity and unit will be updated.
        /// </summary>
        /// <param name="newItems">A list of <see cref="ItemDTO"/> representing the new or updated ingredients.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing a list of <see cref="ItemDTO"/> representing the updated inventory,
        /// or an error response if an issue occurs.
        /// </returns>
        [HttpPut("")]
        public async Task<ActionResult<List<ItemDTO>>> Update([FromBody] List<ItemDTO> newItems)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                if (newItems == null || !newItems.Any())
                {
                    var currentItems = await _context.UserIngredients.Where(ui => ui.UserId == id)
                                                                     .Include(ui => ui.Ingredient)
                                                                     .Select(i => new ItemDTO(i))
                                                                     .ToListAsync();
                    return Ok(currentItems);
                }

                var userIngredients = await _context.UserIngredients
                                                    .Where(ui => ui.UserId == id)
                                                    .Include(ui => ui.Ingredient)
                                                    .ToDictionaryAsync(ui => ui.Ingredient.Name);

                var ingredients = await _context.Ingredients.ToDictionaryAsync(i => i.Name);

                foreach (ItemDTO item in newItems)
                {
                    // Add Item
                    if (!userIngredients.TryGetValue(item.Name, out var existingIngredient))
                    {
                        if (!ingredients.TryGetValue(item.Name, out var ingredient)) continue;

                        var ui = new UserIngredient { IngredientId = ingredient.Id, Ingredient = ingredient, Quantity = item.Quantity, Unit = item.Unit, UserId = id };
                        _context.UserIngredients.Add(ui);
                        userIngredients.Add(item.Name, ui);
                        _logger.LogInformation("Ingredient {IngredientName} has been added to user {UserId}.", item.Name, id);

                    }
                    else //Edit Item
                    {
                        existingIngredient.Quantity = item.Quantity;
                        existingIngredient.Unit = item.Unit;
                        _logger.LogInformation("Ingredient {IngredientName} has been eddited to quantity: {Quantity}; unit: {Unit}; for user {UserId}.", item.Name, item.Quantity, item.Unit, id);
                    }
                }
                await _context.SaveChangesAsync();

                var updatedItems = await _context.UserIngredients.Where(ui => ui.UserId == id)
                                                                 .Include(ui => ui.Ingredient)
                                                                 .Select(i => new ItemDTO(i))
                                                                 .ToListAsync();

                return Ok(updatedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating user ingredients");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes specific ingredients from the authenticated user's inventory.
        /// </summary>
        /// <param name="nameOfItemsToRemove">A list of ingredient names to be removed.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the updated list of ingredients after deletion,
        /// or an error response if an issue occurs.
        /// </returns>
        [HttpDelete("")]
        public async Task<ActionResult<List<ItemDTO>>> Delete([FromBody] List<string> nameOfItemsToRemove)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                if (nameOfItemsToRemove == null || !nameOfItemsToRemove.Any()) {
                    var currentItems = await _context.UserIngredients.Where(ui => ui.UserId == id)
                                                                     .Include(ui => ui.Ingredient)
                                                                     .Select(ui => new ItemDTO(ui))
                                                                     .ToListAsync();
                    return Ok(currentItems);
                }

                var itemsToRemoveSet = new HashSet<string>(nameOfItemsToRemove);

                var itemsToRemove = await _context.UserIngredients
                                          .Where(ui => ui.UserId == id)
                                          .Include(ui => ui.Ingredient)
                                          .Where(ui => itemsToRemoveSet.Contains(ui.Ingredient.Name))
                                          .ToListAsync();

                if (itemsToRemove.Any())
                {
                    _context.UserIngredients.RemoveRange(itemsToRemove);
                    await _context.SaveChangesAsync();
                }

                var updatedItems = await _context.UserIngredients.Where(ui => ui.UserId == id)
                                                                 .Include(ui => ui.Ingredient)
                                                                 .Select(ui => new ItemDTO(ui))
                                                                 .ToListAsync();

                _logger.LogInformation("Ingredients {IngredientsName} have been deleted.", nameOfItemsToRemove.ToString());
                return Ok(updatedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user ingredients");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
