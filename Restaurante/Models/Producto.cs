namespace Restaurante.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedida { get; set; }
        public int ProductoTipoId { get; set; }
        public string Foto { get; set; }

        // Relación con ProductoTipo
        public ProductoTipo ProductoTipo { get; set; }

        //Relacion con ingredientes 1:N
        public List<Ingrediente> platillosIngredientes { get; set; }
    }

}
