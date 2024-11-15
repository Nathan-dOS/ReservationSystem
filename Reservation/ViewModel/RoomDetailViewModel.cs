using Reservation.Models;

namespace Reservation.ViewModel
{
    public class RoomDetailViewModel
    {
        public Room? Room { get; set; }
        public CreateReserveViewModel CreateReserveViewModel { get; set; }
    }
}
