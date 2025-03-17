using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using ProyectoActivoFijo.Controllers;
using Newtonsoft.Json;

namespace ProyectoActivoFijo.Models
{
    public class Asset
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("ubicacion")]
        public string Ubicacion { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [Display(Name = "Fecha Adquisición")]
        [JsonProperty("fecha_adquisicion")]
        public DateTime FechaAdquisicion { get; set; }
    }

    public class AssetHelper
    {

        public async Task<bool> saveAsset(Asset asset)
        {
            try
            {

                FirestoreDb db = FirestoreDb.Create("dotnet-activos-fijos");
                DocumentReference docRef = await db.Collection("activos").AddAsync(
                    new Dictionary<string, object>
                    {
                        //esto es un objeto json, por eso va entre parentesis
                        {"Id",asset.ID },
                        {"Nombre",asset.Nombre },
                        {"Estado",asset.Estado },
                        {"Ubicacion",asset.Ubicacion },
                        {"Fecha",asset.FechaAdquisicion.ToUniversalTime()  },
                    });

                return true;
            }
            catch
            {
                return false;
            }





        }

        public async Task<bool> SaveHistorialCambio(HistorialCambio cambio)
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create("dotnet-activos-fijos");
                DocumentReference docRef = await db.Collection("HistorialCambios").AddAsync(
                    new Dictionary<string, object>
                    {
                    {"Fecha", cambio.Fecha.ToUniversalTime()},
                    {"UsuarioEmail", cambio.UsuarioEmail},
                    {"Accion", cambio.Accion},
                    {"Detalles", cambio.Detalles}
                    });
                return true;
            }
            catch
            {
                return false;
            }




        }

    }
}
