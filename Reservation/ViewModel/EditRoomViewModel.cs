using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.ViewModel
{
    public class EditRoomViewModel
    {
        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        public int Capacity { get; set; }

        public float SizeInSquareMeters { get; set; }

        public EnumRoomStatus RoomStatus { get; set; }

        public float RoomPrice { get; set; }
        public List<int> DeleteImages { get; set; } = new List<int>();// lista de imagens que serao deletadas

        public List<IFormFile> RoomImages { get; set; } = new List<IFormFile>(); // IFormFile para fazer upload de novas imagens
        public List<byte[]> ExistingImages { get; set; } = new List<byte[]>();// lista do tipo byte com as fotos ja armazenadas no banco de dados

        public bool HasInternet { get; set; }

        public bool HasSecurityCamera { get; set; }

        public bool HasAirConditioning { get; set; }

        public EnumRoomType RoomType { get; set; }
    }
}
