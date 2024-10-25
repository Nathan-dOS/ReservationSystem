using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation.Models
{
    public class Reserve
    {
        [Key]
        public required int Id { get; set; }
        [ForeignKey("User")]
        public required string UserId { get; set; }
        [ForeignKey("Room")]
        public required string RoomId { get; set; }
        [ForeignKey("Equipment")]
        public required string EquipementId { get; set; }
        public required DateTime Date { get; set; }
        public required TimeOnly Start { get; set; }
        public required TimeOnly End { get; set; }
        public required string Status { get; set; }
        public required float RentPrice { get; set; }
        public required string Photos { get; set; }
    }
}
