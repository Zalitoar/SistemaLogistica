using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using Servicios;
namespace BLL
{
    public class GestorEntrega
    {
        public const string Permiso="TD_REGISTRAR_ENTREGA";
        public List<Viaje> ObtenerViajes()
        {
            ReglasTD.Permiso(Permiso);
            return new MP_Viaje().Listar().Where(v => v.Estado=="EnCurso").ToList();
        }
        public List<Entrega> ObtenerEntregas(int viaje) { ReglasTD.Permiso(Permiso); return new MP_Entrega().Obtener(viaje); }
        public static bool PuedeRegistrar(Viaje viaje, Entrega entrega)
        {
            return viaje!=null && viaje.Estado=="EnCurso" && viaje.FechaInicio.HasValue
                && entrega!=null && entrega.IdViaje==viaje.IdViaje && entrega.Estado=="Pendiente";
        }
        public static void ValidarResultado(Viaje viaje, Entrega entrega, string resultado, DateTime fecha, string observaciones)
        {
            ReglasTD.Exigir(PuedeRegistrar(viaje,entrega),"TD.EstadoInvalido");
            ReglasTD.Exigir(new[]{"Entregada","Parcial","Incumplida"}.Contains(resultado));
            ReglasTD.Exigir(ReglasTD.Fecha(fecha) && fecha>=viaje.FechaInicio.Value && fecha<=DateTime.Now,"TD.FechaResultado");
            ReglasTD.Exigir((observaciones ?? "").Length<=1000);
            ReglasTD.Exigir(resultado=="Entregada" || ReglasTD.Texto(observaciones,1000),"TD.ExpliqueResultado");
        }
        public void Registrar(int viajeId,int entregaId,string resultado,DateTime fecha,string observaciones)
        {
            ReglasTD.Permiso(Permiso);
            var viaje=new MP_Viaje().Listar().SingleOrDefault(v => v.IdViaje==viajeId);
            var entrega=new MP_Entrega().Obtener(viajeId).SingleOrDefault(e => e.IdEntrega==entregaId);
            ValidarResultado(viaje,entrega,resultado,fecha,observaciones);
            new MP_Entrega().Registrar(viajeId,entregaId,resultado,fecha,observaciones);
            BitacoraManager.Registrar("TD entrega "+entregaId+" "+resultado);
        }
    }
}
