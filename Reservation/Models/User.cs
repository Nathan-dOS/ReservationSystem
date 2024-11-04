using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    [Index(nameof(CPF), nameof(CRMNumber), nameof(OABNumber), IsUnique = true)]
    public class User : IdentityUser
    {
        [Key]
        [Required]
        public int UserId { get; set; }

        [StringLength(11)]
        public string CPF { get; set; }

        [Required]
        public EnumUserType UserType { get; set; }

        [StringLength(10)]
        public string? CRMNumber { get; set; } // Opcional, controle externo de formato

        [StringLength(10)]
        public string? OABNumber { get; set; } // Opcional, controle externo de formato

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(60)]
        public string Password { get; set; } // Armazena hash da senha

        [Required]
        [StringLength(100)]
        public string Address { get; set; }
    }
}
