using Reservation.Models;

namespace Reservation.ViewModel
{
    public class RoomDetailViewModel
    {
        public Room? Room { get; set; }
        public IEnumerable<Reserve>? Reservations { get; set; }
        public CreateReserveViewModel CreateReserveViewModel { get; set; }
        public DateOnly? SelectedDate { get; set; }
        public required List<EquipmentViewModel> RoomEquipments { get; set; }
        public IEnumerable<Equipment>? Equipments { get; set; }

    }
}
