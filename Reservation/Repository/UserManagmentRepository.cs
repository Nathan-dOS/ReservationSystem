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

     


        public async Task<List<User>> GetAllUsersWithRolesAsync()
        {
            return await _dbContext.Users.ToListAsync();

        }
    }

}
