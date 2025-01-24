using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Preferences;

namespace backend.Data
{
    //Mário
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<backend.Models.User> User { get; set; } = default!;
        public DbSet<backend.Models.UserLog> UserLogs { get; set; } = default!;
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserPreference>()
                .HasKey(up => new { up.UserId, up.PreferenceId }); // Composite key

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPreferences)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.Preference)
                .WithMany(p => p.UserPreferences)
                .HasForeignKey(up => up.PreferenceId);

            modelBuilder.Entity<Preference>().HasData(
                new Preference { Id = 1, Name = "Vegetarian" },
                new Preference { Id = 2, Name = "Vegan" },
                new Preference { Id = 3, Name = "Carnivore" },
                new Preference { Id = 4, Name = "Keto" }
            );
        }

    }
}
