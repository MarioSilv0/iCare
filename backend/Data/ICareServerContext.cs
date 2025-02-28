using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Identity;
using backend.Models.Ingredients;
using backend.Models.Recipes;

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
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = default!;
        public DbSet<Recipe> Recipes { get; set; } = default!;
        public DbSet<UserRecipe> UserRecipes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite keys
            modelBuilder.Entity<UserPreference>()
                .HasKey(up => new { up.UserId, up.PreferenceId });
            modelBuilder.Entity<UserRestriction>()
                .HasKey(up => new { up.UserId, up.RestrictionId });
            modelBuilder.Entity<UserIngredient>()
                .HasKey(ui => new { ui.UserId, ui.IngredientId });
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            modelBuilder.Entity<UserRecipe>()
                .HasKey(ri => new { ri.UserId, ri.RecipeId });

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPreferences)
                .HasForeignKey(up => up.UserId);
            modelBuilder.Entity<UserRestriction>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRestrictions)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserIngredient>()
                .HasOne(ui => ui.User)
                .WithMany(u => u.UserIngredients)
                .HasForeignKey(ui => ui.UserId);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);
            modelBuilder.Entity<UserRecipe>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.favoriteRecipes)
                .HasForeignKey(uf => uf.UserId);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.Preference)
                .WithMany(p => p.UserPreferences)
                .HasForeignKey(up => up.PreferenceId);
            modelBuilder.Entity<UserRestriction>()
                .HasOne(ur => ur.Restriction)
                .WithMany(r => r.UserRestrictions)
                .HasForeignKey(ur => ur.RestrictionId);
            modelBuilder.Entity<UserIngredient>()
                .HasOne(ui => ui.Ingredient)
                .WithMany(i => i.UserIngredients)
                .HasForeignKey(ui => ui.IngredientId);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);
            modelBuilder.Entity<UserRecipe>()
                .HasOne(ur => ur.Recipe)
                .WithMany(r => r.UserRecipes)
                .HasForeignKey(ur => ur.RecipeId);

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
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Arroz", Kcal = 124, KJ = 517, Protein = 2.6f, Carbohydrates = 25.8f, Lipids = 10, Fibers = 10, Category = "Cereais e Derivados" },
                new Ingredient { Id = 2, Name = "Batata", Kcal = 137, KJ = 175, Protein = 1.6f, Carbohydrates = 35.4f, Lipids = 2, Fibers = 103, Category = "Cereais e Derivados" },
                new Ingredient { Id = 3, Name = "Carne", Kcal = 137, KJ = 175, Protein = 1.6f, Carbohydrates = 35.4f, Lipids = 2, Fibers = 103, Category = "Carne" }
            );
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, Picture = "", Name = "Algo de Bom", Description = "Tu Consegues", Category = "Bom", Area = "Portugal", YoutubeVideo = "" },
                new Recipe { Id = 2, Picture = "", Name = "Algo de Mau", Description = "Boa Sorte", Category = "Mau", Area = "Bugs", YoutubeVideo = "" }
            );
        }
    }
}
