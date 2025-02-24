using PetProjectOne.Entities;

namespace PetProjectOne.Services;
public interface IUserService
{
    Task<List<User>> GetAllUsers();
    Task<List<TaskerProfile>> GetAllTaskers();
    Task<int> TotalCountOfUsers();
    Task DeleteUserByIdAsync(string userId);
    Task DeleteTaskByIdAsync(int taskId);
    Task<User> GetUserByIdAsync(string userId);
}
