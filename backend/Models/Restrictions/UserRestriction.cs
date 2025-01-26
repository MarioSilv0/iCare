namespace backend.Models.Restrictions
{
    /// <summary>
    /// Represents the relationship between a user and a restriction.
    /// </summary>
    public class UserRestriction
    {
        /// <value>
        /// Property <c>UserId</c> represents the unique identifier of the user associated with the restriction.
        /// </value>
        public string UserId { get; set; }

        /// <value>
        /// Property <c>User</c> provides navigation to the associated user entity.
        /// </value>
        public User User { get; set; }

        /// <value>
        /// Property <c>RestrictionId</c> represents the unique identifier of the restriction associated with the user.
        /// </value>
        public int RestrictionId { get; set; }

        /// <value>
        /// Property <c>Restriction</c> provides navigation to the associated restriction entity.
        /// </value>
        public Restriction Restriction { get; set; }
    }
}
