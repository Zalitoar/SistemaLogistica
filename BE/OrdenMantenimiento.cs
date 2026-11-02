using System;

namespace BE
{
    public class OrdenMantenimiento
    {
        public int IdOrdenMantenimiento { get; set; }
        public int IdUnidadFlota { get; set; }
        public int IdParteEstadoUnidad { get; set; }
        public string Dominio { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaProgramada { get; set; }
        public string TipoMantenimiento { get; set; }
        public string Prioridad { get; set; }
        public string DescripcionTrabajo { get; set; }
        public string EstadoOrden { get; set; }
    }
}
