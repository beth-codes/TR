using System.ComponentModel.DataAnnotations;

namespace PetProjectOne.Models
{
    public class TaskerRegisterRequest : RegisterRequest
    {
        public bool IsTasker { get; set; }
        public string? Skills { get; set; } 
        public string? ExperienceLevel { get; set; }
        public string? SelectedCategory { get; set; }
        public decimal HourlyRate { get; set; }
        public int CategoryId { get; set; } 
    }
}
