using PetProjectOne.Models;
using PetProjectOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetProjectOne.Entities;
using System.Security.Claims;
using PetProjectOne.Db;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace PetProjectOne.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userServices;
    private readonly ITaskServices _taskServices;
    private readonly AppDbContext _context;

    public UserController(IAuthenticationService authenticationService, IUserService userService, ITaskServices taskServices, AppDbContext context)
    {
        _authenticationService = authenticationService;
        _userServices = userService;
        _taskServices =  taskServices;
        _context = context;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> SeedDataExistsAsync()
    {
        var categories = await _context.Category.ToListAsync();
        return Ok(categories);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authenticationService.Login(request);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authenticationService.Register(request);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("registertasker")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterTasker([FromBody] TaskerRegisterRequest request)
    {
        var registerTasker = await _authenticationService.RegisterTasker(request);
        return Ok(registerTasker);
    }

    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userServices.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("tasker-users")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTaskers()
    {
        var taskerUsers = await _userServices.GetAllTaskers();
        return Ok(taskerUsers);
    }


    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TotalCountOfUsers()
    {
        var totalCount = await _userServices.TotalCountOfUsers();
        return Ok(totalCount);
    }


    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserByIdAsync(string userId)
    {
        try
        {
            await _userServices.DeleteUserByIdAsync(userId);
            return Ok($"User with ID {userId} has been deleted.");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // get loggedin user id
    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserFromToken()
    {
        try
        {
            // Extract user ID from token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token");
            }

            // Fetch user details using the ID
            var user = await _userServices.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                Id = user.Id,
                userName = user.UserName,
                Email = user.Email,
                Name = user.FullName
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // get user by id
    [HttpGet("details/{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        try
        {
            var user = await _userServices.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                Id = user.Id,
                userName = user.UserName,
                Email = user.Email,
                Name = user.FullName
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("taskrequest")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskRequest))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] TaskRequest task)
    {
        if (task == null)
        {
            return BadRequest("Invalid request data");
        }

        var category = _context.Category.Find(task.CategoryId);
        if (category == null)
        {
            return BadRequest("Category not found");
        }

        // Call the service to create the task with the provided data
        var result = await _taskServices.CreateTask(task, User);

        if (result == null)
        {
            return Unauthorized("User is not authenticated");
        }

        return Ok(result);
    }

    // Get assigned tasks
    [HttpGet("assigned-tasks")]
    public async Task<IActionResult> GetAssignedTasks([FromQuery] string taskerId)
    {
        // Ensure taskerId is provided
        if (string.IsNullOrEmpty(taskerId))
        {
            return BadRequest("Tasker ID is required.");
        }

        // Fetch tasks assigned to the tasker
        var tasksAssignedToTasker = await _context.Tasks
                                                .Where(t => t.AssignedToId == taskerId)
                                                .ToListAsync();

        if (tasksAssignedToTasker.Count == 0)
        {
            return NotFound("No tasks assigned to this tasker.");
        }

        return Ok(tasksAssignedToTasker);
    }

    // update created tasks (Task table) with assignedToId value
    [HttpPut("task/{taskId}/assign")]
    public async Task<IActionResult> AssignTask(int taskId, [FromBody] AssignTaskDto assignTask)
    {
        var taskerProfile = await _context.TaskerProfiles
                                        .FirstOrDefaultAsync(t => t.UserId == assignTask.AssignedToId);  // Assuming UserId is string
        if (taskerProfile == null) return BadRequest("Tasker not found");

        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return NotFound("Task not found");

        // Update the task's AssignedToId
        task.AssignedToId = assignTask.AssignedToId;
        await _context.SaveChangesAsync();

        // Fetch the User to get the email using Tasker's UserId
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == taskerProfile.UserId);
        if (user == null) return BadRequest("User not found");

        // Send the email notification
        // var emailService = new EmailService();
        // await emailService.SendTaskAssignedEmail(user.Email, task.Title);
        return Ok(task);
    }

    // update created tasks (Task table) with status value from assigned tasker
    [HttpPut("task/{taskId}/status")]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] string newStatus)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return NotFound("Task not found");

        task.Status = newStatus;
        await _context.SaveChangesAsync();

        return Ok(task);
    }


    // get all tasks
    [HttpGet("tasks")]
    public async Task<IActionResult> GetAllTasks()
    {
        // Retrieve all tasks from the database
        var tasks = await _taskServices.GetAllTasks();

        // Check if tasks exist, if not return a Not Found response
        if (tasks == null || !tasks.Any())
        {
            return NotFound("No tasks found.");
        }

        // Return the tasks as a response
        return Ok(tasks);
    }

    // Get active tasks
    [HttpGet("active-tasks")]
    public async Task<IActionResult> GetActiveTasks()
    {
        // Retrieve all tasks from the database
        var tasks = await _taskServices.GetActiveTasks();

        // Check if tasks exist, if not return a Not Found response
        if (tasks == null || !tasks.Any())
        {
            return NotFound("No tasks found.");
        }

        // Return the tasks as a response
        return Ok(tasks);
    }

}

