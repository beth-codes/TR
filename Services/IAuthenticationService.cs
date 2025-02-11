using PetProjectOne.DTOs;
using PetProjectOne.Entities;
using PetProjectOne.Models;

namespace PetProjectOne.Services;

public interface IAuthenticationService
{
    Task<string> Register(RegisterRequest request);
    Task<string> RegisterTasker(TaskerRegisterRequest request);
    Task<GenericResponse<LoginResponse>> Login(LoginRequest request);
}
