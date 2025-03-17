using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
 
using ProyectoActivoFijo.Models;

namespace ProyectoActivoFijo.Controllers
{
    public class AssetController : Controller
    {
        FireBaseController fbc = new FireBaseController();

        // GET: AssetController
        public async Task<ActionResult> Index()
        {
            List<Asset> listAssets = await fbc.GetDataAsync<Asset>("activos");
            return View(listAssets);
        }

        public IActionResult Add()
        {
            return View();
        }

        //Aqui deje el anterior en caso de que no sea el ideal y se pueda restablecer 
        //// GET: AssetController/Edit/5
        //public async Task<ActionResult> Edit(int id)
        //{
        //    var filtros = new Dictionary<string, (string, object)>
        //    { 
        //        { "id", ("EQUAL", id) },
        //    };

        //    Asset asset = new Asset();
        //    List<Asset> listAssets = await fbc.GetDataAsync<Asset>("activos", filtros);

        //    if (listAssets.Count != 1)
        //    {
        //        return NotFound();
        //    }

        //    return View(listAssets.FirstOrDefault());

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Asset updatedAsset) 
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create("dotnet-activos-fijos");
                DocumentReference docRef = db.Collection("activos").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                    return NotFound();

                await docRef.UpdateAsync(new Dictionary<string, object>
        {
            {"Nombre", updatedAsset.Nombre},
            {"Estado", updatedAsset.Estado},
            {"Ubicacion", updatedAsset.Ubicacion},
            {"Descripcion", updatedAsset.Descripcion},
            {"Fecha", updatedAsset.FechaAdquisicion.ToUniversalTime()}
        });

                
                HistorialCambio cambio = new HistorialCambio
                {
                    Fecha = DateTime.Now,
                    UsuarioEmail = "usuariotest@domain.com", 
                    Accion = "Edición de Activo",
                    Detalles = $"Se editó el activo {updatedAsset.ID}: {updatedAsset.Nombre}"
                };

                AssetHelper assetHelper = new AssetHelper();
                await assetHelper.SaveHistorialCambio(cambio);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(updatedAsset);
            }
        }


        // GET: AssetController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AssetController/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind( "ID, Nombre, Descripcion, Ubicacion, Estado, FechaAdquisicion")] Asset asset)
        {
            return View();
        }

        //Anterior, se puede restablecer en caso de que no sea el ideal
        //public ActionResult CreateAsset(string txtId, string txtNombre, string txtEstado, string txtUbicacion, string txtDescripcion, DateTime txtFechadquisicion )
        //{
        //    AssetHelper assetHelper = new AssetHelper(); 

        //    bool result = assetHelper.saveAsset(new Asset
        //    {
        //        ID = txtId,
        //        Nombre = txtNombre,
        //        Estado = txtEstado,
        //        Ubicacion = txtUbicacion,
        //        Descripcion = txtDescripcion,
        //        FechaAdquisicion = txtFechadquisicion,


        //    }).Result;

        //    return RedirectToAction("Index");

        //}

        [HttpPost]
        public async Task<ActionResult> CreateAsset( string txtId, string txtNombre, string txtEstado, string txtUbicacion, string txtDescripcion, DateTime txtFechadquisicion)
        {
            AssetHelper assetHelper = new AssetHelper();

            
            bool result = await assetHelper.saveAsset(new Asset
            {
                ID = txtId,
                Nombre = txtNombre,
                Estado = txtEstado,
                Ubicacion = txtUbicacion,
                Descripcion = txtDescripcion,
                FechaAdquisicion = txtFechadquisicion,
            });

            if (result)
            {
               
                HistorialCambio cambio = new HistorialCambio
                {
                    Fecha = DateTime.Now,
                    UsuarioEmail = "usuariotest@domain.com", //aqui supongo que pone el correo de admin? en caso que, se cambia
                    Accion = "Creación de Activo",
                    Detalles = $"Se creó el activo {txtId}: {txtNombre}"
                };

                await assetHelper.SaveHistorialCambio(cambio);
            }

            return RedirectToAction("Index");
        }



        // POST: AssetController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
         

        // POST: AssetController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AssetController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AssetController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
