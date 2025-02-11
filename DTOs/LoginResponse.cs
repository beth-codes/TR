namespace PetProjectOne.DTOs;

public record LoginResponse(string UserId, string FullName, string Email, string UserName, bool IsTasker, string Token);