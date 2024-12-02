using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<IEnumerable<Equipment>> GetAllEquipments();
        Task<Equipment> GetEquipmentById(int id);
        Task BuyingEquipments(List<EquipmentViewModel> equipments);
        bool Save();
        bool Update(Equipment equipment);
        bool Add(Equipment equipment);
        bool Remove(Equipment equipment);
        Task<List<EquipmentViewModel>> GetEquipmentsPriceAsync(List<EquipmentViewModel> equipments);

    }
}
