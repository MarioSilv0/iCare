﻿using System;
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
    //Mário
    public class UserLogsController : Controller
    {
        private readonly ICareServerContext _context;

        public UserLogsController(ICareServerContext context)
        {
            _context = context;
        }

        // GET: UserLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserLogs.Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserLogs/Details/5
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

        // GET: UserLogs/Delete/5
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

        // POST: UserLogs/Delete/5
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

        private bool UserLogExists(int id)
        {
            return _context.UserLogs.Any(e => e.Id == id);
        }
    }
}
