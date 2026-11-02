using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using Servicios;
namespace BLL
{
    public class GestorViaje
    {
        public const string Permiso="TD_EJECUTAR_VIAJE";
        public List<Viaje> ObtenerViajes()
        {
            ReglasTD.Permiso(Permiso);
            return new MP_Viaje().Listar().Where(v => v.Estado!="Cerrado").ToList();
        }
        public List<Entrega> ObtenerEntregas(int viaje) { ReglasTD.Permiso(Permiso); return new MP_Entrega().Obtener(viaje); }
        public DocumentoDespacho ObtenerDespacho(int viaje) { ReglasTD.Permiso(Permiso); return new MP_DocumentoDespacho().Obtener(viaje); }
        public List<CargaDespacho> ObtenerCarga(int viaje) { ReglasTD.Permiso(Permiso); return new MP_DocumentoDespacho().ObtenerCarga(viaje); }
        public static bool PuedeIniciarse(Viaje viaje, DocumentoDespacho despacho, List<Entrega> entregas, List<CargaDespacho> carga)
        {
            return viaje!=null && viaje.Estado=="Preparado" && despacho!=null && despacho.Estado=="Confirmado"
                && despacho.IdViaje==viaje.IdViaje && entregas!=null && entregas.Count>0
                && carga!=null && carga.Count>0 && carga.All(c => c.CantidadPrevista>0 && c.CantidadPrevista==c.CantidadPreparada);
        }
        public void Iniciar(int id)
        {
            ReglasTD.Permiso(Permiso);
            var viaje=new MP_Viaje().Listar().SingleOrDefault(v => v.IdViaje==id);
            ReglasTD.Exigir(PuedeIniciarse(viaje,ObtenerDespacho(id),ObtenerEntregas(id),ObtenerCarga(id)),"TD.InicioNoHabilitado");
            new MP_Viaje().Iniciar(id);
            BitacoraManager.Registrar("TD: inicio de viaje "+id);
        }
    }
}
