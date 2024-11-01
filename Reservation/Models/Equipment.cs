using System.ComponentModel.DataAnnotations.Schema;
using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EquipmentName { get; set; }

        [Required]
        public EnumEquipmentStatus EquipmentStatus { get; set; }

        [Required]
        public float EquipmentPrice { get; set; }

        [Required]
        public int QuantityAvailable { get; set; }

    }
}
