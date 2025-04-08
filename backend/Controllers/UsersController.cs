using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using backend.Models;

namespace backend.Controllers
{
    /// <summary>
    /// Controller responsible for managing system users.
    /// Accessible only to users with the "Admin" role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor for the UsersController class.
        /// </summary>
        /// <param name="userManager">An instance of the UserManager to handle user management operations.</param>
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves and displays the list of all users.
        /// </summary>
        /// <returns>A view with the list of users.</returns>
        // GET: Users
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        /// <summary>
        /// Displays the form to edit a user, given their ID.
        /// </summary>
        /// <param name="id">The ID of the user to edit.</param>
        /// <returns>A view with the edit form, or an error page if the user is not found.</returns>
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Submits the changes to a user after editing.
        /// </summary>
        /// <param name="model">The model containing the updated user information.</param>
        /// <returns>A redirect to the user list or an error page if the edit is not valid.</returns>
        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.Name = model.Name;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Deletes a user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A redirect to the user list or an error page if the user is not found.</returns>
        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
