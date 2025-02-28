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

                var user = await _context.Users.Include(u => u.UserIngredients)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();

                List<PublicItem> items = user.UserIngredients?.Select(ui => new PublicItem { Name = ui.Ingredient.Name, Quantity = ui.Quantity, Unit = ui.Unit })
                                                        .ToList() ?? new List<PublicItem>();

                return Ok(items);
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

                var user = await _context.Users.Include(u => u.UserIngredients)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();
                if (user.UserIngredients == null) user.UserIngredients = new List<UserIngredient>();

                var ingredients = _context.Ingredients.Select(i => new { i.Id, i.Name })
                                                      .ToList()
                                                      .ToDictionary(i => i.Name);
                
                var userItemsMap = user.UserIngredients.ToDictionary(i => i.Ingredient.Name);

                foreach (PublicItem item in newItems)
                {
                    // Add Item
                    if (!userItemsMap.TryGetValue(item.Name, out var tmp))
                    {
                        ingredients.TryGetValue(item.Name, out var ingredient);
                        UserIngredient newItem = new UserIngredient { IngredientId = ingredient.Id, Quantity = item.Quantity, Unit = item.Unit, UserId = user.Id };
                        user.UserIngredients.Add(newItem);
                        userItemsMap[item.Name] = newItem;
                    }
                    else //Edit Item
                    {
                        tmp.Quantity = item.Quantity;
                        tmp.Unit = item.Unit;
                    }
                }
                await _context.SaveChangesAsync();

                var updatedItems = user.UserIngredients.Select(i => new PublicItem { Name = i.Ingredient.Name, Quantity = i.Quantity, Unit = i.Unit }).ToList();

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

                var user = await _context.Users.Include(u => u.UserIngredients)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();
                if (user.UserIngredients == null || !user.UserIngredients.Any()) return Ok(new List<PublicItem>());

                var itemsToRemove = user.UserIngredients.Where(i => nameOfItemsToRemove.Any(n => n == i.IngredientName)).ToList();

                if (itemsToRemove.Any())
                {
                    _context.UserIngredients.RemoveRange(itemsToRemove);
                    await _context.SaveChangesAsync();
                }

                var updatedItems = user.UserIngredients.Select(i => new PublicItem { Name = i.IngredientName, Quantity = i.Quantity, Unit = i.Unit }).ToList();

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
