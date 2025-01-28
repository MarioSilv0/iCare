using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.Extensions;
using backend.Models.Preferences;
using Moq;

namespace backendtest
{
    public class CollectionExtensionTests
    {
        [Fact]
        public void UpdateCollection_WhenParametersAreInvalid_ThrowsArgumentNullException() 
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
        public void UpdateCollection_WhenParametersAreValid_UpdatesCollectionCorrectly()
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
