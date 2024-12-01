using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Reservation.ViewModel;

namespace Reservation.Models
{
    public class ReserveEquipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Reserve")]
        public int ReserveId { get; set; }

        [Required]
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }

        [Required]
        public int Quantity { get; set; } // Quantidade de equipamentos reservados

        // Navegação
        public Reserve Reserve { get; set; }
        public Equipment Equipment { get; set; }
    }

}
