using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reservation.ViewModel
{
    public class ReserveHistoryViewModel
    {
        public int ReserveId { get; set; }
        public  string UserId { get; set; }
        public int? EquipementId { get; set; }
        public  DateOnly ReserveDate { get; set; }
        public  TimeOnly ReserveStart { get; set; }
        public  TimeOnly ReserveEnd { get; set; }
        public  string ReserveStatus { get; set; }
        public  float RentPrice { get; set; }
    }
}
