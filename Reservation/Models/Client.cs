using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    public class Client
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [StringLength(11)]
        public string CPF { get; set; }

        [Required]
        public PersonType Type { get; set; }

        [StringLength(10)]
        public string? CRM { get; set; } // Opcional, controle externo de formato

        [StringLength(10)]
        public string? OAB { get; set; } // Opcional, controle externo de formato

        [Required]
        [StringLength(60)]
        public string Nome { get; set; }

        [Required]
        [StringLength(15)]
        public string Telefone { get; set; }

        [Required]
        [StringLength(60)]
        public string Password { get; set; } // Armazena hash da senha

        [Required]
        [StringLength(100)]
        public string Address { get; set; }
    }
}
