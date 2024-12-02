using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Repository
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly ApplicationDBContext _context;

        public EquipmentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<EquipmentViewModel>> GetEquipmentsPriceAsync(List<EquipmentViewModel> equipments)
        {
            var result = new List<EquipmentViewModel>();

            foreach (var item in equipments)
            {
                // Busca o equipamento pelo nome no banco de dados
                var equipmentInDb = await _context.Equipments.FirstOrDefaultAsync(c => c.EquipmentName == item.EquipmentName);

                // Adiciona o equipamento com preço atualizado à lista de resultados
                result.Add(new EquipmentViewModel
                {
                    EquipmentId = equipmentInDb.EquipmentId,
                    EquipmentName = item.EquipmentName,
                    EquipmentQuantity = item.EquipmentQuantity,
                    EquipmentPrice = equipmentInDb.EquipmentPrice
                });
            }

            return result;
        }




        public async Task<IEnumerable<Equipment>> GetAllEquipments()
        {
            return await _context.Equipments.ToListAsync();
        }
        public async Task<Equipment> GetEquipmentById(int id)
        {
            return await _context.Equipments.FirstOrDefaultAsync(c => c.EquipmentId == id);
        }

        public async Task BuyingEquipments(List<EquipmentViewModel> equipments)
        {
            foreach (var equipment in equipments)
            {
                // Busca o equipamento no banco de dados
                var dbEquipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipmentId == equipment.EquipmentId);

                if (dbEquipment == null)
                {
                    throw new Exception($"Equipamento com ID {equipment.EquipmentId} não encontrado.");
                }

                // Verifica se há quantidade suficiente disponível
                if (dbEquipment.QuantityAvailable < equipment.EquipmentQuantity)
                {
                    throw new Exception($"Quantidade insuficiente para o equipamento {dbEquipment.EquipmentName}. Disponível: {dbEquipment.QuantityAvailable}, Solicitado: {equipment.EquipmentQuantity}.");
                }

                // Subtrai a quantidade solicitada do banco
                dbEquipment.QuantityAvailable -= equipment?.EquipmentQuantity?? 0;
            }

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();
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
