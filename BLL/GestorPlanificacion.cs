using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using Servicios;

namespace BLL
{
    public class GestorPlanificacion
    {
        public const string Permiso = "TD_PLANIFICAR_DISTRIBUCION";
        private readonly MP_PlanDistribucion mapper = new MP_PlanDistribucion();
        public List<RequerimientoDistribucion> ObtenerPendientes()
        {
            ReglasTD.Permiso(Permiso);
            return mapper.ListarPendientes();
        }
        public List<Mercaderia> ObtenerMercaderia()
        {
            ReglasTD.Permiso(Permiso);
            return mapper.ListarMercaderia();
        }
        public int RecibirRequerimiento(string numero, string origen)
        {
            ReglasTD.Permiso(Permiso);
            ReglasTD.Exigir(ReglasTD.Texto(numero, 50) && ReglasTD.Texto(origen, 200));
            return mapper.Recibir(numero.Trim(), origen.Trim());
        }
        public int CrearMercaderia(string codigo, string descripcion)
        {
            ReglasTD.Permiso(Permiso);
            ReglasTD.Exigir(ReglasTD.Texto(codigo, 50) && ReglasTD.Texto(descripcion, 200));
            return mapper.CrearMercaderia(codigo.Trim(), descripcion.Trim());
        }
        public void Validar(PlanDistribucion plan)
        {
            ReglasTD.Exigir(plan != null && plan.IdRequerimiento > 0 && ReglasTD.Texto(plan.Numero, 50));
            ReglasTD.Exigir(plan.Viajes != null && plan.Viajes.Count > 0, "TD.PlanIncompleto");
            ReglasTD.Exigir(plan.Viajes.All(v => v != null && ReglasTD.Texto(v.Numero, 50)));
            ReglasTD.Exigir(plan.Viajes.Select(v => v.Numero.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == plan.Viajes.Count, "TD.Duplicado");
            foreach (var viaje in plan.Viajes)
            {
                ReglasTD.Exigir(ReglasTD.Fecha(viaje.FechaPrevista));
                ReglasTD.Exigir(viaje.Entregas != null && viaje.Entregas.Count > 0 && viaje.CargaPlanificada != null && viaje.CargaPlanificada.Count > 0, "TD.PlanIncompleto");
                ReglasTD.Exigir(viaje.Entregas.All(e => e != null && ReglasTD.Texto(e.Destino, 200) && ReglasTD.Fecha(e.FechaPrevista) && e.FechaPrevista >= viaje.FechaPrevista), "TD.FechaEntrega");
                ReglasTD.Exigir(viaje.CargaPlanificada.All(c => c != null && c.IdMercaderia > 0 && ReglasTD.Cantidad(c.CantidadPrevista)), "TD.CantidadInvalida");
                ReglasTD.Exigir(viaje.CargaPlanificada.Select(c => c.IdMercaderia).Distinct().Count() == viaje.CargaPlanificada.Count, "TD.Duplicado");
            }
        }
        public int Confirmar(PlanDistribucion plan)
        {
            ReglasTD.Permiso(Permiso);
            Validar(plan);
            int id = mapper.Confirmar(plan);
            BitacoraManager.Registrar("TD plan creado/confirmado #" + id);
            return id;
        }
    }
}
