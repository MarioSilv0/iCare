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
    /// The <c>CollectionExtensions</c> static class provides extension methods for working with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Ensures that the elements of the given collection exist in a provided list of valid items.
        /// Any elements not found in <paramref name="allItems"/> are removed from <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection to be validated and fixed.</param>
        /// <param name="allItems">A list of valid items that should be retained in the collection.</param>
        public static void FixCollection(this ICollection<string> collection, List<string> allItems)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(allItems);
            
            HashSet<string> c = new(allItems);
            foreach (var e in collection)
            {
                if(c.Contains(e)) continue;

                collection.Remove(e);
            }
        }
    }
}
