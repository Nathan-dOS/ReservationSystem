namespace Reservation.Models
{
    public class AdministrativeManager : User
    {
        public string? Staff { get; set; }
        public string? AssignedBuilding { get; set; }
        public DateOnly? EntryDate { get; set; }

    }
}
