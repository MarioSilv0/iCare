/// <summary>
/// This file defines the <c>PermissionsDTO</c> class, which represents a data transfer object 
/// containing user permission settings, such as notifications, preferences, and restrictions.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-07</date>

namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// The <c>PermissionsDTO</c> class represents a user's permission settings,
    /// including whether notifications, preferences, and restrictions are enabled.
    /// </summary>
    public class PermissionsDTO
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user has enabled notifications.
        /// </summary>
        public Boolean Notifications {  get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has specified preferences.
        /// </summary>
        public Boolean Preferences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has specified restrictions.
        /// </summary>
        public Boolean Restrictions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has inventory items.
        /// </summary>
        public Boolean Inventory { get; set; }
    }
}
