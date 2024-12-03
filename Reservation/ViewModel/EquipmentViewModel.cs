namespace Reservation.ViewModel
{
    public class EquipmentViewModel
    {
        public int? EquipmentId { get; set; }
        public required string EquipmentName { get; set; }
        public int? EquipmentQuantity { get; set; }
        public float EquipmentPrice { get; set; }
        public bool IsSelected { get; set; }
    }
}
