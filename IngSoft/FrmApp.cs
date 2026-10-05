using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace IngSoft
{
    public partial class FrmApp : FormularioTraducible
    {
        private bool cierreVoluntario = false;
        private bool salirAplicacion = false;

        public FrmApp()
        {
            InitializeComponent();
            InicializarTD();
        }

        private void App_Load(object sender, EventArgs e)
        {
            Servicios.SessionManager sesion = Servicios.SessionManager.GetInstance();
            
            if (sesion.GetUsuario() != null)
            {
                this.Text = "Sistema de Gestión - Usuario: " + sesion.GetUsuario().Nombre;
                ValidarPermiso();

            }

            // Poblar el único selector de idiomas de la ventana principal.
            try
            {
                if (this.toolStripComboBoxIdiomas != null)
                {
                    var listaMenu = IdiomaManager.GetInstance().ListarIdiomas();

                    // Usar la ComboBox interna del ToolStripComboBox para propiedades de datos
                    var inner = this.toolStripComboBoxIdiomas.ComboBox;
                    inner.DisplayMember = "Nombre";
                    inner.ValueMember = "Id_Idioma";
                    inner.DataSource = listaMenu;

                    var activo = IdiomaManager.GetInstance().GetIdiomaActual();
                    if (activo != null)
                    {
                        for (int i = 0; i < inner.Items.Count; i++)
                        {
                            var it = inner.Items[i] as BE.Idioma;
                            if (it != null && it.Id_Idioma == activo.Id_Idioma)
                            {
                                inner.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            catch { }

        }

        private void ValidarPermiso()
        {
            ValidarPermisosTD();
            clientesToolStripMenuItem.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("GESTION_VENTAS");
            productosToolStripMenuItem.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("ABM_CLIENTES");
            productosToolStripMenuItem1.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("ABM_PRODUCTOS");
            historialToolStripMenuItem.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("VER_HISTORIAL");
            usuariosToolStripMenuItem.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("ABM_USUARIOS");
            perfilesToolStripMenuItem.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("ABM_ROLES");
            bitácoraToolStripMenuItem.Enabled = Servicios.SessionManager.GetInstance().TienePermiso("VER_BITACORA");
            
        }

        // Nuevo handler para la lista en la cinta de menú
        private void toolStripComboBoxIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var sel = this.toolStripComboBoxIdiomas.SelectedItem as BE.Idioma;
                if (sel != null)
                {
                    IdiomaManager.GetInstance().SetIdiomaActual(sel);

                    // Si hay usuario en sesión, guardar preferencia
                    var usuario = SessionManager.GetInstance()?.GetUsuario();
                    if (usuario != null)
                    {
                        try
                        {
                            new BLL.Idioma().SetIdiomaPreferidoUsuario(usuario.Id_Usuario, sel.Id_Idioma);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private void FrmApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cierreVoluntario || salirAplicacion) return;

            DialogResult respuesta = MessageBox.Show(
                ObtenerTexto("FrmApp.msgConfirmarSalir", "¿Realmente desea cerrar sesión y salir?"),
                ObtenerTexto("FrmApp.msgTituloConfirmarSalida", "Confirmar salida"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                salirAplicacion = true;
                BitacoraManager.Registrar("Cierre de sesión");
                SessionManager.Logout();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show(
                ObtenerTexto("FrmApp.msgConfirmarCerrarSesion", "¿Realmente desea cerrar sesión?"),
                ObtenerTexto("FrmApp.msgTituloConfirmarSalida", "Confirmar salida"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                cierreVoluntario = true;
                BitacoraManager.Registrar("Cierre de sesión");
                SessionManager.Logout();

                FrmLogin login = Application.OpenForms
                    .OfType<FrmLogin>()
                    .FirstOrDefault();

                this.Close();

                if (login != null)
                    login.PrepararNuevoIngreso();
                else
                    new FrmLogin().Show();
            }
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmABMUsuarios frmu = new FrmABMUsuarios();
            frmu.MdiParent = this;
            frmu.Show();
        }

        
        private void perfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           FrmABMRoles frmRol = new FrmABMRoles();
            frmRol.MdiParent = this;
            frmRol.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void bitácoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmBitacora frmb = new FrmBitacora();
            frmb.MdiParent = this;
            frmb.Show();
        }

        // IIdiomaObserver
        public override void ActualizarIdioma(BE.Idioma nuevoIdioma)
        {
            base.ActualizarIdioma(nuevoIdioma);

            if (nuevoIdioma == null)
                return;

            if (toolStripComboBoxIdiomas == null || IsDisposed || Disposing)
                return;

            for (int i = 0; i < toolStripComboBoxIdiomas.Items.Count; i++)
            {
                BE.Idioma item = toolStripComboBoxIdiomas.Items[i] as BE.Idioma;
                if (item != null && item.Id_Idioma == nuevoIdioma.Id_Idioma)
                {
                    toolStripComboBoxIdiomas.SelectedIndex = i;
                    break;
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Salir después de que FrmApp terminó de destruir sus controles.
            // Esto evita abortar el WndProc activo del ToolStripComboBox.
            if (salirAplicacion)
                Application.Exit();
        }

        // Modificado: abre FrmIdioma como ventana MDI
        private void idiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmIdioma frm = new FrmIdioma();
                frm.MdiParent = this;
                frm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ObtenerTexto("FrmApp.msgErrorAbrirIdiomas", "No se pudo abrir el formulario de idiomas.") + " " + ex.Message,
                    ObtenerTexto("Common.msgError", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void idiomasToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }
    }
}
