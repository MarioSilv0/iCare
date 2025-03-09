/// <summary>
/// This file defines the <c>ItemDTO</c> class, which serves as a data transfer object (DTO) 
/// representing an item with a name, quantity, and unit. It is primarily used for 
/// transferring ingredient-related data between the backend and frontend.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

using backend.Models.Ingredients;

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
        /// Gets or sets the unit of measurement for the item's quantity (e.g., "kg", "g").
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Default constructor for the <c>ItemDTO</c> class.
        /// </summary>
        public ItemDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>ItemDTO</c> class using a <c>UserIngredient</c>.
        /// This constructor extracts relevant properties from the <c>UserIngredient</c> object.
        /// </summary>
        /// <param name="ui">The <c>UserIngredient</c> instance containing the item's data.</param>
        public ItemDTO(UserIngredient ui)
        {
            Name = ui.Ingredient.Name;
            Quantity = ui.Quantity;
            Unit = ui.Unit;
        }

    }
}
