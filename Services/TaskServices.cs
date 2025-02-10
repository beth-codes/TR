using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PetProjectOne.Db;
using PetProjectOne.Entities;

namespace PetProjectOne.Services;

public class TaskServices : ITaskServices
{
    private readonly AppDbContext _dbContex;

    public TaskServices(AppDbContext context)
    {
        _dbContex = context;
    }

    public async Task<TaskRequest?> CreateTask(TaskRequest task, ClaimsPrincipal user)
    {
        // Extract the user ID from the ClaimsPrincipal (provided by the controller)
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return null; 
        }


        var category = _dbContex.Category.Find(task.CategoryId);
        if (category == null)
        {
            return null; 
        }

        // Create the new task with the provided data
        TaskRequest newTask = new()
        {
            Title = task.Title,
            Description = task.Description,
            Location = task.Location,
            Status = "Pending",
            CreatedAt = task.CreatedAt,
            PostedById = userId,
            CategoryId = task.CategoryId,
            AssignedToId = task.AssignedToId
        };
        Console.WriteLine($"AssignedToId: {task.AssignedToId}");


        // Add and save the task in the database
        await _dbContex.Tasks.AddAsync(newTask);
        await _dbContex.SaveChangesAsync();

        return newTask;
    }

    public async Task<List<TaskRequest>> GetAllTasks()
    {
        return await _dbContex.Tasks.ToListAsync();
    }
    public async Task<List<TaskRequest>> GetActiveTasks()
    {
        return await _dbContex.Tasks.Where(x=>!string.IsNullOrEmpty(x.AssignedToId)).ToListAsync();
    }



}
