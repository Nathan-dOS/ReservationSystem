using System.ComponentModel.DataAnnotations;
using Reservation.Data.Enum;

namespace Reservation.ViewModel
{
    public class CreateReserveViewModel
    {
        [Required(ErrorMessage = "O campo RoomId é obrigatório.")]
        public int RoomId { get; set; } // ID da sala

        [Required(ErrorMessage = "O campo UserId é obrigatório.")]
        public string UserId { get; set; } // ID do usuário

        [Required(ErrorMessage = "O campo ReserveDate é obrigatório.")]
        public DateOnly ReserveDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required(ErrorMessage = "O campo ReserveStart é obrigatório.")]
        public TimeOnly ReserveStart { get; set; }

        [Required(ErrorMessage = "O campo ReserveEnd é obrigatório.")]
        public TimeOnly ReserveEnd { get; set; }

        [Required(ErrorMessage = "O campo ReserveStatus é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo ReserveStatus pode ter no máximo 50 caracteres.")]
        public string ReserveStatus { get; set; } = "Pendente"; // Status da reserva

        [Required(ErrorMessage = "O campo RentPrice é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public float RentPrice { get; set; }
    }
}
