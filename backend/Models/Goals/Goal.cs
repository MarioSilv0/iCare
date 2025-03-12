using System.ComponentModel.DataAnnotations;

namespace backend.Models.Goals
{
    public class Goal
    {
        [Key]
        public int Id { get; set; }

        public required string UserId { get; set; } // Para associar ao utilizador autenticado

        public required string Type { get; set; } // "Automática" ou "Manual"

        public string? GoalType { get; set; } // "Perder Peso", "Manter Peso", "Ganhar Peso" (para metas automáticas)

        public int? Calories { get; set; } // Apenas para metas manuais

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}

