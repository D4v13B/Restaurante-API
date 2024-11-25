using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SucursalController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var sucursales = new Db().ObtenerSucursales();
            return Ok(sucursales);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var sucursal = new Db().ObtenerSucursalPorId(id);
            if (sucursal == null)
            {
                return NotFound(new
                {
                    titulo = "Sucursal no encontrada",
                    msg = "No se encontró una sucursal con el ID proporcionado",
                    Code = 404
                });
            }

            return Ok(sucursal);
        }

        [HttpPost]
        public IActionResult Create([FromBody] SucursalRequest sucursalRequest)
        {
            if (string.IsNullOrEmpty(sucursalRequest.Nombre) || string.IsNullOrEmpty(sucursalRequest.Direccion))
            {
                return BadRequest(new
                {
                    titulo = "Error al crear",
                    msg = "Los campos 'Nombre' y 'Dirección' son obligatorios",
                    Code = 400
                });
            }

            int result = new Db().GuardarSucursal(sucursalRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Creación exitosa",
                    msg = "La sucursal se ha creado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al crear",
                msg = "No se pudo guardar la sucursal",
                Code = 400
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SucursalRequest sucursalRequest)
        {
            var sucursalExistente = new Db().ObtenerSucursalPorId(id);
            if (sucursalExistente == null)
            {
                return NotFound(new
                {
                    titulo = "Sucursal no encontrada",
                    msg = "No se encontró una sucursal con el ID proporcionado",
                    Code = 404
                });
            }

            int result = new Db().ActualizarSucursal(id, sucursalRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Actualización exitosa",
                    msg = "La sucursal se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "No se pudo actualizar la sucursal",
                Code = 400
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var sucursalExistente = new Db().ObtenerSucursalPorId(id);
            if (sucursalExistente == null)
            {
                return NotFound(new
                {
                    titulo = "Sucursal no encontrada",
                    msg = "No se encontró una sucursal con el ID proporcionado",
                    Code = 404
                });
            }

            int result = new Db().EliminarSucursal(id);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Eliminación exitosa",
                    msg = "La sucursal se ha eliminado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al eliminar",
                msg = "No se pudo eliminar la sucursal",
                Code = 400
            });
        }
    }
}
