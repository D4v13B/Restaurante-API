using Microsoft.AspNetCore.Mvc;
using Restaurante.Datos;
using Restaurante.Models;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenDetalleController : ControllerBase
    {

            //Obtiene todos los detalles
            [HttpGet("{id}")]
            public List<OrdenDetalle> GetOrdenDetalle(int id)
            {
                return new Db().ObtenerOrdenDetalle(id);
            }

            //Guarda los detalles de la orden
            [HttpPost]
            public IActionResult SaveOrden(OrdenDetalleRequest ordenDetalleRequest)
            {
                string[] datos = { ordenDetalleRequest.OrdenId.ToString(), ordenDetalleRequest.Cantidad.ToString(), ordenDetalleRequest.Precio.ToString(), ordenDetalleRequest.ProductoId.ToString(), ordenDetalleRequest.Descuento.ToString() };
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

                int verifySave = new Db().CrearOrdenDetalle(ordenDetalleRequest);
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

            
            //Actualiza las ordenesDetalle
            [HttpPut("{id}")]
            public IActionResult UpdateOrdenDetalle(int id, OrdenDetalleRequest ordenDetalleRequest)
            {
                {
                    var existingProduct = new Db().UpdateOrdenDetalle(id, ordenDetalleRequest);
                    if (existingProduct == null)
                    {
                        return NotFound(new
                        {
                            titulo = "Producto no encontrado",
                            msg = "No se ha encontrado el producto para actualizar",
                            Code = 404
                        });
                    }

                    int verifyUpdate = new Db().UpdateOrdenDetalle(id, ordenDetalleRequest);
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

            
            //Elimina las ordenes
            [HttpDelete("{id}")]
            public IActionResult DeleteOrdenDetalle(int id)
            {
                int verifyDelete = new Db().DeleteOrdenDetalle(id);
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
