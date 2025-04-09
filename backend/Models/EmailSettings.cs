/// <summary>
/// Represents the configuration settings required to send an email through an SMTP server.
/// These settings include server address, port, sender email, password, and SSL encryption option.
/// </summary>
namespace backend.Models
{
    public class EmailSettings
    {
        /// <summary>
        /// Gets or sets the SMTP server address.
        /// This is the address of the mail server used for sending emails (e.g., smtp.gmail.com).
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the port number used by the SMTP server.
        /// Common port numbers for SMTP are 25, 465 (SSL), and 587 (TLS).
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the sender's email address.
        /// This is the email address used to send the email (e.g., example@gmail.com).
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the sender's email account password.
        /// This is used for authentication with the SMTP server.
        /// </summary>
        public string SenderPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL (Secure Sockets Layer) should be enabled.
        /// SSL ensures a secure connection between the client and the mail server.
        /// </summary>
        public bool EnableSsl { get; set; }
    }
}
