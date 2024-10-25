using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation.Models
{
    public class Room
    {
        [Key]
        public int IdRoom { get; set; }

        [MaxLength(10)]
        [Required]
        public string Number { get; set; }

        public int Capacity { get; set; }

        public float Size { get; set; }

        [Required]
        public RoomStatus Status { get; set; }
        [Required]
        public float Price { get; set; }

        [MaxLength(255)]
        public string PhotoAlbum { get; set; }  // Caminho do álbum de fotos

        [Required]
        public bool Internet { get; set; }

        [Required]
        public bool SecurityCamera { get; set; }

        [Required]
        public bool AirConditioning { get; set; }

        [Required]
        public RoomType Type { get; set; }
    }
}
