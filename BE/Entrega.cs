using System;
using System.Collections.Generic;

namespace BE
{
    public class Entrega
    {
        public ComprobanteEntrega Comprobante { get; set; }
        public string NumeroComprobante { get { return Comprobante?.Numero; } }
        public int IdEntrega { get; set; }
        public int IdViaje { get; set; }
        public string Destino { get; set; }
        public DateTime FechaPrevista { get; set; }
        public DateTime? FechaRealizada { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
}
