using backend.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace backend.Services
{
    /// <summary>
    /// Provides functionality for sending emails asynchronously using SMTP.
    /// This service is configured with email settings such as SMTP server, sender credentials,
    /// and SSL usage from the application's configuration.
    /// </summary>
    public class EmailSenderService
    {
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSenderService"/> class.
        /// </summary>
        /// <param name="emailSettings">The email settings used for configuring the SMTP client.</param>
        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Sends an email asynchronously to the specified recipient.
        /// </summary>
        /// <param name="to">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <returns>A task that represents the asynchronous operation of sending the email.</returns>
        /// <exception cref="SmtpException">Thrown when an error occurs during sending the email via SMTP.</exception>
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = _emailSettings.EnableSsl
            };

            var mail = new MailMessage(_emailSettings.SenderEmail, to, subject, body)
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
