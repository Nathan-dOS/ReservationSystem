using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;

namespace Reservation.Repository
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly ApplicationDBContext _context;

        public EquipmentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Equipment>> GetAllEquipments()
        {
            return await _context.Equipments.ToListAsync();
        }
        public async Task<Equipment> GetEquipmentById(int id)
        {
            return await _context.Equipments.FirstOrDefaultAsync(c => c.EquipmentId == id);
        }
        public bool Save()
        {
           var saved = _context.SaveChanges();

           return saved > 0? true : false;
        }
        public bool Update(Equipment equipment)
        {
           _context.Update(equipment);
           return Save();

        }
        public bool Add(Equipment equipment)
        {
            _context.Add(equipment);
            return Save();
        }
        public bool Remove(Equipment equipment)
        {
            _context.Remove(equipment);
            return Save();
        }

    }
}
