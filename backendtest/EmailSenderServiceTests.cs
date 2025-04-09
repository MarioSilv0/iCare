using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace backendtest
{
    /// <summary>
    /// Unit tests for the <see cref="EmailSenderService"/> class.
    /// </summary>
    public class EmailSenderServiceTest
    {
        private readonly EmailSenderService _emailSenderService; ///< The instance of the <see cref="EmailSenderService"/> being tested.
        private readonly EmailSettings _emailSettings; ///< The email settings used by the service.

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSenderServiceTest"/> class.
        /// Sets up the in-memory database and prepares the test environment.
        /// </summary>
        public EmailSenderServiceTest()
        {
            // Setting up the in-memory database options
            var settings = new DbContextOptionsBuilder<ICareServerContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
        }
    }
}
