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
    public partial class FrmApp : Form
    {

        private bool cierreVoluntario = false;
        public FrmApp()
        {
            InitializeComponent();
        }

        private void App_Load(object sender, EventArgs e)
        {
            Servicios.SessionManager sesion = Servicios.SessionManager.GetInstance();

            if(sesion.GetUsuario() != null)
            {
                this.Text = "Sistema de Gestión - Usuario: " + sesion.GetUsuario().Nombre;
            }
        }

        private void FrmApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cierreVoluntario) return;

            DialogResult respuesta = MessageBox.Show("¿Realmente desea cerrar sesión y salir?", "Confirmar Salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(respuesta == DialogResult.Yes)
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
            DialogResult respuesta = MessageBox.Show("¿Realmente desea cerrar sesión?", "Confirmar Salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void bitácoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmBitacora frmb = new FrmBitacora();
            frmb.MdiParent = this;
            frmb.Show();
        }

        private void perfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmABMRoles frmu = new FrmABMRoles();
            frmu.MdiParent = this;
            frmu.Show();
        }
    }
}
