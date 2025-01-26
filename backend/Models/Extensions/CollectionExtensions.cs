namespace backend.Models.Extensions
{
    public static class CollectionExtensions
    {
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
