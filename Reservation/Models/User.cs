using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    public class User
    {
        [Key]
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Type { get; set; }
        public required string CRM { get; set; }
        public required string OAB { get; set; }
        public required string CadastroNacional { get; set; }
        [ForeignKey("Address")]
        public string? AddressId { get; set; }

    }
}
