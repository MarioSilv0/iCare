using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
