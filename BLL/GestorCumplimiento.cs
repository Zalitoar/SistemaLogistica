using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using Servicios;
namespace BLL
{
    public class GestorCumplimiento
    {
        public const string Permiso="TD_CONTROLAR_CUMPLIMIENTO";
        public List<Viaje> ObtenerViajes() { ReglasTD.Permiso(Permiso); return new MP_Viaje().Listar(); }
        public static CumplimientoViaje Calcular(Viaje viaje, List<Entrega> entregas)
        {
            ReglasTD.Exigir(viaje!=null && entregas!=null);
            var resumen=new CumplimientoViaje {
                Viaje=viaje, Entregas=entregas, Total=entregas.Count,
                Entregadas=entregas.Count(e => e.Estado=="Entregada"),
                Parciales=entregas.Count(e => e.Estado=="Parcial"),
                Incumplidas=entregas.Count(e => e.Estado=="Incumplida"),
                Pendientes=entregas.Count(e => e.Estado=="Pendiente"),
                Comprobantes=entregas.Count(e => e.Comprobante!=null)
            };
            resumen.Porcentaje=resumen.Total==0 ? 0 : Math.Round(100m*resumen.Entregadas/resumen.Total,2,MidpointRounding.AwayFromZero);
            resumen.PuedeCerrar=viaje.Estado=="EnCurso" && resumen.Total>0 && resumen.Entregadas==resumen.Total && resumen.Comprobantes==resumen.Total;
            return resumen;
        }
        public CumplimientoViaje Analizar(int id)
        {
            ReglasTD.Permiso(Permiso);
            var viaje=new MP_Viaje().Listar().SingleOrDefault(v => v.IdViaje==id);
            ReglasTD.Exigir(viaje!=null,"TD.Seleccionar");
            return Calcular(viaje,new MP_Entrega().Obtener(id));
        }
        public void Cerrar(int id)
        {
            ReglasTD.Permiso(Permiso);
            ReglasTD.Exigir(Analizar(id).PuedeCerrar,"TD.CierreNoHabilitado");
            new MP_Viaje().Cerrar(id);
            BitacoraManager.Registrar("TD cierre de viaje "+id);
        }
    }
}
