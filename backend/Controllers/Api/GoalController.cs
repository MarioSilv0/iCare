using backend.Models.Goals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.Data;
using backend.Services;
namespace backend.Controllers.Api
{

    [Route("api/goal")]
    [ApiController]
    [Authorize]
    public class GoalController : ControllerBase
    {
        private readonly GoalService _goalService;

        public GoalController(GoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGoal()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var goal = await _goalService.GetCurrentGoalAsync(userId);
            if (goal == null) return NotFound("Nenhuma meta alimentar definida.");

            return Ok(goal);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] Goal goal)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            goal.UserId = userId;

            var success = await _goalService.ValidateAndSaveGoalAsync(goal,userId);
            if (!success) return BadRequest("Meta inválida ou utilizador já tem uma meta ativa.");

            return CreatedAtAction(nameof(GetUserGoal), new { userId }, goal);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] Goal goal)
        {
            if (id != goal.Id) return BadRequest("IDs não coincidem!");

            var success = await _goalService.UpdateGoalAsync(goal);
            if (!success) return NotFound("Meta não encontrada!");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var success = await _goalService.DeleteGoalAsync(id);
            if (!success) return NotFound("Meta não encontrada!");

            return NoContent();
        }
    }

}

//const goal = "/api/goal"
//
//getGoal() : Observable<any> {
//return this.http.get<any>(this.goal);
//}
//createGoal() : Observable<any> {
//return this.http.post<any>(this.goal);
//}
//updateGoal() : Observable<any> {
//return this.http.put<any>(``${this.goal}/${id});
//}
//deleteGoal() : Observable<any> {
//return this.http.delete<any>(``${this.goal}/${id});
//}