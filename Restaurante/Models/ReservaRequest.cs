using System;

namespace Restaurante.Models;

public class ReservaRequest
{
   public string Nombre {get; set;}
   public string Telefono {get; set;}
   public DateTime FechaHoraReserva{get; set;}
}
