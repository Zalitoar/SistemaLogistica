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
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private void App_Load(object sender, EventArgs e)
        {
            BLL.SessionManager sesion = BLL.SessionManager.GetInstance();

            if(sesion.GetUsuario() != null)
            {
                this.Text = "Sistema de Gestión - Usuario: " + sesion.GetUsuario().Nombre;
            }
        }
    }
}
