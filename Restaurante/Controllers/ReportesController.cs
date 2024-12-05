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
        public List<TotalFacturadoXProducto> GetTotalesXProducto()
        {
            return new Db().GetTotalesFacturadosPorProducto();
        }
        [HttpGet("PorMetodoPago")]
        public List<CantXMetodoPago> GetTotalesXMetodoPago()
        {
            return new Db().GetCantidadVendidaPorMetodoPago();
        }
        [HttpGet("Top10/{fechaInicio}/{fechaFin}")]
        public List<TopProductosVendidos> GetTopXFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return new Db().GetTopProductosMasVendidos(fechaInicio, fechaFin);
        }
    }
}
