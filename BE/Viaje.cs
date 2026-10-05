using System;
using System.Collections.Generic;

namespace BE
{
    public class Viaje
    {
        public int IdViaje { get; set; }
        public int? IdUnidadFlota { get; set; }
        public int? IdInformeDisponibilidad { get; set; }
        public int IdPlan { get; set; }
        public string Numero { get; set; }
        public DateTime FechaPrevista { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string Estado { get; set; }
        public decimal PorcentajeCumplimiento { get; set; }
        public List<Entrega> Entregas { get; set; } = new List<Entrega>();
        public List<DetalleCargaPlanificada> CargaPlanificada { get; set; } = new List<DetalleCargaPlanificada>();
    }
}
