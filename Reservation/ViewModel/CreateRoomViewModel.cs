using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.ViewModel
{
    public class CreateRoomViewModel
    {
        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        public int Capacity { get; set; }

        public float SizeInSquareMeters { get; set; }

        public EnumRoomStatus RoomStatus { get; set; }

        public float RoomPrice { get; set; }
        public List<IFormFile> RoomImages { get; set; } = new List<IFormFile>(); // IFormFile para fazer upload de novas imagens

        public bool HasInternet { get; set; }

        public bool HasSecurityCamera { get; set; }

        public bool HasAirConditioning { get; set; }

        public EnumRoomType RoomType { get; set; }
    }
}
