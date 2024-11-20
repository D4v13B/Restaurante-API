using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.Datos;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] EmpresaRequest empresaRequest)
        {
            if (string.IsNullOrEmpty(empresaRequest.Nombre) || string.IsNullOrEmpty(empresaRequest.Direccion))
            {
                return BadRequest(new
                {
                    titulo = "Error al actualizar",
                    msg = "Los campos 'Nombre' y 'Dirección' son obligatorios",
                    Code = 400
                });
            }

            int result = new Db().UpdateEmpresa(id, empresaRequest);
            if (result > 0)
            {
                return Ok(new
                {
                    titulo = "Actualización correcta",
                    msg = "La empresa se ha actualizado exitosamente",
                    Code = 200
                });
            }

            return BadRequest(new
            {
                titulo = "Error al actualizar",
                msg = "Hubo un problema al actualizar la empresa",
                Code = 400
            });
        }
    }
}
