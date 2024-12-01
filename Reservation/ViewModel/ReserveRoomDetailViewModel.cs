using Reservation.Models;

namespace Reservation.ViewModel
{
    public class ReserveRoomDetailViewModel
    {
        public Room? Room { get; set; }
        public IEnumerable<Reserve>? Reservations { get; set; }
        public CreateReserveViewModel CreateReserveViewModel { get; set; }
        public DateOnly? SelectedDate { get; set; }
        public required List<EquipmentViewModel> RoomEquipments { get; set; }
    }
}
