using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    //Mário
    public class User : IdentityUser
    {
        [Display(Name = "Nome")]
        public required string Name { get; set; }

        public ICollection<UserLog>? Logs { get; set; }
    }
}
