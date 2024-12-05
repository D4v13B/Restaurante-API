using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        [HttpGet]
        public List<Pedido> GetPedidos()
        {
            return new Db().ObtenerPedidos();
        }

        [HttpGet("{id}")]
        public Pedido GetPedidoById(int id)
        {
            return new Db().ObtenerPedidoPorId(id);
        }

        [HttpPost]
        public IActionResult SavePedido(PedidoRequest pedidoRequest)
        {
           

                if (string.IsNullOrEmpty(pedidoRequest.Cliente) || string.IsNullOrEmpty(pedidoRequest.Producto))
                {
                    return BadRequest(new
                    {
                        titulo = "Error al guardar",
                        msg = "Llenar todos los campos requeridos",
                        Code = 400
                    });
                }

                int verifySave = new Db().GuardarPedido(pedidoRequest);
                if (verifySave > 0)
                {
                    return Ok(new
                    {
                        titulo = "Guardado correctamente",
                        msg = "El pedido se ha guardado exitosamente",
                        Code = 200
                    });

                }

                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "No se pudo guardar el pedido",
                    Code = 400
                });
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePedido(int id, PedidoRequest pedidoRequest)
        {
            int verifyUpdate = new Db().ActualizarPedido(id, pedidoRequest);
            if (verifyUpdate > 0)
            {
                return Ok(new
                {
                    titulo = "Actualizado correctamente",
                    msg = "El pedido se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "No se pudo actualizar el pedido",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePedido(int id)
        {
            int verifyDelete = new Db().EliminarPedido(id);
            if (verifyDelete > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El pedido se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "No se pudo eliminar el pedido",
                Code = 400
            });
        }
    }
}
