using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    public class RoomImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public byte[] ImageData { get; set; }
        [ForeignKey("Room")]

        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
