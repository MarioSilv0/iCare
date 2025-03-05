/// <summary>
/// This file contains the <c>UserTests</c> class, which provides unit tests 
/// for the <see cref="User"/> class. These tests ensure that user properties 
/// are updated correctly while preserving validation constraints.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Models;

namespace backendtest
{
    /// <summary>
    /// The <c>UserTests</c> class contains unit tests for the <see cref="User"/> model.
    /// These tests validate that the <c>UpdateFromModel()</c> method updates user properties 
    /// correctly while maintaining data integrity.
    /// </summary>
    public class UserTests
    {
        private readonly User user;
        private readonly UserDTO pu;

        /// <summary>
        /// Initializes a new instance of the <c>UserTests</c> class.
        /// Creates instances of <see cref="User"/> and <see cref="UserDTO"/> 
        /// for testing property updates.
        /// </summary>
        public UserTests()
        {
            user = new User
            {
                Name = "User",
                Picture = "User Picutre",
                Email = "user@email.com",
                Birthdate = DateOnly.FromDateTime(DateTime.Today),
                Notifications = false,
                Height = 0.50f,
                Weight = 3.5f,
                Preferences = new List<string>(),
                Restrictions = new List<string>()
            };

            pu = new UserDTO
            {
                Name = "Public User",
                Picture = "Public User Picture",
                Email = "public_user@email.com",
                Birthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                Notifications = true,
                Height = 1.50f,
                Weight = 50.0f,
                Preferences = new List<string>() { "Preference" },
                Restrictions = new List<string>() { "Restriction" }
            };
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> correctly updates all user properties 
        /// when valid data is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_WhenDataIsValid_UpdatesUser()
        {
            user.UpdateFromModel(pu);

            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Email, user.Email);
            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Equal(pu.Weight, user.Weight);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> does not update the picture 
        /// when an invalid value (empty, null, or whitespace) is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_InvalidPicture_PictureRemainsTheSame()
        {
            string? expected = user.Picture;

            pu.Picture = "";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Picture, user.Picture);
            Assert.Equal(expected, user.Picture);

            pu.Picture = null;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Picture, user.Picture);
            Assert.Equal(expected, user.Picture);

            pu.Picture = "      ";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Picture, user.Picture);
            Assert.Equal(expected, user.Picture);

            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Email, user.Email);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Equal(pu.Weight, user.Weight);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> does not update the name 
        /// when an invalid value (empty, null, or whitespace) is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_InvalidName_NameRemainsTheSame()
        {
            string? expected = user.Name;

            pu.Name = "";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Name, user.Name);
            Assert.Equal(expected, user.Name);

            pu.Name = null;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Name, user.Name);
            Assert.Equal(expected, user.Name);

            pu.Name = "      ";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Name, user.Name);
            Assert.Equal(expected, user.Name);

            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Email, user.Email);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Equal(pu.Weight, user.Weight);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> does not update the email 
        /// when an invalid value (empty, null, or improperly formatted) is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_InvalidEmail_EmailRemainsTheSame()
        {
            string? expected = user.Email;

            pu.Email = "";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Email, user.Email);
            Assert.Equal(expected, user.Email);

            pu.Email = null;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Email, user.Email);
            Assert.Equal(expected, user.Email);

            pu.Email = "      ";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Email, user.Email);
            Assert.Equal(expected, user.Email);

            pu.Email = "invalid email@mail.com";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Email, user.Email);
            Assert.Equal(expected, user.Email);

            pu.Email = "invalidEmailmail.com";
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Email, user.Email);
            Assert.Equal(expected, user.Email);

            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Equal(pu.Weight, user.Weight);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> does not update the birthdate 
        /// when an invalid value (too old or future date) is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_InvalidBirthdate_BirthdateRemainsTheSame()
        {
            DateOnly expected = user.Birthdate;

            pu.Birthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(5));
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Birthdate, user.Birthdate);
            Assert.Equal(expected, user.Birthdate);

            pu.Birthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-121));
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Birthdate, user.Birthdate);
            Assert.Equal(expected, user.Birthdate);

            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Email, user.Email);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Equal(pu.Weight, user.Weight);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> does not update the height 
        /// when an invalid value (negative or too high) is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_InvalidHeight_HeightRemainsTheSame()
        {
            float? expected = user.Height;

            pu.Height = 3.5f;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Height, user.Height);
            Assert.Equal(expected, user.Height);

            pu.Height = -3.5f;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Height, user.Height);
            Assert.Equal(expected, user.Height);

            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Email, user.Email);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Weight, user.Weight);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>UpdateFromModel()</c> does not update the weight 
        /// when an invalid value (negative or too high) is provided.
        /// </summary>
        [Fact]
        public async Task UpdateFromModel_InvalidWeight_WeightRemainsTheSame()
        {
            float? expected = user.Weight;

            pu.Weight = 705f;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Weight, user.Weight);
            Assert.Equal(expected, user.Weight);

            pu.Weight = -3.5f;
            user.UpdateFromModel(pu);
            Assert.NotEqual(pu.Weight, user.Weight);
            Assert.Equal(expected, user.Weight);

            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Email, user.Email);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Single(user.Preferences);
            Assert.Single(user.Restrictions);
            Assert.Contains(user.Preferences, n => n == pu.Preferences.First());
            Assert.Contains(user.Restrictions, n => n == pu.Restrictions.First());
        }
    }
}
