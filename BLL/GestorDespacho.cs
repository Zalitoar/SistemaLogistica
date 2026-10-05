using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using Servicios;
namespace BLL
{
    public class GestorDespacho
    {
        public const string Permiso="TD_PREPARAR_DESPACHO";
        private readonly MP_DocumentoDespacho mapper=new MP_DocumentoDespacho();
        public List<Viaje> ObtenerViajes()
        {
            ReglasTD.Permiso(Permiso);
            return new MP_Viaje().Listar().Where(v => v.Estado=="Planificado" || v.Estado=="Preparado").ToList();
        }
        public List<CargaDespacho> ObtenerCarga(int viaje) { ReglasTD.Permiso(Permiso); return mapper.ObtenerCarga(viaje); }
        public DocumentoDespacho ObtenerDespacho(int viaje) { ReglasTD.Permiso(Permiso); return mapper.Obtener(viaje); }
        public static bool ValidarCarga(List<CargaDespacho> prevista, List<CargaDespacho> real, string observaciones)
        {
            ReglasTD.Exigir(prevista != null && prevista.Count > 0 && real != null && real.Count == prevista.Count);
            ReglasTD.Exigir(real.All(c => c != null && ReglasTD.Cantidad(c.CantidadPreparada,true)), "TD.CantidadPreparadaInvalida");
            ReglasTD.Exigir(real.Select(c => c.IdMercaderia).Distinct().Count()==real.Count);
            ReglasTD.Exigir(prevista.All(p => real.Any(r => r.IdMercaderia==p.IdMercaderia)));
            bool coincide=prevista.All(p => real.Single(r => r.IdMercaderia==p.IdMercaderia).CantidadPreparada==p.CantidadPrevista);
            ReglasTD.Exigir((observaciones ?? "").Length<=1000);
            ReglasTD.Exigir(coincide || ReglasTD.Texto(observaciones,1000), "TD.ExpliqueDiferencia");
            return coincide;
        }
        public DocumentoDespacho Guardar(int viaje, string numero, string observaciones, List<CargaDespacho> carga)
        {
            ReglasTD.Permiso(Permiso);
            ReglasTD.Exigir(ReglasTD.Texto(numero,50));
            var estado=new MP_Viaje().Listar().SingleOrDefault(v => v.IdViaje==viaje);
            ReglasTD.Exigir(estado != null && estado.Estado=="Planificado","TD.EstadoInvalido");
            ValidarCarga(mapper.ObtenerCarga(viaje),carga,observaciones);
            var despacho=mapper.Guardar(viaje,numero.Trim(),observaciones,carga);
            BitacoraManager.Registrar("TD despacho "+despacho.IdDespacho+" "+despacho.Estado+" V"+viaje);
            return despacho;
        }
    }
}
