using backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace backend.Controllers
{
    /// <summary>
    /// Controller responsible for managing the main pages of the application.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger instance for the HomeController class to log information and errors.
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Constructor for the HomeController class.
        /// </summary>
        /// <param name="logger">An instance of the logger to record information and errors.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Method that returns the homepage of the application.
        /// </summary>
        /// <returns>A View representing the homepage of the application.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Method that returns the privacy page of the application.
        /// </summary>
        /// <returns>A View representing the privacy page of the application.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Method responsible for handling errors in the application.
        /// </summary>
        /// <returns>A View displaying error details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
