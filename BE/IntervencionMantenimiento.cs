using System;

namespace BE
{
    public class IntervencionMantenimiento
    {
        public int IdIntervencionMantenimiento { get; set; }
        public int IdOrdenMantenimiento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal Kilometraje { get; set; }
        public string TrabajoRealizado { get; set; }
        public string Resultado { get; set; }
        public string Observaciones { get; set; }
    }
}
