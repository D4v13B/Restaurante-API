using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        [HttpGet]
        public List<Usuario> GetUsers()
        {
            return new Db().ObtenerUsuarios();
        }

        [HttpPost]
        public object SaveUser(UsuarioRequest usuario)
        {
            string[] datos = { usuario.Nombre, usuario.Email, usuario.UsuarioTipoId.ToString() };
            //Vamos a verificar primero
            if (datos.Any(string.IsNullOrEmpty)) 
            {
                return new
                {
                    titulo = "Error al guardar",
                    msg = "Llenar todos los campos",
                    Code = 400
                };
            }

            int verifySave = new Db().SaveUser(usuario);
            if (verifySave > 0)
            {
                return new
                {
                    titulo = "Guardado correctamente",
                    msg = "El usuario se ha guardado exitosamente",
                    Code = 200
                };
            }

            return new
            {
                titulo = "Error al guardar",
                msg = "Los datos explotaron",
                Code = 400
            };
        }


    }
}
