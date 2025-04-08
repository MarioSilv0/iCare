using backend.Models.Goals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using backend.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// This file defines the <c>GoalController</c> class, responsible for managing user goals.
/// It provides API endpoints for creating, retrieving, updating, and deleting goals based on authentication tokens.
/// </summary>
/// <author>Mário Silva - 202000500</author>
/// <date>Last Modified: 2025-03-14</date>

namespace backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly ILogger<GoalController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>GoalController</c> class.
        /// </summary>
        /// <param name="goalService">An instance of <c>IGoalService</c> to handle goal operations.</param>
        /// <param name="logger">An instance of <c>ILogger<GoalController></c> to log errors and information.</param>
        public GoalController(IGoalService goalService, ILogger<GoalController> logger)
        {
            _goalService = goalService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the latest goal for the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>Ok</c> response containing the latest <c>GoalDTO</c> if found,
        /// <c>Ok(null)</c> if no goal exists, or an <c>Unauthorized</c> response
        /// if the user ID is not found in the token.
        /// </returns>
        /// <remarks>
        /// If an unexpected error occurs, a <c>500 Internal Server Error</c> is returned.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetCurrentGoalByUserId()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized("User ID not found in token.");
            try
            {
                var goal = await _goalService.GetLatestGoalByUserIdAsync(userId);

                if (goal == null)
                {
                    return Ok(null);
                }

                return Ok(new GoalDTO(goal));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao acessar a meta.");
                return StatusCode(500, "Erro ao acessar a meta.");
            }
        }

        /// <summary>
        /// Creates a new goal for the authenticated user.
        /// </summary>
        /// <param name="goalDto">The goal data transfer object containing goal details.</param>
        /// <returns>The created goal.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] GoalDTO goalDto)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized("User ID not found in token.");
            try
            {
                var createdGoal = await _goalService.CreateGoalAsync(userId, goalDto);
                return CreatedAtAction(nameof(GetCurrentGoalByUserId), new { id = createdGoal.Id }, createdGoal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar a meta.");
                return StatusCode(500, "Erro ao criar a meta: ");
            }
        }

        /// <summary>
        /// Updates an existing goal.
        /// </summary>
        /// <param name="goalDto">The updated goal data.</param>
        /// <returns>No content if successful; <c>NotFound</c> if the goal does not exist.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateGoal([FromBody] GoalDTO goalDto)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized("User ID not found in token.");
            try
            {
                var success = await _goalService.UpdateGoalAsync(userId, goalDto);
                if (!success)
                {
                    return NotFound("Meta não encontrada.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a meta.");
                return StatusCode(500, "Erro ao atualizar a meta.");
            }
        }

        /// <summary>
        /// Deletes the current goal.
        /// </summary>
        /// <returns>No content if successful; <c>NotFound</c> if the goal does not exist.</returns>+
        [HttpDelete]
        public async Task<IActionResult> DeleteGoal()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return Unauthorized("User ID not found in token.");
            try
            {
                var success = await _goalService.DeleteGoalAsync(userId);
                if (!success)
                {
                    return NotFound("Meta não encontrada.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar a meta.");
                return StatusCode(500, "Erro ao deletar a meta.");
            }
        }
    }
}
