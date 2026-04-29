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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = "HOLAAAAAA";
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if(txtUsuario.Text != "" && txtClave.Text != "")
            {
                if (ClaveValida(txtClave.Text))
                {
                    MessageBox.Show("Clave Válida.");
                }
                else
                {
                    MessageBox.Show("La clave no cumple los valores.");
                }
            }
            else { MessageBox.Show("Complete todos los campos."); }
        }

        public bool ClaveValida(string _c)
        {
            if (string.IsNullOrEmpty(_c)) return false;
            string patron = @"^(?=.*[A-Z])(?=.*\d).{6,}$";
            return Regex.IsMatch(_c, patron);

        }
    }
}
