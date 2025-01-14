using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Name is mandatory.")]
        [Display(Name = "Name")]
        public required string Name { get; set; }

        [Display(Name = "Birthdate")]
        public DateOnly Birthdate { get; set; }

        [Display(Name = "Height")]
        public float Height { get; set; }

        [Display(Name = "Weight")]
        public float Weight { get; set; }

        [Display(Name = "Preferences")]
        public List<Preferences> Preferences { get; set; } = [];

        [Display(Name = "Restrictions")]
        public List<Restrictions> Restrictions { get; set; } = [];

        public ICollection<UserLog>? Logs { get; set; }
    }
}
