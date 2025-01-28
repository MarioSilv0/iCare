/// <summary>
/// This file defines the <c>Restriction</c> class, which models a restriction in the system.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Models.Restrictions
{
    /// <summary>
    /// Class <c>Restriction</c> represents a restriction that can be associated with one or more users.
    /// </summary>
    public class Restriction
    {
        /// <value>
        /// Property <c>Id</c> represents the unique identifier of the restriction.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Property <c>Name</c> represents the name of the restriction.
        /// </value>
        public string? Name { get; set; }

        /// <value>
        /// Property <c>UserRestrictions</c> represents the collection of user-restriction associations
        /// that reference this restriction.
        /// </value>
        public ICollection<UserRestriction>? UserRestrictions { get; set; }
    }
}
