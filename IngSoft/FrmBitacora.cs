using BE;
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
    public partial class FrmBitacora : Form
    {
        public FrmBitacora()
        {
            InitializeComponent();
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            cbUsuarios.DataSource = BitacoraManager.ListarUsuariosAuditados();
            cbUsuarios.SelectedIndex = -1;        
            
            dgvBitacora.DataSource = null;
            dgvBitacora.DataSource = BitacoraManager.FiltrarBitacora("", DateTime.MinValue, DateTime.MaxValue);
        }

        private void gbFiltros_Enter(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

            List<Bitacora> Resultado = BitacoraManager.FiltrarBitacora(cbUsuarios.Text, dtpDesde.Value, dtpHasta.Value);
            dgvBitacora.DataSource = null;
            dgvBitacora.DataSource = Resultado;

        }
    }
}
