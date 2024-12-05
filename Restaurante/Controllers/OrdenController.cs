using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;
using Heiwa.Models;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenController : ControllerBase
    {

        [HttpGet]
        public List<Orden> GetOrdens()
        {
            return new Db().ObtenerOrdenes();
        }

        [HttpGet("{id}")]
        public Orden GetOrdenById(int id)
        {
            return new Db().ObtenerOrdenexId(id);
        }

        //Orden y detalle 
        [HttpGet("/detalle{id}")]
        public Orden GetOrderYDetailsById(int id)
        {
            return new Db().ObtenerOrdenYDetallePorId(id);
        }


        [HttpPost]
        public IActionResult SaveOrden(OrdenRequest ordenRequest)
        {
            string[] datos = { ordenRequest.Fecha.ToString(), ordenRequest.MetodoPagoId.ToString(),  ordenRequest.UsuarioId.ToString(), ordenRequest.OrdenEstadoId.ToString()};
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

            int verifySave = new Db().CrearOrden(ordenRequest);
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
        public IActionResult UpdateOrden(int id, OrdenRequest ordenRequest)
        {
            {
                var existingProduct = new Db().UpdateOrden(id, ordenRequest);
                if (existingProduct == null)
                {
                    return NotFound(new
                    {
                        titulo = "Producto no encontrado",
                        msg = "No se ha encontrado el producto para actualizar",
                        Code = 404
                    });
                }

                int verifyUpdate = new Db().UpdateOrden(id, ordenRequest);
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
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrden(int id)
        {
            int verifyDelete = new Db().DeleteOrden(id);
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
