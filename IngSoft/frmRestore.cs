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
            }
            else
            {
                MessageBox.Show("Error al restaurar la base de datos.");
            }
        }
    }
}
