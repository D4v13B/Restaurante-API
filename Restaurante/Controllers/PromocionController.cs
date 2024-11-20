using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromocionController : ControllerBase
    {
        [HttpGet]
        public List<Promocion> GetAll()
        {
            return new Db().ObtenerPromociones();
        }

        [HttpGet("{id}")]
        public Promocion GetById(int id)
        {
            return new Db().ObtenerPromocionPorId(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PromocionRequest promocionRequest)
        {
            if (string.IsNullOrEmpty(promocionRequest.Nombre))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "El campo 'Nombre' es obligatorio",
                    Code = 400
                });
            }

            int result = new Db().SavePromocion(promocionRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "La promoción se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un problema al guardar la promoción",
                Code = 400
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] PromocionRequest promocionRequest)
        {
            var existingPromocion = new Db().ObtenerPromocionPorId(id);
            if (existingPromocion == null)
            {
                return NotFound(new
                {
                    titulo = "Promoción no encontrada",
                    msg = "No se encontró la promoción para actualizar",
                    Code = 404
                });
            }

            int result = new Db().UpdatePromocion(id, promocionRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Actualizado correctamente",
                    msg = "La promoción se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "Hubo un error al actualizar la promoción",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int result = new Db().DeletePromocion(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "La promoción se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar la promoción",
                Code = 400
            });
        }
    }
}
