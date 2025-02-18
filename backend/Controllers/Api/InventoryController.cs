using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Text.Json.Nodes;

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
        /// Retrieves the items of the authenticated user.
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

                var user = await _context.Users.Include(u => u.UserItems)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();

                List<PublicItem> items = user.UserItems?.Select(i => new PublicItem { Name = i.ItemName, Quantity = i.Quantity })
                                                        .ToList() ?? new List<PublicItem>();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user items");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Adds or updates the items to the authenticated user.
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

                var user = await _context.Users.Include(u => u.UserItems)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();
                if (user.UserItems == null) user.UserItems = new List<UserItem>();

                var userItemsMap = user.UserItems.ToDictionary(i => i.ItemName);

                foreach (PublicItem item in newItems)
                {
                    if (!userItemsMap.TryGetValue(item.Name, out var tmp))
                    {
                        UserItem newItem = new UserItem { ItemName = item.Name, Quantity = item.Quantity, UserId = user.Id };
                        user.UserItems.Add(newItem);
                        userItemsMap[item.Name] = newItem;
                    }
                    else tmp.Quantity = item.Quantity;
                }
                await _context.SaveChangesAsync();

                var updatedItems = user.UserItems.Select(i => new PublicItem { Name = i.ItemName, Quantity = i.Quantity }).ToList();

                return Ok(updatedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating user items");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes the items of the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>List<PublicItem></c> object if found and deleted correctly, or an error response otherwise.
        /// </returns>
        [HttpDelete("")]
        public async Task<ActionResult<List<PublicItem>>> Delete([FromBody] List<PublicItem> newItems)
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var user = await _context.Users.Include(u => u.UserItems)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();
                if (user.UserItems == null || !user.UserItems.Any()) return Ok(new List<PublicItem>());

                var itemsToRemove = user.UserItems.Where(i => newItems.Any(ni => ni.Name == i.ItemName)).ToList();

                if (itemsToRemove.Any())
                {
                    _context.UserItems.RemoveRange(itemsToRemove);
                    await _context.SaveChangesAsync();
                }

                var updatedItems = user.UserItems.Select(i => new PublicItem { Name = i.ItemName, Quantity = i.Quantity }).ToList();

                return Ok(updatedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user items");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
