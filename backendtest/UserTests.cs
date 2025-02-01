using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backendtest
{
    public class UserTests
    {
        private readonly User user;
        private readonly PublicUser pu;
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
            };

            pu = new PublicUser
            {
                Name = "Public User",
                Picture = "Public User Picture",
                Email = "public_user@email.com",
                Birthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                Notifications = true,
                Height = 1.50f,
                Weight = 50.0f
            };
        }

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
        }

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
        }

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
        }

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

            Assert.Equal(pu.Picture, user.Picture);
            Assert.Equal(pu.Name, user.Name);
            Assert.Equal(pu.Birthdate, user.Birthdate);
            Assert.True(user.Notifications);
            Assert.Equal(pu.Height, user.Height);
            Assert.Equal(pu.Weight, user.Weight);
        }

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
        }

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
        }

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
        }
    }
}
