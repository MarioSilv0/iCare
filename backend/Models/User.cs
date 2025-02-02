using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace backend.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Name is mandatory.")]
        [Display(Name = "Name")]
        [PersonalData]
        public string Name { get; set; }

        [Display(Name = "Picture")]
        public string? Picture { get; set; }

        [Display(Name = "Birthdate")]
        public DateOnly Birthdate { get; set; }

        [Display(Name = "Notifications")]
        public bool Notifications { get; set; }

        [Display(Name = "Height")]
        public float Height { get; set; }

        [Display(Name = "Weight")]
        public float Weight { get; set; }

        [Display(Name = "Preferences")]
        public ICollection<UserPreference>? UserPreferences { get; set; }

        [Display(Name = "Restrictions")]
        public ICollection<UserRestriction>? UserRestrictions { get; set; }

        public ICollection<UserLog>? Logs { get; set; }

        public void UpdateFromModel(PublicUser model)
        {
            if(Picture != model.Picture && !string.IsNullOrWhiteSpace(model.Picture))
                Picture = model.Picture;

            if(Name != model.Name && !string.IsNullOrWhiteSpace(model.Name))
                Name = model.Name;

            if(Email != model.Email && !string.IsNullOrWhiteSpace(model.Email))
                Email = model.Email;

            if (Birthdate != model.Birthdate)
            {
                int age = this.CaculateAge(model.Birthdate);
                if (age > 0 && age <= 120)
                    Birthdate = model.Birthdate;
            }

            Notifications = model.Notifications;

            float roundedHeight = (float) Math.Round(model.Height, 1);
            if (Height != roundedHeight && roundedHeight > 0 && roundedHeight < 3)
                Height = roundedHeight;

            float roundedWeight = (float) Math.Round(model.Weight, 1);
            if (Weight != roundedWeight && roundedWeight > 0 && roundedWeight < 700)
                Weight = roundedWeight;
        }

        private int CaculateAge(DateOnly birthdate)
        {
            int age = DateTime.Today.Year - birthdate.Year;

            // Adjust if birthday hasn't occurred yet
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (birthdate > today.AddYears(-age))
                age -= 1;

            return age;
        }
    }
}


