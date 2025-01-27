using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetProjectOne.Entities;

namespace PetProjectOne.Db
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TaskRequest>()
                .HasOne(tr => tr.PostedBy)
                .WithMany(u => u.Tasks)
                .HasForeignKey(tr => tr.PostedById)
                .IsRequired();
                
            // New relationship for AssignedToId with TaskerProfile
            modelBuilder.Entity<TaskRequest>()
                .HasOne(tr => tr.AssignedTo)
                .WithMany()
                .HasForeignKey(tr => tr.AssignedToId)
                .HasPrincipalKey(tp => tp.UserId);

            modelBuilder.Entity<TaskCategory>().HasData(
                new TaskCategory { Id = 1, Category = "Cleaning" },
                new TaskCategory { Id = 2, Category = "Plumbing" },
                new TaskCategory { Id = 3, Category = "Electrical" },
                new TaskCategory { Id = 4, Category = "Gardening" },
                new TaskCategory { Id = 5, Category = "Painting" },
                new TaskCategory { Id = 6, Category = "Moving" },
                new TaskCategory { Id = 7, Category = "Carpentry" }
            );
        }

        public DbSet<TaskRequest> Tasks { get; set; }
        public DbSet<TaskCategory> Category { get; set; }  
        public DbSet<TaskerProfile> TaskerProfiles { get; set; }
    }
}