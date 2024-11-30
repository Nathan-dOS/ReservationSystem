using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    [Index(nameof(CPF), nameof(CRMNumber), nameof(OABNumber), nameof(Email), IsUnique = true)] // Essas tabelas nao podem repetir o mesmo dado
    public class User : IdentityUser
    {
        [StringLength(11)]
        [Required]
        public string CPF { get; set; }
        [Required]
        public override string Email {get; set; }

        [StringLength(10)]
        public string? CRMNumber { get; set; } // Opcional, controle externo de formato

        [StringLength(10)]
        public string? OABNumber { get; set; } // Opcional, controle externo de formato

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public override string PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        public string? AssignedBuilding { get; set; }
        public bool IsBanned { get; set; } = false;
        public DateTime? BannedUntil { get; set; }

        public DateOnly? EntryDate { get; set; }

    }
}
