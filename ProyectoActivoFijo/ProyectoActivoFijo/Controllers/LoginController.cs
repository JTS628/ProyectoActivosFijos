using System.Text;
using ProyectoActivoFijo.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc; 

namespace ProyectoActivoFijo.Controllers
{
    public class LoginController : Controller
    { 
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        async public Task<IActionResult> Authenticate(Models.User user)
        {
            FireBaseController fbc = new FireBaseController();

            try
            {
                UserCredential userCreadential = await fbc.SignInWithEmailAndPassowrd(user.Username, user.Password);

                if (userCreadential == null)
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos";
                    return View("Index");
                }

                Models.User userLogged = new Models.User();
                
                userLogged.UID = userCreadential.User.Uid;
                userLogged.Username = user.Username;

                // Falta setear datos en session
                // HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(user));
                //HttpContext.Session.Set("user_logged", Encoding.UTF8.GetBytes(userLogged.ToString()));
                return Redirect("/Asset");
            } catch (Exception ex) {
                ViewBag.Error = ex.Message;
            }

            return View("Index");
        }

        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

    }
}
