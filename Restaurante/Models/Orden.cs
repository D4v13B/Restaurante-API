namespace Restaurante.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string ObservacionCliente { get; set; }
        public int MetodoPagoId { get; set; }
        public int OrdenEstadoId { get; set; }
        public int UsuarioId { get; set; }

        // Relaciones con MetodoPago, OrdenEstado y Usuario
        public MetodoPago MetodoPago { get; set; }
        public OrdenEstado OrdenEstado { get; set; }
        public Usuario Usuario { get; set; }

        public List<OrdenDetalle> OrdenDetalles { get; set; }
    }

}
