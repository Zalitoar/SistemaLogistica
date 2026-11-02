using System;
using System.Collections.Generic;

namespace BE
{
    public class DetalleDespacho
    {
        public int IdDetalle { get; set; }
        public int IdDespacho { get; set; }
        public int IdMercaderia { get; set; }
        public decimal Cantidad { get; set; }
    }
}
