using Microsoft.AspNetCore.Http;
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

        // GET: AssetController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var filtros = new Dictionary<string, (string, object)>
            { 
                { "id", ("EQUAL", id) },
            };

            Asset asset = new Asset();
            List<Asset> listAssets = await fbc.GetDataAsync<Asset>("activos", filtros);

            if (listAssets.Count != 1)
            {
                return NotFound();
            }

            return View(listAssets.FirstOrDefault());
            
        }

        // GET: AssetController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AssetController/Create
        public ActionResult Create()
        {
            return View();
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
