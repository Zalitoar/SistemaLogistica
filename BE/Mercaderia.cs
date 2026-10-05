using System;
using System.Collections.Generic;

namespace BE
{
    public class Mercaderia
    {
        public string Etiqueta { get { return Codigo + " - " + Descripcion; } }
        public int IdMercaderia { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
