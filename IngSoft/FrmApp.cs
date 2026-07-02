using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Servicios;

namespace IngSoft
{
    public partial class FrmApp : FormularioTraducible
    {
        private bool cierreVoluntario = false;
        private ComboBox cmbIdiomasApp;

        public FrmApp()
        {
            InitializeComponent();
        }

        private void App_Load(object sender, EventArgs e)
        {
            Servicios.SessionManager sesion = Servicios.SessionManager.GetInstance();
            


            if (sesion.GetUsuario() != null)
            {
                this.Text = "Sistema de Gestión - Usuario: " + sesion.GetUsuario().Nombre;
            }

            AgregarComboIdiomas();

            // Poblamos también el ToolStripComboBox (lista en la cinta) si existe
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

        private void AgregarComboIdiomas()
        {
            if (cmbIdiomasApp != null) return;

            cmbIdiomasApp = new ComboBox();
            cmbIdiomasApp.Name = "cmbIdiomasApp";
            cmbIdiomasApp.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIdiomasApp.Width = 160;
            cmbIdiomasApp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbIdiomasApp.Location = new Point(Math.Max(8, this.ClientSize.Width - cmbIdiomasApp.Width - 8), 4);
            cmbIdiomasApp.SelectionChangeCommitted += CmbIdiomasApp_SelectedIndexChanged;

            this.Controls.Add(cmbIdiomasApp);

            try
            {
                var lista = IdiomaManager.GetInstance().ListarIdiomas();
                cmbIdiomasApp.DisplayMember = "Nombre";
                cmbIdiomasApp.ValueMember = "Id_Idioma";
                cmbIdiomasApp.DataSource = lista;

                var activo = IdiomaManager.GetInstance().GetIdiomaActual();
                if (activo != null)
                {
                    for (int i = 0; i < cmbIdiomasApp.Items.Count; i++)
                    {
                        var it = cmbIdiomasApp.Items[i] as BE.Idioma;
                        if (it != null && it.Id_Idioma == activo.Id_Idioma)
                        {
                            cmbIdiomasApp.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        private void CmbIdiomasApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var sel = cmbIdiomasApp.SelectedItem as BE.Idioma;
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

        // Nuevo handler para la lista en la cinta de menú
        private void toolStripComboBoxIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Usar la ComboBox interna para obtener el SelectedItem
                var sel = this.toolStripComboBoxIdiomas.ComboBox.SelectedItem as BE.Idioma;
                if (sel != null)
                {
                    IdiomaManager.GetInstance().SetIdiomaActual(sel);

                    // Sincronizar el combo de la esquina si existe
                    if (cmbIdiomasApp != null)
                    {
                        for (int i = 0; i < cmbIdiomasApp.Items.Count; i++)
                        {
                            var it = cmbIdiomasApp.Items[i] as BE.Idioma;
                            if (it != null && it.Id_Idioma == sel.Id_Idioma)
                            {
                                cmbIdiomasApp.SelectedIndex = i;
                                break;
                            }
                        }
                    }

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
            if (cierreVoluntario) return;

            DialogResult respuesta = MessageBox.Show(
                ObtenerTexto("FrmApp.msgConfirmarSalir", "¿Realmente desea cerrar sesión y salir?"),
                ObtenerTexto("FrmApp.msgTituloConfirmarSalida", "Confirmar salida"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                BitacoraManager.Registrar("Cierre de sesión");
                SessionManager.Logout();
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                this.Close();
                FrmLogin login = new FrmLogin();
                login.ShowDialog();
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

            if (cmbIdiomasApp != null)
                cmbIdiomasApp.SelectedValue = nuevoIdioma.Id_Idioma;

            if (toolStripComboBoxIdiomas != null)
                toolStripComboBoxIdiomas.ComboBox.SelectedValue = nuevoIdioma.Id_Idioma;
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
