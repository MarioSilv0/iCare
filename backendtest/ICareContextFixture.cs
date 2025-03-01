/// <summary>
/// This file defines the <c>ICareContextFixture</c> class, which is a test fixture 
/// that provides an in-memory database context for unit tests.
/// It ensures a clean and isolated test environment by creating and disposing of a database context.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backendtest
{
    /// <summary>
    /// The <c>ICareContextFixture</c> class sets up an in-memory database for testing.
    /// It ensures that test cases have access to a fresh database context while also 
    /// cleaning up after execution to maintain test isolation.
    /// Implements <see cref="IDisposable"/> to properly clean up resources.
    /// </summary>
    public class ICareContextFixture : IDisposable
    {
        /// <summary>
        /// Gets the database context used for testing.
        /// </summary>
        public ICareServerContext DbContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <c>ICareContextFixture</c> class.
        /// It configures an in-memory database for test cases and ensures necessary test data is available.
        /// </summary>
        public ICareContextFixture()
        {
            var options = new DbContextOptionsBuilder<ICareServerContext>()
                                .UseInMemoryDatabase(databaseName: "ICareTestDatabase")
                                .Options;

            DbContext = new ICareServerContext(options);
            DbContext.Database.EnsureCreated();

            // Persist Data created on OnModelCreating
            if (DbContext.Ingredients.Any() || DbContext.Recipes.Any())
            {
                DbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Cleans up the database by deleting the in-memory instance and disposing of the context.
        /// This ensures that test cases do not interfere with each other.
        /// </summary>
        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}
