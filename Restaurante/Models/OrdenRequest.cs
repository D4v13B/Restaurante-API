namespace Restaurante.Models
{
    public class OrdenRequest
    {
        public DateTime Fecha { get; set; }
        public string ObservacionCliente { get; set; }
        public int MetodoPagoId { get; set; }
        public int OrdenEstadoId { get; set; }
        public int UsuarioId { get; set; }
    }
}
