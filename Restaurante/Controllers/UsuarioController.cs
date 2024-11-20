using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace Restaurante.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        [HttpGet]
        public List<Usuario> GetUsers()
        {
            return new Db().ObtenerUsuarios();
        }

        [HttpPost]
        public IActionResult SaveUser(UsuarioRequest usuario)
        {
            string[] datos = { usuario.Nombre, usuario.Email, usuario.UsuarioTipoId.ToString() };
            //Vamos a verificar primero
            if (datos.Any(string.IsNullOrEmpty)) 
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "Llenar todos los campos",
                    Code = 400
                });
            }

            int verifySave = new Db().SaveUser(usuario);
            if (verifySave > 0)
            {
                return Ok (new
                {
                    titulo = "Guardado correctamente",
                    msg = "El usuario se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Los datos explotaron",
                Code = 400
            });
        }
    }
}