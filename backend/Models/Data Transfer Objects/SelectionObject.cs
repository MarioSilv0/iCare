/// <summary>
/// This file defines the <c>SelectionObject</c> class, which represents an object
/// that can be selected and includes associated metadata.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Models
{
    /// <summary>
    /// Class <c>SelectionObject</c> represents an item that includes an identifier, a name,
    /// and a selection status.
    /// </summary>
    public class SelectionObject
    {
        /// <value>
        /// Property <c>Id</c> represents the unique identifier of the selection object.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Property <c>Name</c> represents the name of the selection object.
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// Property <c>IsSelected</c> indicates whether the selection object is selected.
        /// </value>
        public bool IsSelected { get; set; }
    }
}
