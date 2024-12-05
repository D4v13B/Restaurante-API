using System;
using System.Collections.Generic;

namespace Heiwa.Models
{
    public class OrdenRequest
    {
        public DateTime? Fecha { get; set; }
        public string ObservacionCliente { get; set; }
        public int MetodoPagoId { get; set; }
        public int OrdenEstadoId { get; set; }
        public int UsuarioId { get; set; }

        public List<DetalleOrdenRequest> DetalleOrden { get; set; }
    }
}
