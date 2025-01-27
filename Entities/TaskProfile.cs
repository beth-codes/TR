using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetProjectOne.Entities
{
    public class TaskerProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public string? Skills { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? SelectedCategory { get; set; }
        public decimal? HourlyRate { get; set; }

        [ForeignKey("CategoryId")]  
        public int CategoryId { get; set; }
        public TaskCategory Category { get; set; }
    }
}
