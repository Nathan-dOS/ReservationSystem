using System.ComponentModel.DataAnnotations;

namespace Reservation.ViewModel
{
    public class CreateEquipmentViewModel
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EquipmentName { get; set; }

        [Required]
        public float EquipmentPrice { get; set; }

        [Required]
        public int QuantityAvailable { get; set; }
    }
}
