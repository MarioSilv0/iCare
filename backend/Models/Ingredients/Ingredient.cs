using backend.Models.Preferences;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Ingredients
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Kcal {  get; set; }
        public float KJ { get; set; }
        public float Protein { get; set; }
        public float Carbohydrates { get; set; }
        public string? Category { get; set; }
        public ICollection<UserIngredient>? UserIngredients { get; set; }
        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
    }
}
