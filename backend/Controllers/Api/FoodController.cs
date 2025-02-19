using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly TacoApiService _tacoApiService;

        public FoodController(TacoApiService tacoApiService)
        {
            _tacoApiService = tacoApiService;
        }

        [HttpGet("all-ingredients")]
        public async Task<IActionResult> GetAllIngredients()
        {
            var query = @"
            {
                getAllFood {
                    name
                    nutrients {
                        kcal
                    }
                }
            }"; 

            var result = await _tacoApiService.GetFoodDataAsync(query);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFood([FromQuery] string term)
        {
            var query = @"
            {
                getFoodByName(name: """ + term + @""") {
                    name
                    nutrients {
                        kcal
                    }
                }
            }"; 

            var result = await _tacoApiService.GetFoodDataAsync(query);
            return Ok(result);
        }
    }
}
