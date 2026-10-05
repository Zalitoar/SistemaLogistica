using Servicios;
using System.Windows.Forms;

namespace IngSoft
{
    public partial class FrmApp
    {
        private ToolStripMenuItem menuConfiguracionBD;
        private void InicializarConfiguracionBD()
        {
            menuConfiguracionBD=new ToolStripMenuItem { Name="menuConfiguracionBD", Text="Configurar base de datos", Enabled=false };
            menuStrip1.Items.Add(menuConfiguracionBD);
            menuConfiguracionBD.Click += (s,e) =>
            {
                if (SessionManager.GetInstance()?.TienePermiso(ConfiguracionBDManager.Permiso)!=true) return;
                using (var formulario=new FrmConfiguracionBD(false))
                {
                    if (formulario.ShowDialog(this)!=DialogResult.OK) return;
                    salirAplicacion=true;
                    SessionManager.Logout();
                    Application.Restart();
                }
            };
        }
    }
}
