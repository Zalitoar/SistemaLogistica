using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE;
namespace DAL
{
    public class MP_Viaje
    {
        public void Cerrar(int viaje) { MP_TD.Leer("TD_CERRAR_VIAJE", MP_TD.Parametro("@IdViaje",viaje)); }
        public void Iniciar(int viaje) { MP_TD.Leer("TD_INICIAR_VIAJE", MP_TD.Parametro("@IdViaje",viaje)); }
        public List<Viaje> Listar()
        {
            return MP_TD.Leer("TD_LISTAR_VIAJES").AsEnumerable().Select(r => new Viaje
            {
                IdUnidadFlota=r.Field<int?>("IdUnidadFlota"), IdInformeDisponibilidad=r.Field<int?>("IdInformeDisponibilidad"),
                IdViaje=MP_TD.Id(r,"Id_Viaje"), IdPlan=MP_TD.Id(r,"Id_Plan"), Numero=(string)r["Numero"],
                FechaPrevista=(DateTime)r["FechaPrevista"], FechaInicio=MP_TD.FechaOpcional(r,"FechaInicio"),
                FechaFinalizacion=MP_TD.FechaOpcional(r,"FechaFinalizacion"), Estado=(string)r["Estado"],
                PorcentajeCumplimiento=(decimal)r["PorcentajeCumplimiento"]
            }).ToList();
        }
    }
}
