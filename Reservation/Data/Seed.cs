using Microsoft.AspNetCore.Identity;
using Reservation.Controllers;
using Reservation.Data.Enum;
using Reservation.Models;
using System.Diagnostics;
using System.Net;

using Room = Reservation.Models.Room;
using Equipment = Reservation.Models.Equipment;



namespace Reservation.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDBContext>();
                context.Database.EnsureCreated();

                if (!context.Rooms.Any())
                {
                    context.Rooms.AddRange(new List<Room>()
                    {
                        new Models.Room()
                        {
                            IdRoom = 1,
                            Number = "101",
                            Capacity = 20,
                            Size = 35.5f,
                            Status = RoomStatus.Available,
                            Price = 500.0f,
                            PhotoAlbum = "/photos/room101/",
                            Internet = true,
                            SecurityCamera = true,
                            AirConditioning = true
                        },
                        new Models.Room()
                        {
                            IdRoom = 2,
                            Number = "102",
                            Capacity = 15,
                            Size = 28.0f,
                            Status = RoomStatus.Occupied,
                            Price = 300.0f,
                            PhotoAlbum = "/photos/room102/",
                            Internet = false,
                            SecurityCamera = false,
                            AirConditioning = true
                        }
                    });
                    context.SaveChanges();
                }

                if (context.Equipments.Any())
                {
                    context.Equipments.AddRange(new List<Models.Equipment>()
                    {
                        new Equipment
                        {
                            IdEquipment = 1,
                            EquipmentName = "Projector",
                            Status = EquipmentStatus.Available,
                            Price = 150.0f,
                            QuantityAvailable = 5
                        },
                        new Equipment
                        {
                            IdEquipment = 2,
                            EquipmentName = "Whiteboard",
                            Status = EquipmentStatus.Maintenance,
                            Price = 50.0f,
                            QuantityAvailable = 2
                        }
                    });
                    context.SaveChanges();
                }
                

            }
        }
    }
}
