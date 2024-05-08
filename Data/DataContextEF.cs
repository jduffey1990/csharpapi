using DotnetApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
     public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<User> Users {get; set;}
        public virtual DbSet<UserSalary> UserSalary {get; set;}
        public virtual DbSet<UserJobInfo> UserJobInfo {get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                        optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            modelBuilder.Entity<User>()
                .ToTable("Users", "TutorialAppSchema") //This is because User is the model, Users is the table
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UserSalary>()  //These don't need it, because the table matches the model
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UserJobInfo>()
                .HasKey(u => u.UserId);
        }
    }
}
