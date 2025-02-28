using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PetProjectOne.Models;
using PetProjectOne.Entities;
using PetProjectOne.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PetProjectOne.DTOs;

namespace PetProjectOne.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthenticationService (UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    // Register User
    public async Task<GenericResponse<string>> Register(RegisterRequest request)
    {
        var userByEmail = await _userManager.FindByEmailAsync(request.Email);
        var userByUsername = await _userManager.FindByNameAsync(request.UserName);
        if (userByEmail is not null || userByUsername is not null)
        {
            return GenericResponse<string>.Failed("User already exists, please log in.", 409);
        }

        User user = new()
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
            IsTasker = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if(!result.Succeeded)
        {
            return GenericResponse<string>.Failed("Password must contain at least 6 characters; 1 uppercase, 1 special character, and 1 number.", 409);
        }

        return GenericResponse<string>.Success("User created successfully", "");
    }

    public async Task<GenericResponse<string>> RegisterTasker(TaskerRegisterRequest request)
    {

         var userByEmail = await _userManager.FindByEmailAsync(request.Email);
        var userByUsername = await _userManager.FindByNameAsync(request.UserName);
        if (userByEmail is not null || userByUsername is not null)
        {
            return GenericResponse<string>.Failed("User already exists, please log in.", 409);
        }
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            IsTasker = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

         if(!result.Succeeded)
        {
            return GenericResponse<string>.Failed("Password must contain at least 6 characters; 1 uppercase, 1 special character, and 1 number.", 409);
        }

        // Assign "Tasker" role
        await _userManager.AddToRoleAsync(user, "MemberTasker");

        // Create TaskerProfile
        var taskerProfile = new TaskerProfile
        {
            UserId = user.Id,
            Skills = request.Skills,
            ExperienceLevel = request.ExperienceLevel,
            HourlyRate = request.HourlyRate,
            SelectedCategory = request.SelectedCategory,
            CategoryId = request.CategoryId
        };
        _dbContext.TaskerProfiles.Add(taskerProfile);
        await _dbContext.SaveChangesAsync();

        return GenericResponse<string>.Success("Tasker created successfully!", "");
    }


    // login user
    public async Task<GenericResponse<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail) ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return GenericResponse<LoginResponse>.Failed("Invalid credentials",400);
        }

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = GetToken(authClaims);
        
        var returnedToken =  new JwtSecurityTokenHandler().WriteToken(token);

        var loginResponse = new LoginResponse(user.Id, user.FullName, user.Email, user.UserName, user.IsTasker, returnedToken);

        return GenericResponse<LoginResponse>.Success("Successfully logged in", loginResponse);
    }

    // autorization
    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}