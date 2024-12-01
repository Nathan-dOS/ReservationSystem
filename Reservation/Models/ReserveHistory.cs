using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation.Models
{
    public class ReserveHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        [ForeignKey("Reserve")] // Relaciona ao ID de reserva original
        public int ReserveId { get; set; }

        [Required]
        [ForeignKey("User")] // Relaciona ao usuário que fez a reserva
        public string UserId { get; set; }

        [Required]
        [ForeignKey("Room")] // Relaciona à sala reservada
        public int RoomId { get; set; }

        [Required(ErrorMessage = "A data da reserva é obrigatória.")]
        public DateOnly ReserveDate { get; set; }

        [Required(ErrorMessage = "O horário de início é obrigatório.")]
        public TimeOnly ReserveStart { get; set; }

        [Required(ErrorMessage = "O horário de término é obrigatório.")]
        public TimeOnly ReserveEnd { get; set; }

        [Required(ErrorMessage = "O status da reserva é obrigatório.")]
        public EnumReserveStatus ReserveStatus { get; set; }

        public required float RentPrice { get; set; }

        public Room  Room { get; set; }

        [Required]
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
