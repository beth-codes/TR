using PetProjectOne.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetProjectOne.Entities
{
    public class TaskRequest
    {
        public int Id {get; set;}
        public string? Title {get; set;}
        public string? Description {get; set;}
        public string? Location {get; set;}
        public string? PostedById { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        [ForeignKey("PostedById")]          
        public User? PostedBy { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public TaskCategory? Category { get; set; }

        public string? AssignedToId { get; set; }

        [JsonIgnore]
        [ForeignKey("AssignedToId")]
        public TaskerProfile? AssignedTo { get; set; }
        
    }
}