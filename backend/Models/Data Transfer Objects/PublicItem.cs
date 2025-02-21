/// <summary>
/// This file defines the <c>PublicItem</c> class, which serves as a data transfer object (DTO) 
/// representing an item with a name and quantity.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais - 202001541</author>
/// <date>Last Modified: 2025-02-19</date>
namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// The <c>PublicItem</c> class represents an item with a name and quantity.
    /// It is used as a Data Transfer Object (DTO) to encapsulate item-related data.
    /// </summary>
    public class PublicItem
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item.
        /// </summary>
        public float Quantity { get; set; }
    }
}
