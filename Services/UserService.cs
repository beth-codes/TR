using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetProjectOne.Entities;
using PetProjectOne.Db;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PetProjectOne.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;

        public UserService(UserManager<User> userManager, AppDbContext dbContext){
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users;
        }

        public async Task<List<TaskerProfile>> GetAllTaskers()
        {
            var taskerIds = await _userManager.GetUsersInRoleAsync("MemberTasker");
            return await _dbContext.TaskerProfiles
                .Where(tp => taskerIds.Select(u => u.Id).Contains(tp.UserId))
                .Include(tp => tp.User)
                .ToListAsync();
        }

        public async Task<int> TotalCountOfUsers()
        {
            var totalCount = await _userManager.Users.CountAsync();
            return totalCount;
        }

        // get user by id
        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null){
                throw new ArgumentException("User not found");
            }
            return user;
        }

        // Delete  user by id  
        public async Task DeleteUserByIdAsync(string userId)
        {
           var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            
        }  

        // Delete  booked task by id 
       public async Task DeleteTaskByIdAsync(int taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            
            if (task == null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found.");
            }

            _dbContext.Tasks.Remove(task);
            var result = await _dbContext.SaveChangesAsync();

            if (result == 0)
            {
                throw new InvalidOperationException($"Error deleting task with ID {taskId}.");
            }
        }

    }
}