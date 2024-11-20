using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;
using System.Collections.Generic;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetodoPagoController : ControllerBase
    {
        // Obtener todos los métodos de pago
        [HttpGet]
        public List<MetodoPago> GetMetodosPago()
        {
            return new Db().ObtenerMetodosPago();
        }

        // Obtener un método de pago por su ID
        [HttpGet("{id}")]
        public MetodoPago GetMetodoPagoById(int id)
        {
            return new Db().ObtenerMetodoPagoPorId(id);
        }

        // Guardar un nuevo método de pago
        [HttpPost]
        public IActionResult SaveMetodoPago(MetodoPago metodoPago)
        {
            // Verificación de datos
            if (string.IsNullOrEmpty(metodoPago.Nombre))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "El nombre del método de pago es obligatorio",
                    Code = 400
                });
            }

            int verifySave = new Db().SaveMetodoPago(metodoPago);
            if (verifySave > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "El método de pago se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un error al guardar el método de pago",
                Code = 400
            });
        }

        // Actualizar un método de pago
        [HttpPut("{id}")]
        public IActionResult UpdateMetodoPago(int id, MetodoPago metodoPago)
        {
            var existingMetodoPago = new Db().ObtenerMetodoPagoPorId(id);
            if (existingMetodoPago == null)
            {
                return NotFound(new
                {
                    titulo = "Método de pago no encontrado",
                    msg = "No se ha encontrado el método de pago para actualizar",
                    Code = 404
                });
            }

            int verifyUpdate = new Db().UpdateMetodoPago(id, metodoPago);
            if (verifyUpdate > 0)
            {
                return Ok(new
                {
                    titulo = "Actualizado correctamente",
                    msg = "El método de pago se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "Hubo un error al actualizar el método de pago",
                Code = 400
            });
        }

        // Eliminar un método de pago
        [HttpDelete("{id}")]
        public IActionResult DeleteMetodoPago(int id)
        {
            int verifyDelete = new Db().DeleteMetodoPago(id);
            if (verifyDelete > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El método de pago se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar el método de pago",
                Code = 400
            });
        }
    }
}
