using PetProjectOne.Entities;
using PetProjectOne.Models;

namespace PetProjectOne.Services;

public interface IAuthenticationService
{
    Task<string> Register(RegisterRequest request);
    Task<string> RegisterTasker(TaskerRegisterRequest request);
    Task<string> Login(LoginRequest request);
}
