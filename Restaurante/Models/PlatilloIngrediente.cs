namespace Restaurante.Models
{
    public class PlatilloIngrediente
    {
        public int PlatilloId { get; set; }
        public int IngredienteId { get; set; }
        public decimal CantidadUso { get; set; }

        // Relaciones con Producto e Ingrediente
        public Producto Platillo { get; set; }
        public Ingrediente Ingrediente { get; set; }
    }

}
