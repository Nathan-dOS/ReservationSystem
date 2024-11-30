using Microsoft.AspNetCore.Identity;
using Reservation.Models;
using Reservation.ViewModel;


namespace Reservation.Interfaces
{
    public interface IUserManagmenteRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<List<UserManagmentViewModel>> GetAllUsersWithRolesAsync();
    

    }
}
