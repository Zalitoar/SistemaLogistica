using System;

namespace BE
{
    public class ParteEstadoUnidad
    {
        public int IdParteEstadoUnidad { get; set; }
        public int IdUnidadFlota { get; set; }
        public string Dominio { get; set; }
        public DateTime FechaHora { get; set; }
        public decimal Kilometraje { get; set; }
        public string TipoNovedad { get; set; }
        public string Descripcion { get; set; }
        public bool RequiereMantenimiento { get; set; }
        public string EstadoParte { get; set; }
    }
}
