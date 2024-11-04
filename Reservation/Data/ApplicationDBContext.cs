using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reservation.Models;

namespace Reservation.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<AdministrativeManager> AdministrativeManagers { get; set; }
        public DbSet<GeneralManager> GeneralManagers { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany(r => r.PhotoAlbum)
                .WithOne(i => i.Room)
                .HasForeignKey(i => i.RoomId);

            base.OnModelCreating(modelBuilder);
        }

    }


}


