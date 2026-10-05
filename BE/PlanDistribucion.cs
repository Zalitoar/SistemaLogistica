using System;
using System.Collections.Generic;

namespace BE
{
    public class PlanDistribucion
    {
        public int IdPlan { get; set; }
        public int IdRequerimiento { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public List<Viaje> Viajes { get; set; } = new List<Viaje>();
    }
}
