﻿/// <summary>
/// This file defines the <c>ICareServerContext</c> class, which serves as the database context for the application.
/// It includes DbSets for various entities such as users, ingredients, recipes, and logs. It also configures composite keys
/// and relationships between entities, along with seeding some initial data for ingredients and recipes.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Models.Ingredients;
using backend.Models.Recipes;
using backend.Models.Goals;

namespace backend.Data
{
    /// <summary>
    /// The <c>ICareServerContext</c> class extends <c>IdentityDbContext</c> and provides a set of DbSets for entities in the application.
    /// It also configures relationships between entities such as users, ingredients, recipes, and user interactions with them.
    /// The context supports composite keys for many-to-many relationships and seeds initial data for ingredients and recipes.
    /// </summary>
    public class ICareServerContext : IdentityDbContext<User>
{
        /// <summary>
        /// Initializes a new instance of the <c>ICareServerContext</c> class.
        /// </summary>
        /// <param name="options">The database context options to configure the context.</param>
        public ICareServerContext(DbContextOptions<ICareServerContext> options) : base(options) {}

        /// <summary>
        /// Represents the collection of user logs in the database.
        /// </summary>
        public DbSet<UserLog> UserLogs { get; set; } = default!;

        /// <summary>
        /// Represents the collection of ingredients in the database.
        /// </summary>
        public DbSet<Ingredient> Ingredients { get; set; } = default!;

        /// <summary>
        /// Represents the collection of user-specific ingredients in the database.
        /// </summary>
        public DbSet<UserIngredient> UserIngredients { get; set; } = default!;

        /// <summary>
        /// Represents the collection of recipe ingredients in the database.
        /// </summary>
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = default!;

        /// <summary>
        /// Represents the collection of recipes in the database.
        /// </summary>
        public DbSet<Recipe> Recipes { get; set; } = default!;

        /// <summary>
        /// Represents the collection of user-specific recipes in the database.
        /// </summary>
        public DbSet<UserRecipe> UserRecipes { get; set; } = default!;

        /// <summary>
        /// Represents the collection of user goals in the database.
        /// </summary>
        public DbSet<Goal> Goals { get; set; }

        /// <summary>
        /// Represents the collection of user goals logs in the database.
        /// </summary>
        public DbSet<GoalLog> GoalLogs { get; set; }

        /// <summary>
        /// Configures the model, including relationships, composite keys, and initial data seeding.
        /// </summary>
        /// <param name="modelBuilder">The model builder to configure the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite keys
            modelBuilder.Entity<UserIngredient>()
                .HasKey(ui => new { ui.UserId, ui.IngredientId });
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            modelBuilder.Entity<UserRecipe>()
                .HasKey(ri => new { ri.UserId, ri.RecipeId });

            // Relationships
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
                .WithMany(u => u.FavoriteRecipes)
                .HasForeignKey(uf => uf.UserId);

            modelBuilder.Entity<UserIngredient>()
                .HasOne(ui => ui.Ingredient)
                .WithMany(i => i.UserIngredients)
                .HasForeignKey(ui => ui.IngredientId);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.IngredientRecipes)
                .HasForeignKey(ri => ri.IngredientId);
            modelBuilder.Entity<UserRecipe>()
                .HasOne(ur => ur.Recipe)
                .WithMany(r => r.UserRecipes)
                .HasForeignKey(ur => ur.RecipeId);

            // Seed initial data
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Arroz", Kcal = 124, KJ = 517, Protein = 2.6f, Carbohydrates = 25.8f, Lipids = 10, Fibers = 10, Category = "Cereais e Derivados" },
                new Ingredient { Id = 2, Name = "Batata", Kcal = 137, KJ = 175, Protein = 1.6f, Carbohydrates = 35.4f, Lipids = 2, Fibers = 103, Category = "Cereais e Derivados" },
                new Ingredient { Id = 3, Name = "Carne", Kcal = 137, KJ = 175, Protein = 1.6f, Carbohydrates = 35.4f, Lipids = 2, Fibers = 103, Category = "Carne" }
            );
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, Picture = "", Name = "Algo de Bom", Instructions = "Tu Consegues", Category = "Bom", Area = "Portugal", UrlVideo = "", Calories = 50, Proteins = 20, Carbohydrates = 50, Lipids = 20, Fibers = 10 },
                new Recipe { Id = 2, Picture = "", Name = "Algo de Mau", Instructions = "Boa Sorte", Category = "Mau", Area = "Bugs", UrlVideo = "", Calories = 100, Proteins = 5, Carbohydrates = 70, Lipids = 25, Fibers = 20 }
            );
            modelBuilder.Entity<RecipeIngredient>().HasData(
                new RecipeIngredient { RecipeId = 1, Measure = "1 xícara", Grams = 20, IngredientId = 1 },
                new RecipeIngredient { RecipeId = 1, Measure = "2 xícara", Grams = 40, IngredientId = 2 },
                new RecipeIngredient { RecipeId = 2, Measure = "5 xícara", Grams = 100, IngredientId = 3 }
            );
        }
    }
}
