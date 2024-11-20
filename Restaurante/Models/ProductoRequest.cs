namespace Restaurante.Models
{
    public class ProductoRequest
    {
        public required string Nombre { get; set; }
        public required decimal Precio { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedida { get; set; }
        public int ProductoTipoId { get; set; }
        public string Foto { get; set; }
    }
}
