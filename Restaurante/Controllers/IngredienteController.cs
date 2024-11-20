using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredienteController : ControllerBase
    {
        [HttpGet]
        public List<Ingrediente> GetIngredientes()
        {
            return new Db().ObtenerIngredientes();
        }

        [HttpGet("{id}")]
        public Ingrediente GetIngredienteById(int id)
        {
            return new Db().ObtenerIngredientePorId(id);
        }

        [HttpPost]
        public IActionResult SaveIngrediente(IngredienteRequest ingredienteRequest)
        {
            string[] datos = { ingredienteRequest.Nombre, ingredienteRequest.Stock.ToString(), ingredienteRequest.UnidadMedida };
            if (datos.Any(string.IsNullOrEmpty))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "Llenar todos los campos",
                    Code = 400
                });
            }

            int verifySave = new Db().SaveIngrediente(ingredienteRequest);
            if (verifySave > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "El ingrediente se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un error al guardar el ingrediente",
                Code = 400
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateIngrediente(int id, IngredienteRequest ingredienteRequest)
        {
            var existingIngrediente = new Db().ObtenerIngredientePorId(id);
            if (existingIngrediente == null)
            {
                return NotFound(new
                {
                    titulo = "Ingrediente no encontrado",
                    msg = "No se ha encontrado el ingrediente para actualizar",
                    Code = 404
                });
            }

            int verifyUpdate = new Db().UpdateIngrediente(id, ingredienteRequest);
            if (verifyUpdate > 0)
            {
                return Ok(new
                {
                    titulo = "Actualizado correctamente",
                    msg = "El ingrediente se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "Hubo un error al actualizar el ingrediente",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteIngrediente(int id)
        {
            int verifyDelete = new Db().DeleteIngrediente(id);
            if (verifyDelete > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El ingrediente se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar el ingrediente",
                Code = 400
            });
        }
    }
}
