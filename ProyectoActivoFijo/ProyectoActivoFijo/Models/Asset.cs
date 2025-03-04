using Google.Cloud.Firestore;
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

        [JsonProperty("fecha_adquisicion")]
        public DateTime FechaAdquisicion { get; set; }
    }
}
