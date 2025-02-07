using Microsoft.AspNetCore.Mvc;
using ProyectoActivoFijo.Models;

namespace ProyectoActivoFijo.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate(User user)
        {
            // Aquí puedes agregar lógica de autenticación con una base de datos
            if (user.Username == "admin" && user.Password == "1234")
            {
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View("Index");
        }

        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }
    }
}
