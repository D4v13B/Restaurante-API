using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenEstadoController : ControllerBase
    {
        [HttpGet]
        public List<OrdenEstado> GetAll()
        {
            return new Db().ObtenerOrdenEstados();
        }

        [HttpGet("{id}")]
        public OrdenEstado GetById(int id)
        {
            return new Db().ObtenerOrdenEstadoPorId(id);
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrdenEstadoRequest ordenEstadoRequest)
        {
            if (string.IsNullOrEmpty(ordenEstadoRequest.Estado))
            {
                return BadRequest(new
                {
                    titulo = "Error al guardar",
                    msg = "El campo 'Estado' es obligatorio",
                    Code = 400
                });
            }

            int result = new Db().SaveOrdenEstado(ordenEstadoRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Guardado correctamente",
                    msg = "El estado de la orden se ha guardado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al guardar",
                msg = "Hubo un problema al guardar el estado de la orden",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int result = new Db().DeleteOrdenEstado(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminado correctamente",
                    msg = "El estado de la orden se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "Hubo un error al eliminar el estado de la orden",
                Code = 400
            });
        }
    }
}
