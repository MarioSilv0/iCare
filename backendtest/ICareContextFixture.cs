//using backend.Data;
//using Microsoft.EntityFrameworkCore;

///// <summary>
///// This file contains a test fixture for setting up and disposing of an in-memory database context used in unit tests.
///// The <c>ICareContextFixture</c> class is responsible for creating and initializing a new in-memory instance of the <c>ICareServerContext</c> before each test,
///// and ensuring proper cleanup after tests by disposing of the database and context.
///// </summary>
///// <author>Luís Martins - 202100239</author>
///// <author>João Morais - 202001541</author>
///// <date>Last Modified: 2025-01-30</date>

//namespace backendtest
//{
//    /// <summary>
//    /// Test fixture for <c>ICareServerContext</c>.
//    /// This class creates an in-memory database for testing purposes, initializing the <c>ICareServerContext</c> with the necessary data for the tests,
//    /// ensuring proper cleanup and resource management by deleting the in-memory database after tests are complete.
//    /// </summary>
//    public class ICareContextFixture : IDisposable
//    {
//        public ICareServerContext DbContext { get; private set; }

//        /// <summary>
//        /// Constructor that initializes the in-memory <c>ICareServerContext</c> with the necessary options for unit tests.
//        /// It ensures the creation of the in-memory database and saves initial data if required by the context (such as preferences and restrictions).
//        /// </summary>
//        public ICareContextFixture()
//        {
//            var options = new DbContextOptionsBuilder<ICareServerContext>()
//                                .UseInMemoryDatabase(databaseName: "ICareTestDatabase")
//                                .Options;

//            DbContext = new ICareServerContext(options);
//            DbContext.Database.EnsureCreated();

//            // Persist Data created on OnModelCreating
//            if (DbContext.Preferences.Any() || DbContext.Restrictions.Any() || DbContext.UserItems.Any())
//            {
//                DbContext.SaveChanges();
//            }
//        }

//        /// <summary>
//        /// Disposes of the in-memory database and context after each test, ensuring proper cleanup of resources.
//        /// It deletes the in-memory database to reset the state for the next test run.
//        /// </summary>
//        public void Dispose()
//        {
//            DbContext.Database.EnsureDeleted();
//            DbContext.Dispose();
//        }
//    }
//}
