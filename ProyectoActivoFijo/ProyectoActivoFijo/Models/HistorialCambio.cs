using Newtonsoft.Json;

namespace ProyectoActivoFijo.Models
{
    public class HistorialCambio
    {
        //public int Id { get; set; }

        //public DateTime Fecha { get; set; }

        //public required string UsuarioEmail { get; set; } /*Esto es para saber quien realizo el cambio*/

        //public required string Accion { get; set; }

        //public required string Detalles { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Fecha")]
        public DateTime Fecha { get; set; }

        [JsonProperty("UsuarioEmail")]
        public required string UsuarioEmail { get; set; }

        [JsonProperty("Accion")]
        public required string Accion { get; set; }

        [JsonProperty("Detalles")]
        public required string Detalles { get; set; }
    }
}
