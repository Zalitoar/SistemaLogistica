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

namespace IngSoft
{
    public partial class frmRestore : Form
    {
        public frmRestore()
        {
            InitializeComponent();
        }

        private void frmRestore_Load(object sender, EventArgs e)
        {
            bool integridadDVV = GestorIntegridad.ValidarIntegridadDVV();
            List<BE.Usuario> registrosDVH = GestorIntegridad.ValidarIntegridadDVH();

            if (!integridadDVV)
            {
                lblIntegridadDVV.Text = "Integridad fallida de tabla.";
            }
            else
            {
                lblIntegridadDVV.Text = "Integridad correcta de tabla.";
            }

            if (registrosDVH.Count > 0)
            {
                lblRegistros.Text = "Integridad fallida de registros.";
                dgvRegistros.DataSource = registrosDVH;
            }
            else
            {
                lblRegistros.Text = "Integridad correcta de registros.";
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            int ok = GestorIntegridad.Restore();
            if (ok == 1)
            {
                MessageBox.Show("Restauración completada con éxito.");
                BitacoraManager.Registrar("Se restauró la base de datos desde el backup.");
                SessionManager.Logout();
                this.Close();
                FrmLogin login = new FrmLogin();
                login.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error al restaurar la base de datos.");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            List<BE.Usuario> conError = GestorIntegridad.ValidarIntegridadDVH();
            BLL.Usuario bllUsuario = new BLL.Usuario();

            foreach (var u in conError)
            {
                u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Rol}|{u.Borrado}");
                bllUsuario.Grabar(u); // ya recalcula y persiste el DVV adentro
            }

            BitacoraManager.Registrar("Se recalcularon los dígitos verificadores de Usuario.");
            MessageBox.Show("Dígitos verificadores recalculados.");

            SessionManager.Logout();
            this.Close();
            FrmLogin login = new FrmLogin();
            login.ShowDialog();


        }
    }
}
