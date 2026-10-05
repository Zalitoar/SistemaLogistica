using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE;
namespace DAL
{
    public class MP_Entrega
    {
        public void Registrar(int viaje,int entrega,string resultado,DateTime fecha,string observaciones)
        {
            MP_TD.Leer("TD_REGISTRAR_ENTREGA", MP_TD.Parametro("@IdViaje",viaje), MP_TD.Parametro("@IdEntrega",entrega),
                MP_TD.Parametro("@Resultado",resultado), MP_TD.Parametro("@Fecha",fecha), MP_TD.Parametro("@Observaciones",observaciones));
        }
        public List<Entrega> Obtener(int viaje)
        {
            return MP_TD.Leer("TD_OBTENER_ENTREGAS", MP_TD.Parametro("@IdViaje",viaje)).AsEnumerable().Select(r => new Entrega
            {
                IdEntrega=MP_TD.Id(r,"Id_Entrega"),IdViaje=viaje,Destino=(string)r["Destino"],FechaPrevista=(DateTime)r["FechaPrevista"],
                FechaRealizada=MP_TD.FechaOpcional(r,"FechaRealizada"),Estado=(string)r["Estado"],Observaciones=r["Observaciones"].ToString(),
                Comprobante=r.IsNull("Id_Comprobante") ? null : new ComprobanteEntrega {
                    IdComprobante=MP_TD.Id(r,"Id_Comprobante"),IdEntrega=MP_TD.Id(r,"Id_Entrega"),Numero=(string)r["NumeroComprobante"],
                    Fecha=(DateTime)r["FechaComprobante"],Resultado=(string)r["Resultado"],Observaciones=r["ObservacionesComprobante"].ToString()
                }
            }).ToList();
        }
    }
}
