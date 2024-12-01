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
        bool CreateReservation(RoomDetailViewModel reserveVM);
        Task<bool> IsUserBanned(string UserID);
        float CalculatePriceByHours(TimeOnly start, TimeOnly end, float RentPrice);

        bool AddReserveToHistoryAsync(Reserve reserve);

        Task<bool> UpdateStatusHistory(Reserve reserve);
    }
}
