using System;

namespace BE
{
    public class InformeDisponibilidad
    {
        public int IdInformeDisponibilidad { get; set; }
        public int IdUnidadFlota { get; set; }
        public int? IdIntervencionMantenimiento { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Disponible { get; set; }
        public string EstadoUnidad { get; set; }
        public string Observaciones { get; set; }
        public bool Vigente { get; set; }
    }
}
