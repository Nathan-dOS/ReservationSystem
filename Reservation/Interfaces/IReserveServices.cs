using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Interfaces
{
    public interface IReserveService
    {
        bool IsValidBusinessHours(TimeOnly start, TimeOnly end);
        bool IsValidReserveTime(TimeOnly start, TimeOnly end);
        bool IsValidReserveDate(DateOnly reserveDate);
        Task<Reserve?> CheckExistingReservation(CreateReserveViewModel reserveModel);
        bool CreateReservation(CreateReserveViewModel reserveModel);
    }
}
