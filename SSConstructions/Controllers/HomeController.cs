using Microsoft.AspNetCore.Mvc;
using SSConstructions.Models;
using SSConstructions.Repository.Interface;
using System.Diagnostics;

namespace SSConstructions.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsers _usersRepo;
        public HomeController(IUsers usersRepo)
        {
            _usersRepo = usersRepo;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
