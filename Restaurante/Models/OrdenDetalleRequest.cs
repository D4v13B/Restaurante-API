namespace Restaurante.Models
{
    public class OrdenDetalleRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal? Descuento { get; set; }
        public int OrdenId { get; set; }
    }
}
