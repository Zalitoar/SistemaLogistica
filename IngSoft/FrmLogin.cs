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
        private bool integridadok = true;
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
            ValidarIntegridad();
        }

        private void ValidarIntegridad()
        {        
            //Validación DVV
            string nuevoCalculoDvv = GestorIntegridad.Calcular();
            List<BE.DVVUsuario> listaDVV = GestorIntegridad.Listar();
            if (nuevoCalculoDvv != listaDVV[0].Valor_DVV)
            {                
                integridadok = false;
            }
            //Validación DVH
            if(GestorIntegridad.ValidarIntegridadDVH().Count > 0)
            {
                integridadok = false;
            }

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {            
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show("Debe completar todos los campos.");
                return; 
            }            

            BLL.Usuario bllu = new BLL.Usuario();
            BE.Usuario usuario = bllu.ValidarIngreso(txtUsuario.Text, txtClave.Text);

            if (usuario == null)
            {
                MessageBox.Show("Usuario y/o Clave incorrecta o inexistente.");
                return;
            }

            if (!integridadok && usuario.Id_Rol != 11) // 11 = Id_Permiso de "Administrador" en PERMISO (hardcodeado por ahora, revisar cuando tengamos BLL.Rol)
            {
                MessageBox.Show("El sistema no esta disponible en este momento. Por favor, intente más tarde.");
                return;
            }

            if (!integridadok)
            {
                Servicios.SessionManager.Login(usuario);
                BitacoraManager.Registrar("Inicio de Sesión");
                List<string> permisos = new BLL.Rol().ObtenerArbol(usuario.Id_Rol).ObtenerPermisos().Select(p => p.Nombre_Permiso).ToList();
                Servicios.SessionManager.SetPermisos(permisos);
                MessageBox.Show("Ingreso exitoso.");
                frmRestore res = new frmRestore();
                res.Show();
                this.Hide();
            }else
            {
                Servicios.SessionManager.Login(usuario);
                BitacoraManager.Registrar("Inicio de Sesión");
                List<string> permisos = new BLL.Rol().ObtenerArbol(usuario.Id_Rol).ObtenerPermisos().Select(p => p.Nombre_Permiso).ToList();
                Servicios.SessionManager.SetPermisos(permisos);
                MessageBox.Show("Ingreso exitoso.");
                FrmApp App = new FrmApp();
                App.Show();
                this.Hide();
            }                
        }
    }
}
