using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    // Mário 11/01/25
    public class UserLog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O utilizador é obrigatório.")]
        [Display(Name = "Utilizador")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [Display(Name = "Mensagem")]
        public required string Message { get; set; }

        [Required(ErrorMessage = "A data/hora é obrigatória.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data e Hora")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Nome do Utilizador")]
        public User? User { get; set; }
    }
}
