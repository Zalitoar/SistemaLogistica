using System;
using System.Collections.Generic;

namespace BE
{
    public class DetalleCargaPlanificada
    {
        public int IdDetalleCargaPlanificada { get; set; }
        public int IdViaje { get; set; }
        public int IdMercaderia { get; set; }
        public decimal CantidadPrevista { get; set; }
    }
}
