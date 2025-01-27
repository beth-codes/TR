using PetProjectOne.Entities;

namespace PetProjectOne.Services;
public interface IUserService
{
    Task<List<User>> GetAllUsers();
    // Task<List<TaskerProfile>> GetAllTaskers(int? categoryId = null); // Updated method signature
    Task<List<TaskerProfile>> GetAllTaskers();
    Task<int> TotalCountOfUsers();
    Task DeleteUserByIdAsync(string userId);
    Task<User> GetUserByIdAsync(string userId);
}
