    using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        [HttpGet]
        public List<Producto> GetProducts()
        {
            return new Db().ObtenerProductos();
        }

        [HttpGet("{id}")]
        public Producto GetProductById(int id)
        {
            return new Db().ObtenerProductoPorId(id);
        }

        [HttpPost]
        public IActionResult SaveProduct(ProductoRequest productoRequest)
        {
            string[] datos = { productoRequest.Nombre, productoRequest.Precio.ToString(), productoRequest.ProductoTipoId.ToString() };
            // Verificación de datos
            if (datos.Any(string.IsNullOrEmpty))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "Llenar todos los campos",
                    Code = 400
                });
            }

            int verifySave = new Db().SaveProduct(productoRequest);
            if (verifySave > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "El producto se ha guardado exitosamente",
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
        public IActionResult UpdateProduct(int id, ProductoRequest productoRequest)
        {
            var existingProduct = new Db().ObtenerProductoPorId(id);
            if (existingProduct == null)
            {
                return NotFound(new
                {
                    titulo = "Producto no encontrado",
                    msg = "No se ha encontrado el producto para actualizar",
                    Code = 404
                });
            }

            int verifyUpdate = new Db().UpdateProduct(id, productoRequest);
            if (verifyUpdate > 0)
            {
                return Ok(new
                {
                    titulo = "Actualizado correctamente",
                    msg = "El producto se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "Hubo un error al actualizar el producto",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            int verifyDelete = new Db().DeleteProduct(id);
            if (verifyDelete > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El producto se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar el producto",
                Code = 400
            });
        }
    }
}
