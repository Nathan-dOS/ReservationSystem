using Reservation.Models;
using Reservation.Repository;

namespace Reservation.Interfaces
{
    public interface IReserveHistoryRepository
    {
        bool AddHistory(ReserveHistory reserveHistory);

        bool Save();

        Task<IEnumerable<ReserveHistory>> GetHistoryByUserID(string userID);
    }
}
