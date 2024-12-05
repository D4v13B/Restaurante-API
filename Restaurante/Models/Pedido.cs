using System;

namespace Restaurante.Models
{
   public class Pedido
   {
      public int Id { get; set; }
      public string Cliente { get; set; }
      public string Producto { get; set; }
      public string ProductoImg { get; set; }
      public double Value{get; set;}
      public int Quantity {get;set;}
   }
}
