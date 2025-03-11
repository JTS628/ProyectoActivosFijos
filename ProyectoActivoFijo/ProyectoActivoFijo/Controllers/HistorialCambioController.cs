using Microsoft.AspNetCore.Mvc;
using ProyectoActivoFijo.Models;
using System.Collections.Generic;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;


namespace ProyectoActivoFijo.Controllers
{
    public class HistorialCambioController : Controller
    {
        private readonly FirebaseClient _firebase;

        public HistorialCambioController()
        {
            // URL o ID?
            _firebase = new FirebaseClient("607667637659");
        }

       
        public async Task<IActionResult> Historial()
        {
            var cambios = await _firebase
                .Child("HistorialCambios")
                .OnceAsync<HistorialCambio>();

            var historial = cambios
                .Select(item => new HistorialCambio
                {
                    Id = int.Parse(item.Key), 
                    Fecha = item.Object.Fecha,
                    UsuarioEmail = item.Object.UsuarioEmail,
                    Accion = item.Object.Accion,
                    Detalles = item.Object.Detalles
                })
                .ToList();

            return View(Historial);
        }

        
        public IActionResult CrearCambio()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearCambio(HistorialCambio cambio)
        {
           
            await _firebase
                .Child("HistorialCambios")
                .PostAsync(cambio);

            return RedirectToAction(nameof(Historial));
        }

        
    }

} 

