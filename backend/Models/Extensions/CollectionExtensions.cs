/// <summary>
/// This file defines the <c>CollectionExtensions</c> static class, which provides
/// extension methods for collections.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

namespace backend.Models.Extensions
{
    /// <summary>
    /// The <c>CollectionExtensions</c> static class provides helper methods to manipulate and validate collections.
    /// It includes utility functions that ensure collections adhere to predefined constraints, improving 
    /// data integrity and preventing invalid data from persisting.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Ensures that the elements in the given collection exist in a provided list of valid items.
        /// Any elements not found in <paramref name="allItems"/> are removed from <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection to be validated and fixed.</param>
        /// <param name="allItems">A list of valid items that should be retained in the collection.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if either <paramref name="collection"/> or <paramref name="allItems"/> is null.
        /// </exception>
        public static void FixCollection(this ICollection<string> collection, IEnumerable<string> allItems)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(allItems);

            HashSet<string> validItems = new(allItems);

            var itemsToRemove = collection.Where(e => !validItems.Contains(e)).ToList();
            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }
        }
    }
}
