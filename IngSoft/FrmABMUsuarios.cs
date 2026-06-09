using BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace IngSoft
{
    public partial class FrmABMUsuarios : Form
    {
        public FrmABMUsuarios()
        {
            InitializeComponent();
        }

        private void btnlistar_Click(object sender, EventArgs e)
        {
            Listar();
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Usuario u = dgvUsuarios.Rows[e.RowIndex].DataBoundItem as BE.Usuario;
            txtIdUsuario.Text = u.Id_Usuario.ToString();
            txtNombreUsuario.Text = u.Nombre;
            txtPerfil.Text = u.Id_Perfil.ToString();
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (txtIdUsuario.Text == "" && txtNombreUsuario.Text != "" && txtPerfil.Text != "" && txtClave.Text != "")
            {
                if (!ClaveValida(txtClave.Text))
                {
                    MessageBox.Show("La clave no cumple con los requisitos." +
                                    "\nDebe tener al menos 6 caracteres, una letra mayúscula y un número.");
                    return;
                }
                else
                {
                    BE.Usuario u = new BE.Usuario();
                    u.Nombre = txtNombreUsuario.Text;
                    u.Clave = Servicios.CryptoManager.Hash(txtClave.Text);
                    u.Id_Perfil = int.Parse(txtPerfil.Text);
                    u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Perfil}|{0}");

                    BLL.Usuario bllu = new BLL.Usuario();
                    bllu.Grabar(u);
                    BLL.DVVUsuario blldvv = new BLL.DVVUsuario();
                    blldvv.Actualizar();
                    BitacoraManager.Registrar("Se crea el usuario: " + u.Nombre);
                    Listar();
                    LimpiarCampos();
                }                
            }
            
        }

        public void Listar()
        {
            BLL.Usuario bllu = new BLL.Usuario();
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = bllu.Listar();
            dgvUsuarios.Columns["Clave"].Visible = false;
            dgvUsuarios.Columns["DescripcionPerfil"].Visible = false;
        }

        private void FrmABMUsuarios_Load(object sender, EventArgs e)
        {
            Listar();
        }

        public bool ClaveValida(string _c)
        {
            if (string.IsNullOrEmpty(_c)) return false;
            string patron = @"^(?=.*[A-Z])(?=.*\d).{6,}$";
            return Regex.IsMatch(_c, patron);

        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            BE.Usuario u = new BE.Usuario();
            u.Id_Usuario = int.Parse(txtIdUsuario.Text);
            u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Perfil}|{1}");
            BLL.Usuario bllu = new BLL.Usuario();
            bllu.Borrar(u);
            BLL.DVVUsuario blldvv = new BLL.DVVUsuario();
            blldvv.Actualizar();
            BitacoraManager.Registrar("Se borra el usuario: " + u.Nombre);
            Listar();
            LimpiarCampos();
        }

        public void LimpiarCampos()
        {
            txtIdUsuario.Text = "";
            txtNombreUsuario.Text = "";
            txtPerfil.Text = "";
            txtClave.Text = "";
        }

        private void btnmodificar_Click(object sender, EventArgs e)
        {
            BE.Usuario u = new BE.Usuario();
            u.Id_Usuario = int.Parse(txtIdUsuario.Text);
            u.Nombre = txtNombreUsuario.Text;
            u.Id_Perfil = int.Parse(txtPerfil.Text);
            u.Clave = Servicios.CryptoManager.Hash(txtClave.Text);
            u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Perfil}|{0}");

            BLL.Usuario bllu = new BLL.Usuario();
            bllu.Grabar(u);
            BLL.DVVUsuario blldvv = new BLL.DVVUsuario();
            blldvv.Actualizar();
            BitacoraManager.Registrar("Se modifica el usuario " + u.Nombre);
            Listar();
            LimpiarCampos();
        }
    }
}
