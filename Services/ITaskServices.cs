using System.Security.Claims;
using PetProjectOne.Entities;

namespace PetProjectOne.Services;

public interface ITaskServices
{
    Task<TaskRequest?> CreateTask(TaskRequest task, ClaimsPrincipal user);
    Task<List<TaskRequest>> GetAllTasks();

}
