namespace Restaurante.Models
{
    public class Promocion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaValidezInicio { get; set; }
        public DateTime FechaValidezFinal { get; set; }
        public decimal Precio { get; set; }
        public string? Foto { get; set; }

        public List<PromocionDetalle> Detalles { get; set; }
    }

}
