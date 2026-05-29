using Microsoft.AspNetCore.Mvc;

namespace PasajesAeropuerto.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
