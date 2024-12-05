using Microsoft.AspNetCore.Mvc;
using Restaurante.Datos;
using Restaurante.Models;
using System.Data;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        [HttpGet("porProducto")]
        public DataTable GetTotalesXProducto()
        {
            return new Db().GetTotalesFacturadosPorProducto();
        }
        [HttpGet("PorMetodoPago")]
        public DataTable GetTotalesXMetodoPago()
        {
            return new Db().GetCantidadVendidaPorMetodoPago();
        }
        [HttpGet("Top10/{fechaInicio}/{fechaFin}")]
        public DataTable GetTopXFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return new Db().GetTopProductosMasVendidos(fechaInicio, fechaFin);
        }
    }
}
