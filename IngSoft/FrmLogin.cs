using BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IngSoft
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text != "" && txtClave.Text != "")
            {
                BLL.Usuario bllu = new BLL.Usuario();
                BE.Usuario usuario = bllu.ValidarIngreso(txtUsuario.Text, txtClave.Text);

                if (usuario != null)
                {
                    Servicios.SessionManager.Login(usuario);
                    BitacoraManager.Registrar("Inicio de Sesión");
                    MessageBox.Show("Ingreso exitoso.");
                    FrmApp App = new FrmApp();
                    App.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario y/o Clave incorrecta o inexistente.");
                }
            }                   
        }

        //public bool ClaveValida(string _c)
        //{
        //    if (string.IsNullOrEmpty(_c)) return false;
        //    string patron = @"^(?=.*[A-Z])(?=.*\d).{6,}$";
        //    return Regex.IsMatch(_c, patron);

        //}
    }
}
