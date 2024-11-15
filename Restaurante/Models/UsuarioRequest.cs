namespace Restaurante.Models
{
    public class UsuarioRequest
    {
        public required string Nombre { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string Telefono { get; set; }
        public int UsuarioTipoId { get; set; }
    }
}
