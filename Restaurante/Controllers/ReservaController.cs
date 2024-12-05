using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;
using System.Collections.Generic;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly Db db = new Db();

        [HttpGet]
        public List<Reserva> GetReservas()
        {
            return db.ObtenerReservas();
        }

        [HttpGet("{id}")]
        public IActionResult GetReservaById(int id)
        {
            var reserva = db.ObtenerReservaPorId(id);
            if (reserva == null)
            {
                return NotFound(new { titulo = "No encontrado", msg = "La reserva no existe", Code = 404 });
            }
            return Ok(reserva);
        }

        [HttpPost]
        public IActionResult SaveReserva([FromBody] ReservaRequest reserva)
        {
            int verifySave = db.CrearReserva(reserva);
            if (verifySave > 0)
            {
                return Ok(new { titulo = "Guardado correctamente", msg = "La reserva se guardó exitosamente", Code = 200 });
            }
            return BadRequest(new { titulo = "Error al guardar", msg = "Hubo un error al guardar la reserva", Code = 400 });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReserva(int id, [FromBody] ReservaRequest reserva)
        {
            var existingReserva = db.ObtenerReservaPorId(id);
            if (existingReserva == null)
            {
                return NotFound(new { titulo = "No encontrado", msg = "La reserva no existe", Code = 404 });
            }

            int verifyUpdate = db.ActualizarReserva(id, reserva);
            if (verifyUpdate > 0)
            {
                return Ok(new { titulo = "Actualizado correctamente", msg = "La reserva se actualizó exitosamente", Code = 200 });
            }
            return BadRequest(new { titulo = "Error al actualizar", msg = "Hubo un error al actualizar la reserva", Code = 400 });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReserva(int id)
        {
            int verifyDelete = db.EliminarReserva(id);
            if (verifyDelete > 0)
            {
                return Ok(new { titulo = "Eliminado correctamente", msg = "La reserva se eliminó exitosamente", Code = 200 });
            }
            return BadRequest(new { titulo = "Error al eliminar", msg = "Hubo un error al eliminar la reserva", Code = 400 });
        }
    }
}
