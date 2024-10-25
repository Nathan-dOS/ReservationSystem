using System.ComponentModel.DataAnnotations.Schema;
using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    public class Equipment
    {
        [Key]
        public int IdEquipment { get; set; }

        [Required]
        [MaxLength(100)]
        public string EquipmentName { get; set; }

        [Required]
        public EquipmentStatus Status { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int QuantityAvailable { get; set; }

    }
}
