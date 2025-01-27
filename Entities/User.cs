using Microsoft.AspNetCore.Identity;
using PetProjectOne.Models;

namespace PetProjectOne.Entities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsTasker { get; set; } = false; 
        public ICollection<TaskRequest> Tasks { get; set;}
    }
}