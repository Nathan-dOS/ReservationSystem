using Reservation.Data.Enum;
using Reservation.Interfaces;
using Reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Reservation.ViewModel;

namespace Reservation.Services
{
    public class RoomServicesRepository : IRoomServices
    {
        public List<EquipmentViewModel> GetRoomEquipment(EnumRoomType roomType)
        {
            var equipment = new List<EquipmentViewModel>();

            switch (roomType)
            {
                case EnumRoomType.MeetingRoom:
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Projector", EquipmentQuantity = 1 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Whiteboard", EquipmentQuantity = 1 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Conference Phone", EquipmentQuantity = 1 });
                    break;
                case EnumRoomType.Auditorium:
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Sound System", EquipmentQuantity = 1 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Microphone", EquipmentQuantity = 5 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Stage Lighting", EquipmentQuantity = 1 });
                    break;
                case EnumRoomType.LawOffice:
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Bookshelf", EquipmentQuantity = 2 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Desk", EquipmentQuantity = 1 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Computer", EquipmentQuantity = 1 });
                    break;
                case EnumRoomType.StudyRoom:
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Desk", EquipmentQuantity = 10 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Chairs", EquipmentQuantity = 10 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Lamp", EquipmentQuantity = 10 });
                    break;
                case EnumRoomType.MedicalOffice:
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Examination Table", EquipmentQuantity = 1 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Medical Instruments", EquipmentQuantity = 1 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Computer", EquipmentQuantity = 1 });
                    break;
                case EnumRoomType.WaitingRoom:
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Chairs", EquipmentQuantity = 8 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "Magazines", EquipmentQuantity = 6 });
                    equipment.Add(new EquipmentViewModel { EquipmentName = "TV", EquipmentQuantity = 1 });
                    break;
                default:
                    break;
            }

            return equipment;
        }
    }
}
