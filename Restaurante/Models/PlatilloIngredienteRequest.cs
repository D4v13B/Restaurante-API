namespace Restaurante.Models
{
    public class PlatilloIngredienteRequest
    {
        public int PlatilloId { get; set; }
        public int IngredienteId { get; set; }
        public decimal CantidadUso { get; set; }
    }
}
