using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoTipoController : ControllerBase
    {
        [HttpGet]
        public List<ProductoTipo> GetAll()
        {
            return new Db().ObtenerProductoTipos();
        }

        [HttpGet("{id}")]
        public ProductoTipo GetById(int id)
        {
            return new Db().ObtenerProductoTipoPorId(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductoTipoRequest productoTipoRequest)
        {
            if (string.IsNullOrEmpty(productoTipoRequest.Tipo))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "El campo 'Tipo' es obligatorio",
                    Code = 400
                });
            }

            int result = new Db().SaveProductoTipo(productoTipoRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "El tipo de producto se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un problema al guardar el tipo de producto",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int result = new Db().DeleteProductoTipo(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El tipo de producto se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar el tipo de producto",
                Code = 400
            });
        }
    }
}
