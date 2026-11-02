using System;
using System.Windows.Forms;
using Servicios;
using BLL;

namespace IngSoft
{
    public partial class FrmApp
    {
        private ToolStripMenuItem menuMF;
        private void InicializarMF()
        {
            menuMF = new ToolStripMenuItem
            {
                Name = "menuMF",
                Text = "Mantenimiento y flota"
            };
            menuStrip1.Items.Insert(3, menuMF);
            AgregarMF("menuMF01", "Registrar estado de flota", GestorFlota.Permiso, () => new FrmEstadoFlota());
            AgregarMF("menuMF02", "Programar mantenimiento", GestorMantenimiento.PermisoProgramar, () => new FrmProgramarMantenimiento());
            AgregarMF("menuMF03", "Registrar intervención de mantenimiento", GestorMantenimiento.PermisoIntervenir, () => new FrmIntervencionMantenimiento());
            AgregarMF("menuMF04", "Habilitar unidad para operar", GestorDisponibilidad.PermisoHabilitar, () => new FrmHabilitarUnidad());
            AgregarMF("menuMF05", "Consultar disponibilidad y asignar unidad", GestorDisponibilidad.PermisoAsignar, () => new FrmDisponibilidadFlota());
        }

        private void AgregarMF<T>(string nombre, string titulo, string permiso, Func<T> crear)
            where T : FormularioTraducible
        {
            var item = new ToolStripMenuItem
            {
                Name = nombre,
                Text = titulo,
                Tag = permiso,
                Enabled = false
            };
            item.Click += (s, e) => MFPresentacion.Ejecutar(this, () =>
            {
                if (SessionManager.GetInstance()?.TienePermiso(permiso) != true)
                    throw new BE.ReglaMFException("MF.SinPermiso");
                AbrirMdi(crear);
            }, ObtenerTexto);
            menuMF.DropDownItems.Add(item);
        }

        private void ValidarPermisosMF()
        {
            bool alguno = false;
            foreach (ToolStripMenuItem item in menuMF.DropDownItems)
            {
                item.Enabled = SessionManager.GetInstance()?.TienePermiso((string)item.Tag) == true;
                alguno |= item.Enabled;
            }

            menuMF.Enabled = alguno;
        }
    }
}
