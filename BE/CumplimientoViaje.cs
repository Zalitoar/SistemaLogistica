using System.Collections.Generic;
namespace BE
{
    public class CumplimientoViaje
    {
        public Viaje Viaje { get; set; }
        public List<Entrega> Entregas { get; set; }
        public int Total { get; set; }
        public int Entregadas { get; set; }
        public int Parciales { get; set; }
        public int Incumplidas { get; set; }
        public int Pendientes { get; set; }
        public int Comprobantes { get; set; }
        public decimal Porcentaje { get; set; }
        public bool PuedeCerrar { get; set; }
    }
}
