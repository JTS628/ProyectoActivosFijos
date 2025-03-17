namespace ProyectoActivoFijo.Models
{
    public class HistorialCambio
    {
        //public int Id { get; set; }

        //public DateTime Fecha { get; set; }

        //public required string UsuarioEmail { get; set; } /*Esto es para saber quien realizo el cambio*/

        //public required string Accion { get; set; }

        //public required string Detalles { get; set; }



        public string ID { get; set; }
        public DateTime Fecha { get; set; }
        public required string UsuarioEmail { get; set; }
        public required string Accion { get; set; }
        public required string Detalles { get; set; }
    }
}
