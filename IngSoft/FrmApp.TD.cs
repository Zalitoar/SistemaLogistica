using System;
using System.Windows.Forms;
using Servicios;

namespace IngSoft
{
    public partial class FrmApp
    {
        private ToolStripMenuItem menuTD;
        private void InicializarTD()
        {
            menuTD = new ToolStripMenuItem { Name = "menuTD", Text = "Transporte y distribución" };
            menuStrip1.Items.Insert(2, menuTD);
            AgregarTD("menuTD01", "Planificar distribución", BLL.GestorPlanificacion.Permiso, () => new FrmPlanificacionDistribucion());
            AgregarTD("menuTD02", "Preparar despacho", BLL.GestorDespacho.Permiso, () => new FrmPrepararDespacho());
            AgregarTD("menuTD03", "Registrar ejecución de viaje", BLL.GestorViaje.Permiso, () => new FrmEjecucionViaje());
            AgregarTD("menuTD04", "Registrar entrega", BLL.GestorEntrega.Permiso, () => new FrmRegistroEntrega());
            AgregarTD("menuTD05", "Controlar cumplimiento de viaje", BLL.GestorCumplimiento.Permiso, () => new FrmCumplimientoViaje());
        }
        private void AgregarTD<T>(string nombre, string titulo, string permiso, Func<T> crear) where T : FormularioTraducible
        {
            var item = new ToolStripMenuItem { Name = nombre, Text = titulo, Tag = permiso, Enabled = false };
            item.Click += (s, e) => TDPresentacion.Ejecutar(this, () =>
            {
                if (SessionManager.GetInstance()?.TienePermiso(permiso) != true)
                    throw new BE.ReglaTDException("TD.SinPermiso");
                AbrirMdi(crear);
            }, ObtenerTexto);
            menuTD.DropDownItems.Add(item);
        }
        private void ValidarPermisosTD()
        {
            bool alguno = false;
            foreach (ToolStripMenuItem item in menuTD.DropDownItems)
            {
                item.Enabled = SessionManager.GetInstance()?.TienePermiso((string)item.Tag) == true;
                alguno |= item.Enabled;
            }
            menuTD.Enabled = alguno;
        }
    }
}
