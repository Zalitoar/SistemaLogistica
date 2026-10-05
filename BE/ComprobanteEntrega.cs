using System;
using System.Collections.Generic;

namespace BE
{
    public class ComprobanteEntrega
    {
        public int IdComprobante { get; set; }
        public int IdEntrega { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Resultado { get; set; }
        public string Observaciones { get; set; }
    }
}
