using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatilloIngredienteController : ControllerBase
    {
        [HttpGet]
        public List<PlatilloIngrediente> GetAll()
        {
            return new Db().ObtenerPlatilloIngredientes();
        }

        [HttpGet("{id}")]
        public PlatilloIngrediente GetById(int id)
        {
            return new Db().ObtenerPlatilloIngredientePorId(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PlatilloIngredienteRequest platilloIngredienteRequest)
        {
            if (platilloIngredienteRequest.PlatilloId <= 0 || platilloIngredienteRequest.IngredienteId <= 0)
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "Los campos 'PlatilloId' e 'IngredienteId' son obligatorios",
                    Code = 400
                });
            }

            int result = new Db().SavePlatilloIngrediente(platilloIngredienteRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "La relación entre el platillo y el ingrediente se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un problema al guardar la relación",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int result = new Db().DeletePlatilloIngrediente(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "La relación entre el platillo y el ingrediente se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar la relación",
                Code = 400
            });
        }
    }
}
