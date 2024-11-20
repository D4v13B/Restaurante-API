using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromocionDetalleController : ControllerBase
    {
        [HttpGet]
        public List<PromocionDetalle> GetAll()
        {
            return new Db().ObtenerPromocionDetalles();
        }

        [HttpGet("{id}")]
        public PromocionDetalle GetById(int id)
        {
            return new Db().ObtenerPromocionDetallePorId(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PromocionDetalleRequest promocionDetalleRequest)
        {
            if (promocionDetalleRequest.ProductoId <= 0 || promocionDetalleRequest.PromocionId <= 0)
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "Los campos 'ProductoId' y 'PromocionId' son obligatorios",
                    Code = 400
                });
            }

            int result = new Db().SavePromocionDetalle(promocionDetalleRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "La relación entre el producto y la promoción se ha guardado exitosamente",
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
            int result = new Db().DeletePromocionDetalle(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "La relación entre el producto y la promoción se ha eliminado exitosamente",
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
