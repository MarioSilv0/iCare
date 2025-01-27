/// <summary>
/// This file defines the <c>UserPreference</c> class, which models the association
/// between a user and a preference in the system.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Models.Preferences
{
    /// <summary>
    /// Class <c>UserPreference</c> represents the association between a user and a preference.
    /// </summary>
    public class UserPreference
    {
        /// <value>
        /// Property <c>UserId</c> represents the unique identifier of the user.
        /// </value>
        public string UserId { get; set; }

        /// <value>
        /// Property <c>User</c> represents the user associated with the preference.
        /// </value>
        public User User { get; set; }

        /// <value>
        /// Property <c>PreferenceId</c> represents the unique identifier of the preference.
        /// </value>
        public int PreferenceId { get; set; }

        /// <value>
        /// Property <c>Preference</c> represents the preference associated with the user.
        /// </value>
        public Preference Preference { get; set; }
    }
}
