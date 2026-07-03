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
    public partial class frmRestore : FormularioTraducible
    {
        private bool? integridadDvvCorrecta;
        private bool? integridadRegistrosCorrecta;

        public frmRestore()
        {
            InitializeComponent();
        }

        private void frmRestore_Load(object sender, EventArgs e)
        {
            bool integridadDVV = GestorIntegridad.ValidarIntegridadDVV();
            List<BE.Usuario> registrosDVH = GestorIntegridad.ValidarIntegridadDVH();

            integridadDvvCorrecta = integridadDVV;
            integridadRegistrosCorrecta = registrosDVH.Count == 0;

            if (!integridadDVV)
            {
                lblIntegridadDVV.Text = ObtenerTexto("frmRestore.msgIntegridadDVVFallida", "Integridad fallida de tabla.");
            }
            else
            {
                lblIntegridadDVV.Text = ObtenerTexto("frmRestore.msgIntegridadDVVCorrecta", "Integridad correcta de tabla.");
            }

            if (registrosDVH.Count > 0)
            {
                lblRegistros.Text = ObtenerTexto("frmRestore.msgIntegridadRegistrosFallida", "Integridad fallida de registros.");
                dgvRegistros.DataSource = registrosDVH;
            }
            else
            {
                lblRegistros.Text = ObtenerTexto("frmRestore.msgIntegridadRegistrosCorrecta", "Integridad correcta de registros.");
            }
        }

        public override void ActualizarIdioma(BE.Idioma nuevoIdioma)
        {
            base.ActualizarIdioma(nuevoIdioma);

            if (integridadDvvCorrecta.HasValue)
            {
                lblIntegridadDVV.Text = integridadDvvCorrecta.Value
                    ? ObtenerTexto("frmRestore.msgIntegridadDVVCorrecta", "Integridad correcta de tabla.")
                    : ObtenerTexto("frmRestore.msgIntegridadDVVFallida", "Integridad fallida de tabla.");
            }

            if (integridadRegistrosCorrecta.HasValue)
            {
                lblRegistros.Text = integridadRegistrosCorrecta.Value
                    ? ObtenerTexto("frmRestore.msgIntegridadRegistrosCorrecta", "Integridad correcta de registros.")
                    : ObtenerTexto("frmRestore.msgIntegridadRegistrosFallida", "Integridad fallida de registros.");
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            int ok = GestorIntegridad.Restore();
            if (ok == 1)
            {
                MessageBox.Show(ObtenerTexto("frmRestore.msgRestauracionExitosa", "Restauración completada con éxito."));
                //VolverAlLogin();
            }
            else
            {
                MessageBox.Show(ObtenerTexto("frmRestore.msgRestauracionError", "Error al restaurar la base de datos."));
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

            //VolverAlLogin();
        }

        private void VolverAlLogin()
        {
            SessionManager.Logout();
            FrmLogin login = Application.OpenForms
                .OfType<FrmLogin>()
                .FirstOrDefault();

            Close();

            if (login != null)
                login.PrepararNuevoIngreso();
            else
                new FrmLogin().Show();
        }
    }
}
