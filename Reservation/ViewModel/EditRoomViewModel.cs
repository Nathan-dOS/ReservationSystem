using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.ViewModel
{
    public class EditRoomViewModel
    {
        public int IdRoom { get; set; }

        public string Number { get; set; }

        public int Capacity { get; set; }

        public float Size { get; set; }

        public RoomStatus Status { get; set; }
        
        public float Price { get; set; }

        public string PhotoAlbum { get; set; }  // Caminho do álbum de fotos

        public bool Internet { get; set; }

        public bool SecurityCamera { get; set; }

        [Required]
        public bool AirConditioning { get; set; }

        [Required]
        public RoomType Type { get; set; }
    }
}

