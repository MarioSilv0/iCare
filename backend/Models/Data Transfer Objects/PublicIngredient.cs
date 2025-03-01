using backend.Models.Ingredients;

namespace backend.Models.Data_Transfer_Objects
{
    public class PublicIngredient
    {
        public string? Name { get; set; }
        public float Kcal { get; set; }
        public float KJ { get; set; }
        public float Protein { get; set; }
        public float Carbohydrates { get; set; }
        public float Lipids { get; set; }
        public float Fibers { get; set; }
        public string? Category { get; set; }
    }
}
