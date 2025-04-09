using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    /// <summary>
    /// Controller responsible for managing user activity logs.
    /// </summary>
    public class UserLogsController : Controller
    {
        /// <summary>
        /// The database context used to access user logs.
        /// </summary>
        private readonly ICareServerContext _context;

        /// <summary>
        /// Constructor for the UserLogsController class.
        /// </summary>
        /// <param name="context">The database context instance.</param>
        public UserLogsController(ICareServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves and displays the list of user activity logs.
        /// </summary>
        /// <returns>A view displaying the list of user logs.</returns>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserLogs.Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Retrieves and displays the details of a specific user activity log.
        /// </summary>
        /// <param name="id">The identifier of the user log.</param>
        /// <returns>A view with the details of the user log, or a NotFound result if the log does not exist.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLog = await _context.UserLogs
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLog == null)
            {
                return NotFound();
            }

            return View(userLog);
        }

        /// <summary>
        /// Retrieves and displays the confirmation page to delete a user activity log.
        /// </summary>
        /// <param name="id">The identifier of the user log.</param>
        /// <returns>A view with the details of the user log to be deleted, or a NotFound result if the log does not exist.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLog = await _context.UserLogs
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLog == null)
            {
                return NotFound();
            }

            return View(userLog);
        }

        /// <summary>
        /// Confirms and performs the deletion of a user activity log.
        /// </summary>
        /// <param name="id">The identifier of the user log.</param>
        /// <returns>Redirects to the index page after the log is deleted.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userLog = await _context.UserLogs.FindAsync(id);
            if (userLog != null)
            {
                _context.UserLogs.Remove(userLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a user activity log exists in the database.
        /// </summary>
        /// <param name="id">The identifier of the user log.</param>
        /// <returns>True if the log exists, false otherwise.</returns>
        private bool UserLogExists(int id)
        {
            return _context.UserLogs.Any(e => e.Id == id);
        }
    }
}
