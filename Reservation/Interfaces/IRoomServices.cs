using Reservation.Data.Enum;
using Reservation.ViewModel;

namespace Reservation.Interfaces
{
    public interface IRoomServices
    {
        public List<EquipmentViewModel> GetRoomEquipment(EnumRoomType roomType);
    }
}
