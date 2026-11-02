using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace BLL
{
    public class GestorMantenimiento
    {
        public const string PermisoProgramar = "MF_PROGRAMAR_MANTENIMIENTO";
        public const string PermisoIntervenir = "MF_REGISTRAR_INTERVENCION";
        private readonly MP_MF datos = new MP_MF();
        public List<ParteEstadoUnidad> PartesPendientes()
        {
            ReglasMF.Permiso(PermisoProgramar);
            return datos.Partes().Where(p => p.RequiereMantenimiento && p.EstadoParte == "Pendiente").ToList();
        }

        public List<OrdenMantenimiento> OrdenesProgramacion()
        {
            ReglasMF.Permiso(PermisoProgramar);
            return datos.Ordenes();
        }

        public List<OrdenMantenimiento> Ordenes()
        {
            ReglasMF.Permiso(PermisoIntervenir);
            return datos.Ordenes();
        }

        public List<IntervencionMantenimiento> Intervenciones(int orden)
        {
            ReglasMF.Permiso(PermisoIntervenir);
            return datos.Intervenciones(orden);
        }

        public int Programar(OrdenMantenimiento o)
        {
            ReglasMF.Permiso(PermisoProgramar);
            ReglasMF.Exigir(o != null && o.IdParteEstadoUnidad > 0 && o.FechaProgramada >= DateTime.Today && o.FechaProgramada < new DateTime(9999, 12, 31) && new[] { "Preventivo", "Correctivo" }.Contains(o.TipoMantenimiento) && new[] { "Baja", "Media", "Alta", "Urgente" }.Contains(o.Prioridad) && ReglasMF.Texto(o.DescripcionTrabajo, 500));
            int id = datos.Programar(o);
            ReglasMF.Auditar("programacion orden", id);
            return id;
        }

        public int Iniciar(int orden)
        {
            ReglasMF.Permiso(PermisoIntervenir);
            ReglasMF.Exigir(orden > 0);
            int id = datos.Iniciar(orden);
            ReglasMF.Auditar("inicio intervencion", id);
            return id;
        }

        public void Finalizar(IntervencionMantenimiento i)
        {
            ReglasMF.Permiso(PermisoIntervenir);
            ReglasMF.Exigir(i != null && i.IdIntervencionMantenimiento > 0 && ReglasMF.Km(i.Kilometraje) && ReglasMF.Texto(i.TrabajoRealizado, 1000) && new[] { "Satisfactoria", "RequiereTareas" }.Contains(i.Resultado) && (i.Observaciones ?? "").Length <= 1000 && (i.Resultado != "RequiereTareas" || ReglasMF.Texto(i.Observaciones, 1000)));
            datos.Finalizar(i);
            ReglasMF.Auditar("fin intervencion", i.IdIntervencionMantenimiento);
        }
    }
}
