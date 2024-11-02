using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [MaxLength(10)]
        [Required]
        public string RoomNumber { get; set; }

        public int Capacity { get; set; }

        public float SizeInSquareMeters { get; set; }

        [Required]
        public EnumRoomStatus RoomStatus { get; set; }
        [Required]
        public float RoomPrice { get; set; }

        public List<RoomImage> PhotoAlbum { get; set; } = new List<RoomImage>();

        [Required]
        public bool HasInternet { get; set; }

        [Required]
        public bool HasSecurityCamera { get; set; }

        [Required]
        public bool HasAirConditioning { get; set; }

        [Required]
        public EnumRoomType RoomType { get; set; }
    }

    public class RoomImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public byte[] ImageData { get; set; }  

        public int RoomId { get; set; }  
        public Room Room { get; set; }
    }
}
