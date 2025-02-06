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
    public class EmailSenderServiceTest
    {
        private readonly EmailSenderService _emailSenderService;
        private readonly EmailSettings _emailSettings;

        public EmailSenderServiceTest()
        {
            var options = new DbContextOptionsBuilder<ICareServerContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
        }
    }
}
