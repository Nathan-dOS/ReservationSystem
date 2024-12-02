using System.Threading.Tasks;
using System.Collections.Generic;
using Reservation.Controllers;
using Reservation.Models;

public interface IReserveRepository
{
    Task<Reserve> GetReserveByIdAsync(int reserveId);
    Task<IEnumerable<Reserve>> GetReservesByUserIdAsync(string userId);
    Task<IEnumerable<Reserve>> GetReservesByRoomIdAsync(int roomId);
    Task ProcessExpiredReservations();
    Task<Reserve> GetReserveByRoomAndDateAsync(int roomId, DateOnly reserveDate, TimeOnly reserveStart, TimeOnly reserveEnd);
    Task<IEnumerable<Reserve>> GetReserveWhereStatusIsValidAsync(string UserId);
    Task<bool> UpdateReserveAsync(Reserve reserve);
    bool AddReserve(Reserve reserve);
    bool UpdateReserve(Reserve reserve);
    bool DeleteReserve(Reserve reserve);
    bool Save();
}
