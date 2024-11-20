using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioTipoController : ControllerBase
    {
        [HttpGet]
        public List<UsuarioTipo> GetAll()
        {
            return new Db().ObtenerUsuarioTipos();
        }

        [HttpGet("{id}")]
        public UsuarioTipo GetById(int id)
        {
            return new Db().ObtenerUsuarioTipoPorId(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] UsuarioTipoRequest usuarioTipoRequest)
        {
            if (string.IsNullOrEmpty(usuarioTipoRequest.Tipo))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "El campo 'Tipo' es obligatorio",
                    Code = 400
                });
            }

            int result = new Db().SaveUsuarioTipo(usuarioTipoRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "El tipo de usuario se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un problema al guardar el tipo de usuario",
                Code = 400
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UsuarioTipoRequest usuarioTipoRequest)
        {
            var existingUsuarioTipo = new Db().ObtenerUsuarioTipoPorId(id);
            if (existingUsuarioTipo == null)
            {
                return NotFound(new
                {
                    titulo = "Tipo de usuario no encontrado",
                    msg = "No se encontró el tipo de usuario para actualizar",
                    Code = 404
                });
            }

            int result = new Db().UpdateUsuarioTipo(id, usuarioTipoRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Actualizado correctamente",
                    msg = "El tipo de usuario se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "Hubo un error al actualizar el tipo de usuario",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int result = new Db().DeleteUsuarioTipo(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El tipo de usuario se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar el tipo de usuario",
                Code = 400
            });
        }
    }
}
