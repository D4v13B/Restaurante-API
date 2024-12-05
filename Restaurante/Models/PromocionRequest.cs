using Heiwa.Models;

namespace Restaurante.Models
{
    public class PromocionRequest
    {
        public string Nombre { get; set; }
        public DateTime FechaValidezInicio { get; set; }
        public DateTime FechaValidezFinal { get; set; }
        public decimal Precio { get; set; }
        public string? Foto { get; set; }

        public List<ProductPromocion> Productos { get; set; }
    }
}
