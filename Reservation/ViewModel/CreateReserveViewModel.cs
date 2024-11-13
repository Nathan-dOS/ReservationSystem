namespace Reservation.ViewModel
{
    public class CreateReserveViewModel
    {
        public int RoomId { get; set; }       // ID da sala
        public string UserId { get; set; }     // ID do usuário
        public DateTime ReserveDate { get; set; } = DateTime.Now;
        public TimeOnly ReserveStart { get; set; }
        public TimeOnly ReserveEnd { get; set; }
        public string ReserveStatus { get; set; }
        public float RentPrice { get; set; }
    }
}
