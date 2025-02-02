using backend.Models;
using backend.Models.Extensions;
using backend.Models.Preferences;

/// <summary>
/// This file contains unit tests for the <c>CollectionExtensions</c> class.
/// These tests verify the functionality of the <c>UpdateCollection</c> extension method for the <c>ICollection</c> interface.
/// Specifically, it tests how the method handles various parameter scenarios, including invalid inputs and successful collection updates.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-30</date>


namespace backendtest
{
    /// <summary>
    /// Test class for the <c>CollectionExtensions</c> class.
    /// This class includes tests for the <c>UpdateCollection</c> method, covering edge cases and validation of parameters.
    /// </summary>
    public class CollectionExtensionTests
    {
        [Fact]
        public async Task UpdateCollection_WhenParametersAreInvalid_ThrowsArgumentNullException() 
        {
            ICollection<UserPreference>? preferences = null;
            List<SelectionObject>? selectionObjects = [ new() { Id = 1, IsSelected = true } ];
            Func<SelectionObject, UserPreference>? func = e => new UserPreference { UserId = "123", PreferenceId = e.Id };

            Assert.Throws<ArgumentNullException>(() => preferences.UpdateCollection(selectionObjects, func));

            preferences = [];
            selectionObjects = null;
            Assert.Throws<ArgumentNullException>(() => preferences.UpdateCollection(selectionObjects, func));

            selectionObjects = [new() { Id = 1, IsSelected = true }];
            func = null;
            Assert.Throws<ArgumentNullException>(() => preferences.UpdateCollection(selectionObjects, func));
        }

        [Fact]
        public async Task UpdateCollection_WhenParametersAreValid_UpdatesCollectionCorrectly()
        {
            ICollection<UserPreference> preferences = [];
            List<SelectionObject> selectionObjects = [new() { Id = 1, IsSelected = true }, new() { Id = 2, IsSelected = false }, new() { Id = 3, IsSelected = true }];
            static UserPreference func(SelectionObject e) => new UserPreference { UserId = "123", PreferenceId = e.Id };

            preferences.UpdateCollection(selectionObjects, func);
            
            Assert.Equal(2, preferences.Count);
            Assert.Contains(preferences, p => p.PreferenceId == 1);
            Assert.Contains(preferences, p => p.PreferenceId == 3);
        }
    }
}
