using System;

namespace Restaurante.Models;

public class Reserva
{
   public int Id{get; set;}
   public string Nombre {get; set;}
   public string Telefono {get; set;}
   public DateTime FechaHoraReserva{get; set;}
}
