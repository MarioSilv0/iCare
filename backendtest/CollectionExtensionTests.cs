/// <summary>
/// This file defines the <c>CollectionExtensionTests</c> class, which contains unit tests 
/// for the <c>FixCollection</c> extension method in <see cref="CollectionExtensions"/>.
/// These tests validate that the method correctly filters collections and handles invalid parameters.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Models.Extensions;

namespace backendtest
{
    /// <summary>
    /// The <c>CollectionExtensionTests</c> class contains unit tests for the <see cref="CollectionExtensions.FixCollection"/> method.
    /// It verifies correct handling of invalid parameters and ensures the method properly filters collections.
    /// </summary>
    public class CollectionExtensionTests
    {
        /// <summary>
        /// Tests that <c>FixCollection</c> throws an <see cref="ArgumentNullException"/> 
        /// when either the collection or the list of valid items is null.
        /// </summary>
        [Fact]
        public async Task FixCollection_WhenParametersAreInvalid_ThrowsArgumentNullException()
        {
            List<string>? allItems = ["Item1", "Item2"];

            ICollection<string>? collection = null;
            Assert.Throws<ArgumentNullException>(() => collection.FixCollection(allItems));

            allItems = null;
            collection = [];
            Assert.Throws<ArgumentNullException>(() => collection.FixCollection(allItems));
        }

        /// <summary>
        /// Tests that <c>FixCollection</c> correctly filters the collection, 
        /// removing elements not present in the valid items list.
        /// </summary>
        [Fact]
        public async Task FixCollection_WhenParametersAreValid_FixesCollectionCorrectly()
        {
            string item2 = "Item2", item3 = "Item3";
            List<string> allItems = ["Item1", item2, item3];

            ICollection<string> newItems = [item2, item3, "Item4"];
            newItems.FixCollection(allItems);

            Assert.Equal(2, newItems.Count);
            Assert.Contains(newItems, i => i == item2);
            Assert.Contains(newItems, i => i == item3);
        }
    }
}
