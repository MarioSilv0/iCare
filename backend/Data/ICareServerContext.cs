using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Identity;
using backend.Models.Ingredients;

namespace backend.Data
{
    //Mário e Luis
    public class ICareServerContext : IdentityDbContext<User>
{
        public ICareServerContext(DbContextOptions<ICareServerContext> options) : base(options) {}

        public DbSet<UserLog> UserLogs { get; set; } = default!;
        public DbSet<Preference> Preferences { get; set; } = default!;
        public DbSet<UserPreference> UserPreferences { get; set; } = default!;
        public DbSet<Restriction> Restrictions { get; set; } = default!;
        public DbSet<UserRestriction> UserRestrictions { get; set; } = default!;
        public DbSet<Ingredient> Ingredients { get; set; } = default!;
        public DbSet<UserIngredient> UserIngredients { get; set; } = default!;
        public DbSet<Recipe> Recipes { get; set; } = default!;
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserPreference>()
                .HasKey(up => new { up.UserId, up.PreferenceId }); // Composite key
            modelBuilder.Entity<UserRestriction>()
                .HasKey(up => new { up.UserId, up.RestrictionId }); // Composite key
            modelBuilder.Entity<UserIngredient>()
                .HasKey(ui => new { ui.UserId, ui.IngredientId }); // Composite key
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId }); // Composite key

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPreferences)
                .HasForeignKey(up => up.UserId);
            modelBuilder.Entity<UserRestriction>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserRestrictions)
                .HasForeignKey(up => up.UserId);
            modelBuilder.Entity<UserIngredient>()
                .HasOne(ui => ui.User)
                .WithMany(u => u.UserIngredients)
                .HasForeignKey(up => up.UserId);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.Preference)
                .WithMany(p => p.UserPreferences)
                .HasForeignKey(up => up.PreferenceId);
            modelBuilder.Entity<UserRestriction>()
                .HasOne(up => up.Restriction)
                .WithMany(p => p.UserRestrictions)
                .HasForeignKey(up => up.RestrictionId);
            modelBuilder.Entity<UserIngredient>()
                .HasOne(ui => ui.Ingredient)
                .WithMany(p => p.UserIngredients)
                .HasForeignKey(ui => ui.IngredientId);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);

            modelBuilder.Entity<Preference>().HasData(
                new Preference { Id = 1, Name = "Vegetarian" },
                new Preference { Id = 2, Name = "Vegan" },
                new Preference { Id = 3, Name = "Carnivore" },
                new Preference { Id = 4, Name = "Keto" }
            );
            modelBuilder.Entity<Restriction>().HasData(
                new Restriction { Id = 1, Name = "Lactose Intolerance" },
                new Restriction { Id = 2, Name = "Gluten Intolerance" }
            );
        }

    }
}
