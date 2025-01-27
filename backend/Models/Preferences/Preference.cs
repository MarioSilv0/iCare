/// <summary>
/// This file defines the <c>Preference</c> class, which models a preference in the system.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Models.Preferences
{
    /// <summary>
    /// Class <c>Preference</c> represents a preference that can be associated with one or more users.
    /// </summary>
    public class Preference
    {
        /// <value>
        /// Property <c>Id</c> represents the unique identifier of the preference.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Property <c>Name</c> represents the name of the preference.
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// Property <c>UserPreferences</c> represents the collection of user-preference associations
        /// that reference this preference.
        /// </value>
        public ICollection<UserPreference> UserPreferences { get; set; }
    }
}
