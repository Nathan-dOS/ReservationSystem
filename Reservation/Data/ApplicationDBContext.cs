using Microsoft.EntityFrameworkCore;
using Reservation.Models;

namespace Reservation.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<AdministrativeManager> AdministrativeManagers { get; set; }
        public DbSet<GeneralManager> GeneralManagers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

    }


}


