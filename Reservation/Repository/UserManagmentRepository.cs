using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Repository
{
    public class UserManagmentRepository : IUserManagmenteRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<User> _userManager;
        public UserManagmentRepository(ApplicationDBContext context, UserManager<User> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string UserID)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == UserID);
        }

     


        public async Task<List<UserManagmentViewModel>> GetAllUsersWithRolesAsync()
        {
            var users = await _dbContext.Users.ToListAsync();
            var userRoles = new List<UserManagmentViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserManagmentViewModel
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CPF = user.CPF,
                    PhoneNumber = user.PhoneNumber,
                    OABNumber = user.OABNumber,
                    CRMNumber = user.CRMNumber,
                    IsBanned = user.IsBanned,
                    Roles = roles.ToList()
                });
            }

            return userRoles;
        }
    }

}
