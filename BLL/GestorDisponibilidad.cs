using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace BLL
{
    public class GestorDisponibilidad
    {
        public const string PermisoHabilitar = "MF_HABILITAR_UNIDAD";
        public const string PermisoAsignar = "MF_ASIGNAR_UNIDAD";
        private readonly MP_MF datos = new MP_MF();
        public List<UnidadFlota> Unidades()
        {
            ReglasMF.Permiso(PermisoHabilitar);
            return datos.Unidades();
        }

        public List<OrdenMantenimiento> Ordenes(int unidad)
        {
            ReglasMF.Permiso(PermisoHabilitar);
            return datos.Ordenes(unidad);
        }

        public List<InformeDisponibilidad> Informes(int unidad)
        {
            ReglasMF.Permiso(PermisoHabilitar);
            return datos.Informes(unidad);
        }

        public List<IntervencionMantenimiento> Intervenciones(int unidad)
        {
            ReglasMF.Permiso(PermisoHabilitar);
            return datos.Ordenes(unidad).SelectMany(o => datos.Intervenciones(o.IdOrdenMantenimiento)).ToList();
        }

        public int Habilitar(int unidad, string observaciones)
        {
            ReglasMF.Permiso(PermisoHabilitar);
            ReglasMF.Exigir(unidad > 0 && (observaciones ?? "").Length <= 1000);
            int id = datos.Habilitar(unidad, observaciones);
            ReglasMF.Auditar("habilitacion informe", id);
            return id;
        }

        public List<Viaje> Viajes()
        {
            ReglasMF.Permiso(PermisoAsignar);
            return new MP_Viaje().Listar().Where(v => v.Estado == "Planificado" || v.Estado == "Preparado").ToList();
        }

        public List<InformeDisponibilidad> InformesDisponibles()
        {
            ReglasMF.Permiso(PermisoAsignar);
            return datos.Informes().Where(i => i.Vigente && i.Disponible).ToList();
        }

        public List<UnidadFlota> Disponibles(int? viaje = null)
        {
            ReglasMF.Permiso(PermisoAsignar);
            var ocupadas = new MP_Viaje().Listar().Where(v => v.Estado != "Cerrado" && v.IdViaje != viaje).Select(v => v.IdUnidadFlota).ToList();
            var informes = InformesDisponibles().Select(i => i.IdUnidadFlota).ToList();
            var pendientes = datos.Partes().Where(p => p.EstadoParte != "Resuelto").Select(p => p.IdUnidadFlota).ToList();
            return datos.Unidades().Where(u => u.Activa && u.EstadoOperativo == "Disponible" && informes.Contains(u.IdUnidadFlota) && !ocupadas.Contains(u.IdUnidadFlota) && !pendientes.Contains(u.IdUnidadFlota)).ToList();
        }

        public void Asignar(int unidad, int viaje)
        {
            ReglasMF.Permiso(PermisoAsignar);
            ReglasMF.Exigir(unidad > 0 && viaje > 0);
            datos.Asignar(unidad, viaje);
            ReglasMF.Auditar("unidad a viaje", viaje);
        }
    }
}
