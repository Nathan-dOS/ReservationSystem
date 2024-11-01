using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;
using SQLitePCL;
namespace Reservation.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDBContext _context;

        public RoomRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public bool Add(Room room)
        {
            _context.Add(room);
            return Save();
        }

        public bool Update(Room room) 
        {
            _context.Update(room);
            return Save();
        }

        public bool Delete(Room room)
        {
            _context.Remove(room);
            return Save();
        }

        public async Task<IEnumerable<Room>> GetAllRooms()
        {
            return await _context.Rooms.ToListAsync();

        }

        public async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms.FirstOrDefaultAsync(i => i.RoomId == id);
        }
         
        public bool Save()
        {
            var saved = _context.SaveChanges();

            return saved > 0 ? true : false;
        }


    }
}
