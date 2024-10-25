using Microsoft.EntityFrameworkCore;

namespace Reservation.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Reservation.Models.AdministrativeManager> AdministrativeManagers { get; set; }
        public DbSet<Reservation.Models.GeneralManager> GeneralManagers { get; set; }
        public DbSet<Reservation.Models.User> Users { get; set; }
        public DbSet<Reservation.Models.Reserve> Reserves { get; set; }
        public DbSet<Reservation.Models.Room> Rooms { get; set; }
        public DbSet<Reservation.Models.Equipment> Equipments { get; set; }

    }
}
