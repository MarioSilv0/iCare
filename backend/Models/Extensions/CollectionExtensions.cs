/// <summary>
/// This file defines the <c>CollectionExtensions</c> static class, which provides
/// extension methods for collections.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Models.Extensions
{
    /// <summary>
    /// Static class <c>CollectionExtensions</c> contains extension methods for working with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Updates a collection based on a provided list of <c>SelectionObject</c> items.
        /// The method clears the collection and adds items where <c>IsSelected</c> is true,
        /// using a provided delegate to create elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to update.</param>
        /// <param name="list">The list of <c>SelectionObject</c> items.</param>
        /// <param name="createElement">
        /// A function that creates an element of type <c>T</c> from a <c>SelectionObject</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="collection"/>, <paramref name="list"/>, or <paramref name="createElement"/> is null.
        /// </exception>
        public static void UpdateCollection<T>(this ICollection<T> collection, List<SelectionObject> list, Func<SelectionObject, T> createElement)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(createElement);

            collection.Clear();
            foreach (var e in list)
            {
                if (!e.IsSelected) continue;
                collection.Add(createElement(e));
            }
        }
    }
}
