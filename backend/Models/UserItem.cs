/// <summary>
/// This file defines the <c>UserItem</c> class, which represents an item 
/// associated with a user, including its name, quantity, and user reference.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais - 202001541</author>
/// <date>Last Modified: 2025-02-19</date>
namespace backend.Models
{
    /// <summary>
    /// The <c>UserItem</c> class represents an item that belongs to a user, 
    /// including the item's name, quantity, and the associated user.
    /// </summary>
    public class UserItem
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item.
        /// </summary>
        public float Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user associated with this item.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this item.
        /// </summary>
        public User User { get; set; }
    }
}
