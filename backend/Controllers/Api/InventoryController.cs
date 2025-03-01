using backend.Data;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InventoryController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<InventoryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>InventoryController</c> class.
        /// </summary>
        /// <param name="context">The database context for accessing user data.</param>
        /// <param name="logger">The logger instance for logging application activity.</param>
        public InventoryController(ICareServerContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the ingredients of the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>List<PublicItem></c> object if found, or an error response otherwise.
        /// </returns>
        [HttpGet("")]
        public async Task<ActionResult<List<PublicItem>>> Get()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var ingredients = await _context.UserIngredients
                                                .Where(ui => ui.UserId == id)
                                                .Select(ui => new PublicItem
                                                {
                                                    Name = ui.Ingredient.Name,
                                                    Quantity = ui.Quantity,
                                                    Unit = ui.Unit
                                                })
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
        /// Adds or updates the ingredients to the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>List<PublicItem></c> object if found and added/updated correctly, or an error response otherwise.
        /// </returns>
        [HttpPut("")]
        public async Task<ActionResult<List<PublicItem>>> Update([FromBody] List<PublicItem> newItems)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var userIngredients = await _context.UserIngredients
                                                    .Where(ui => ui.UserId == id)
                                                    .Include(ui => ui.Ingredient)
                                                    .ToDictionaryAsync(ui => ui.Ingredient.Name);

                var ingredients = await _context.Ingredients.ToDictionaryAsync(i => i.Name);

                foreach (PublicItem item in newItems)
                {
                    // Add Item
                    if (!userIngredients.TryGetValue(item.Name, out var existingIngredient))
                    {
                        if (!ingredients.TryGetValue(item.Name, out var ingredient)) return BadRequest($"Ingredient '{item.Name}' does not exist.");

                        _context.UserIngredients.Add(new UserIngredient { IngredientId = ingredient.Id, Quantity = item.Quantity, Unit = item.Unit, UserId = id });
                    }
                    else //Edit Item
                    {
                        existingIngredient.Quantity = item.Quantity;
                        existingIngredient.Unit = item.Unit;
                    }
                }
                await _context.SaveChangesAsync();

                var updatedItems = await _context.UserIngredients.Where(ui => ui.UserId == id)
                                                                 .Select(i => new PublicItem { Name = i.Ingredient.Name, Quantity = i.Quantity, Unit = i.Unit })
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
        /// Deletes the ingredients of the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>List<PublicItem></c> object if found and deleted correctly, or an error response otherwise.
        /// </returns>
        [HttpDelete("")]
        public async Task<ActionResult<List<PublicItem>>> Delete([FromBody] List<string> nameOfItemsToRemove)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                if (nameOfItemsToRemove == null || !nameOfItemsToRemove.Any()) {
                    var currentItems = await _context.UserIngredients.Where(ui => ui.UserId == id)
                                                                     .Include(ui => ui.Ingredient)
                                                                     .Select(ui => new PublicItem { Name = ui.Ingredient.Name, Quantity = ui.Quantity, Unit = ui.Unit })
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
                                                                 .Select(ui => new PublicItem { Name = ui.Ingredient.Name, Quantity = ui.Quantity, Unit = ui.Unit })
                                                                 .ToListAsync();

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
