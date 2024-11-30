using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;

namespace Reservation.Repository
{
    public class ReserveRepository : IReserveRepository
    {
        private readonly ApplicationDBContext _context;

        public ReserveRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Reserve> GetReserveByIdAsync(int reserveId)
        {
            return await _context.Reserves.FirstOrDefaultAsync(c => c.ReserveId == reserveId);
        }

        public async Task<IEnumerable<Reserve>> GetReservesByUserIdAsync(string userId)
        {
            return await _context.Reserves.Where(c => c.UserId == userId).ToListAsync();
        }



        public async Task<Reserve> GetReserveByRoomAndDateAsync(int roomId, DateOnly reserveDate, TimeOnly reserveStart, TimeOnly reserveEnd)

        {
            return await _context.Reserves.Where(r => r.RoomId == roomId && r.ReserveDate == reserveDate)
        .Where(r => (r.ReserveStart < reserveEnd && r.ReserveEnd > reserveStart)) // Verificando se há sobreposição de horários
        .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Reserve>> GetReservesByRoomIdAsync(int roomId)
        {
            return await _context.Reserves.Where(r => r.RoomId == roomId).ToListAsync();
        }

        public bool AddReserve(Reserve reserve)
        {
            _context.Reserves.Add(reserve);
            return Save();
        }

        public bool UpdateReserve(Reserve reserve)
        {
            _context.Reserves.Update(reserve);
            return Save();
        }

        public bool DeleteReserve(Reserve reserve)
        {
            _context.Reserves.Remove(reserve);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

     
    }
}
