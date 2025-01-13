using System.ComponentModel.DataAnnotations;
using System.Net;

namespace backend.Models
{
    public class UserLog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O utilizador é obrigatório.")]
        [Display(Name = "Utilizador")]
        public string UserId { get; set; }

        [Display(Name = "Mensagem")]
        public required string Message { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data e Hora")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Endereço de IP")]
        public string? IpAddress { get; set; }

        [Display(Name = "Utilizador")]
        public User? User { get; set; }
    }
}
