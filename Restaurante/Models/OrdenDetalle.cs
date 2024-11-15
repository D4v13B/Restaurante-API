namespace Restaurante.Models
{
    public class OrdenDetalle
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal? Descuento { get; set; }
        public int OrdenId { get; set; }

        // Relaciones con Producto y Orden
        public Producto Producto { get; set; }
        public Orden Orden { get; set; }
    }

}
