using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation.Models
{
    public class Reserve
    {
        [Key]
        public int ReserveId { get; set; }
        [ForeignKey("User")]
        public required string UserId { get; set; }
        [ForeignKey("Room")]
        public required int RoomId { get; set; }
        [ForeignKey("Equipment")]
        public int? EquipementId { get; set; }
        public required DateOnly ReserveDate { get; set; }
        public required TimeOnly ReserveStart { get; set; }
        public required TimeOnly ReserveEnd { get; set; }
        public required EnumReserveStatus ReserveStatus { get; set; }
        public required float RentPrice { get; set; }
        public Room Room { get; set; } // Relação de 1 para muitos com a tabela de salas
    }
}
