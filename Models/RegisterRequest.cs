using System.ComponentModel.DataAnnotations;

namespace PetProjectOne.Models
{
    public class RegisterRequest
    {
        [Required]
        public bool IsTasker { get; set; }
        public string? FullName{get; set;}
        [Required]
        public string? UserName{get; set;}
        [Required]
        public string? Email{get; set;}
        [Required]
        public string? Password{get; set;}
    }
}