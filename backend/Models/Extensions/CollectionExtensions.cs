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
