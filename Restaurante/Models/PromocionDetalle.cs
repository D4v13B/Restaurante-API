namespace Restaurante.Models
{
    public class PromocionDetalle
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int PromocionId { get; set; }

        // Relaciones con Producto y Promocion
        public Producto Producto { get; set; }
        public Promocion Promocion { get; set; }
    }

}
