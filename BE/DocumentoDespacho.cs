using System;
using System.Collections.Generic;

namespace BE
{
    public class DocumentoDespacho
    {
        public int IdDespacho { get; set; }
        public int IdViaje { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public List<DetalleDespacho> Detalles { get; set; } = new List<DetalleDespacho>();
    }
}
