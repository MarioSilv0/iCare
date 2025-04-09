using System.ComponentModel.DataAnnotations;
using System.Net;

namespace backend.Models
{
    /// <summary>
    /// Represents a log entry for a user, capturing details about the user's activities
    /// such as the message, timestamp, and associated IP address.
    /// </summary>
    public class UserLog
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user associated with the log entry.
        /// This value is used to track the user related to the log event.
        /// </summary>
        [Display(Name = "Utilizador")]
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the log entry.
        /// This message provides a description of the user's activity.
        /// </summary>
        [Display(Name = "Mensagem")]
        public required string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the log entry was created.
        /// The timestamp is automatically captured when the event occurs.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Data e Hora")]
        public required DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the user made the request.
        /// This value is helpful for tracing the origin of the user's activity.
        /// </summary>
        [Display(Name = "Endereço de IP")]
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the associated user object.
        /// This property links the log entry to the actual user data.
        /// </summary>
        [Display(Name = "Utilizador")]
        public User? User { get; set; }
    }
}
