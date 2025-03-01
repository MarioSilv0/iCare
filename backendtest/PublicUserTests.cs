//using backend.Models;
//using backend.Models.Preferences;
//using backend.Models.Restrictions;

///// <summary>
///// This file contains unit tests for the <c>PublicUser</c> class, which is responsible for representing a user's public profile with preferences and restrictions.
///// The tests verify the initialization of <c>PublicUser</c> objects, including both default and populated scenarios with or without models for user data.
///// The tests ensure that the <c>PublicUser</c> class correctly maps user properties, preferences, and restrictions and that it behaves as expected when populated with different data sources.
///// </summary>
///// <author>Luís Martins - 202100239</author>
///// <author>João Morais - 202001541</author>
///// <date>Last Modified: 2025-01-30</date>

//namespace backendtest
//{
//    /// <summary>
//    /// Test class for the <c>PublicUser</c> class.
//    /// This class contains tests that validate the behavior of <c>PublicUser</c> when instantiated with default values or populated with user data and preference/restriction models.
//    /// It checks if the <c>PublicUser</c> object initializes correctly and if the mappings between user preferences and restrictions are correct.
//    /// </summary>
//    public class PublicUserTests
//    {
//        [Fact]
//        public async Task PublicUser_DefaultConstructor_InitializesWithDefaults()
//        {
//            var publicUser = new PublicUser();

//            Assert.NotNull(publicUser.Preferences);
//            Assert.NotNull(publicUser.Restrictions);
//            Assert.Empty(publicUser.Preferences);
//            Assert.Empty(publicUser.Restrictions);
//            Assert.Null(publicUser.Picture);
//            Assert.Null(publicUser.Name);
//            Assert.Null(publicUser.Email);
//            Assert.Equal(default, publicUser.Birthdate);
//            Assert.False(publicUser.Notifications);
//            Assert.Equal(0, publicUser.Height);
//            Assert.Equal(0, publicUser.Weight);
//        }

//        [Fact]
//        public async Task PublicUser_PopulatesWithoutModel()
//        {
//            User user = new()
//            {
//                Id = "1",
//                Picture = "picture_url",
//                Name = "User",
//                Email = "user@something.com",
//                Birthdate = new DateOnly(1990, 1, 1),
//                Notifications = true,
//                Height = 1.80f,
//                Weight = 75f,
//                UserPreferences = [new() { UserId = "1", PreferenceId = 1 }],
//                UserRestrictions = [new() { UserId = "1", RestrictionId = 2 }]
//            };
//            var listPreferences = new List<Preference>
//            {
//                new() { Id = 1, Name = "P1" },
//                new() { Id = 2, Name = "P2" }
//            };
//            var listRestrictions = new List<Restriction>
//            {
//                new() { Id = 1, Name = "R1" },
//                new() { Id = 2, Name = "R2" }
//            };

//            PublicUser publicUser = new (user, null, listPreferences, listRestrictions);

//            Assert.Equal(user.Picture, publicUser.Picture);
//            Assert.Equal(user.Name, publicUser.Name);
//            Assert.Equal(user.Email, publicUser.Email);
//            Assert.Equal(user.Birthdate, publicUser.Birthdate);
//            Assert.Equal(user.Notifications, publicUser.Notifications);
//            Assert.Equal(user.Height, publicUser.Height);
//            Assert.Equal(user.Weight, publicUser.Weight);

//            Assert.Equal(2, publicUser.Preferences.Count);
//            Assert.Equal(1, publicUser.Preferences[0].Id);
//            Assert.Equal(2, publicUser.Preferences[1].Id);
//            Assert.True(publicUser.Preferences[0].IsSelected);
//            Assert.False(publicUser.Preferences[1].IsSelected);

//            Assert.Equal(2, publicUser.Restrictions.Count);
//            Assert.Equal(1, publicUser.Restrictions[0].Id);
//            Assert.Equal(2, publicUser.Restrictions[1].Id);
//            Assert.False(publicUser.Restrictions[0].IsSelected);
//            Assert.True(publicUser.Restrictions[1].IsSelected);
//        }

//        public async Task PublicUser_PopulatesWitModel()
//        {
//            User user = new()
//            {
//                Id = "1",
//                Picture = "picture_url",
//                Name = "User",
//                Email = "user@something.com",
//                Birthdate = new DateOnly(1990, 1, 1),
//                Notifications = true,
//                Height = 1.80f,
//                Weight = 75f,
//                UserPreferences = [new() { UserId = "1", PreferenceId = 1 }],
//                UserRestrictions = [new() { UserId = "1", RestrictionId = 2 }]
//            };

//            PublicUser publicUser = new PublicUser()
//            {
//                Preferences = [new() { Id = 1, Name = "P1", IsSelected = true}, new() { Id = 2, Name = "P2", IsSelected = false}],
//                Restrictions = [new() { Id = 1, Name = "R1", IsSelected = false }, new() { Id = 2, Name = "R2", IsSelected = true }]
//            };

//            PublicUser result = new(user, publicUser, [], []);

//            Assert.Equal(user.Picture, result.Picture);
//            Assert.Equal(user.Name, result.Name);
//            Assert.Equal(user.Email, result.Email);
//            Assert.Equal(user.Birthdate, result.Birthdate);
//            Assert.Equal(user.Notifications, result.Notifications);
//            Assert.Equal(user.Height, result.Height);
//            Assert.Equal(user.Weight, result.Weight);

//            Assert.Equal(publicUser.Preferences.Count, result.Preferences.Count);
//            Assert.Equal(publicUser.Preferences[0].Id, result.Preferences[0].Id);
//            Assert.Equal(publicUser.Preferences[1].Id, result.Preferences[1].Id);
//            Assert.Equal(publicUser.Preferences[0].IsSelected, result.Preferences[0].IsSelected);
//            Assert.Equal(publicUser.Preferences[1].IsSelected, result.Preferences[1].IsSelected);

//            Assert.Equal(publicUser.Restrictions.Count, result.Restrictions.Count);
//            Assert.Equal(publicUser.Restrictions[0].Id, result.Restrictions[0].Id);
//            Assert.Equal(publicUser.Restrictions[1].Id, result.Restrictions[1].Id);
//            Assert.Equal(publicUser.Restrictions[0].IsSelected, result.Restrictions[0].IsSelected);
//            Assert.Equal(publicUser.Restrictions[1].IsSelected, result.Restrictions[1].IsSelected);
//        }
//    }
//}
