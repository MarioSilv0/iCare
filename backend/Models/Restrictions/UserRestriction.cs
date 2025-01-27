/// <summary>
/// This file defines the <c>UserRestriction</c> class, which models the association
/// between a user and a restriction in the system.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Models.Restrictions
{
    /// <summary>
    /// Class <c>UserRestriction</c> represents the association between a user and a restriction.
    /// </summary>
    public class UserRestriction
    {
        /// <value>
        /// Property <c>UserId</c> represents the unique identifier of the user associated with the restriction.
        /// </value>
        public string UserId { get; set; }

        /// <value>
        /// Property <c>User</c> represents the user associated with the restriction.
        /// </value>
        public User User { get; set; }

        /// <value>
        /// Property <c>RestrictionId</c> represents the unique identifier of the restriction.
        /// </value>
        public int RestrictionId { get; set; }

        /// <value>
        /// Property <c>Restriction</c> represents the restriction associated with the user.
        /// </value>
        public Restriction Restriction { get; set; }
    }
}
