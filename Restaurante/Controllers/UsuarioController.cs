using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;

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
                return Ok(new
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

        [HttpPut("{id}")]
        public IActionResult UpdateUsuario(int id, [FromBody] UsuarioRequest usuarioRequest)
        {
            int result = new Db().ActualizarUsuario(id, usuarioRequest);

            if (result > 0)
            {
                return Ok("Usuario actualizado exitosamente");
            }
            else
            {
                return NotFound("Usuario no encontrado");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                // Llamamos al método EliminarUsuario de la clase Db
                int result = new Db().EliminarUsuario(id);
                
                // Si result es mayor que 0, significa que el usuario fue eliminado exitosamente
                if(result > 0)
                {r
                    return Ok(new { message = "Usuario eliminado exitosamente" });
                }
                else
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, devolvemos un mensaje de error
                return StatusCode(500, new { message = "Ocurrió un error al eliminar el usuario", error = ex.Message });
            }
        }

    }
}