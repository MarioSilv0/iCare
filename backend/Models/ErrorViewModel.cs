/// <summary>
/// Represents the error view model used to display error-related information to the user.
/// This model typically contains information about the request ID to help with error tracking.
/// </summary>
namespace backend.Models
{
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the unique request ID associated with the error.
        /// This ID can be used for tracking and debugging specific requests that resulted in an error.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request ID should be shown.
        /// Returns true if the request ID is not null or empty, false otherwise.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
