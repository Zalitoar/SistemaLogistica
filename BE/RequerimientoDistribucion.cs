using System;
using System.Collections.Generic;

namespace BE
{
    public class RequerimientoDistribucion
    {
        public int IdRequerimiento { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Origen { get; set; }
        public string Estado { get; set; }
    }
}
