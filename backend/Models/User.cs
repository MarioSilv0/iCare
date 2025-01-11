using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // Mário 11/01/25
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public required string Name { get; set; }

        public ICollection<UserLog>? Logs { get; set; }
    }
}
