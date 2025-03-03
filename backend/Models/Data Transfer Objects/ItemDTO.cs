
using backend.Models.Ingredients;

/// <summary>
/// This file defines the <c>PublicItem</c> class, which serves as a data transfer object (DTO) 
/// representing an item with a name, quantity, and unit.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-03</date>
namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// The <c>PublicItem</c> class represents an item with a name, quantity, and unit.
    /// It is used as a Data Transfer Object (DTO) to encapsulate item-related data for efficient data transfer.
    /// </summary>
    public class ItemDTO
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Backing field for the <c>Quantity</c> property to enforce constraints.
        /// </summary>
        private float _quantity;

        /// <summary>
        /// Gets or sets the quantity of the item.
        /// Ensures the quantity is non-negative and rounds it to two decimal places.
        /// </summary>
        public float Quantity
        {
            get => _quantity;
            set => _quantity = (value < 0) ? 0 : (float)Math.Round(value, 2);
        }

        /// <summary>
        /// Gets or sets the unit of measurement for the item's quantity (e.g., "kg", "g", "ml").
        /// </summary>
        public string? Unit { get; set; }

        public ItemDTO() { }

        public ItemDTO(UserIngredient ui)
        {
            Name = ui.Ingredient.Name;
            Quantity = ui.Quantity;
            Unit = ui.Unit;
        }

        public ItemDTO(RecipeIngredient ui)
        {
            Name = ui.Ingredient.Name;
            Quantity = ui.Quantity;
            Unit = ui.Unit;
        }
    }
}
