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
    public partial class FrmABMUsuarios : FormularioTraducible
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
            try
            {
                Usuario u = dgvUsuarios.Rows[e.RowIndex].DataBoundItem as BE.Usuario;
                txtIdUsuario.Text = u.Id_Usuario.ToString();
                txtNombreUsuario.Text = u.Nombre;
                txtPerfil.Text = u.Id_Rol.ToString();

                CambioUsuario cu = new CambioUsuario
                {
                    Id_Usuario = u.Id_Usuario,
                    Nombre = u.Nombre,
                    Clave = u.Clave,
                    Id_Rol = u.Id_Rol,
                    Borrado = u.Borrado,
                };

                ListarCambios(u);

                lblid.Text = u.Id_Usuario.ToString();
                lblususel.Text = u.Nombre;
            }
            catch (Exception)
            {
                
            }
        }

        public void ListarCambios(BE.Usuario u)
        {            
            dgvVerisionesAnteriores.DataSource = null;
            BLL.CambioUsuario bllcu = new BLL.CambioUsuario();
            dgvVerisionesAnteriores.DataSource = bllcu.Listar().Where(c => c.Id_Usuario == u.Id_Usuario).ToList();
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (txtIdUsuario.Text == "" && txtNombreUsuario.Text != "" && txtPerfil.Text != "" && txtClave.Text != "")
            {
                if (!ClaveValida(txtClave.Text))
                {
                    MessageBox.Show(ObtenerTexto(
                        "FrmABMUsuarios.msgClaveNoCumple",
                        "La clave no cumple con los requisitos.\nDebe tener al menos 6 caracteres, una letra mayúscula y un número."));
                    return;
                }
                else
                {
                    BE.Usuario u = new BE.Usuario();
                    u.Nombre = txtNombreUsuario.Text;
                    u.Clave = Servicios.CryptoManager.Hash(txtClave.Text);
                    u.Id_Rol = int.Parse(txtPerfil.Text);
                    u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Rol}|{0}");

                    BLL.Usuario bllu = new BLL.Usuario();
                    bllu.Grabar(u);                    
                    GestorIntegridad.Actualizar();
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
            List<BE.Usuario> usuarios = bllu.Listar();
            dgvUsuarios.DataSource = usuarios.Where(u => u.Borrado == 0).ToList();
            dgvUsuarios.Columns["Clave"].Visible = false;
            dgvUsuarios.Columns["DescripcionPerfil"].Visible = false;
            dgvUsuarios.Columns["DVH"].Visible = false;
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
            BLL.Usuario bllu = new BLL.Usuario();
            u = bllu.Listar().FirstOrDefault(x => x.Id_Usuario == int.Parse(txtIdUsuario.Text));
            u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Rol}|{1}");

            bllu.Borrar(u);
            GestorIntegridad.Actualizar();
            BitacoraManager.Registrar("Se borra el usuario: " + u.Nombre);

            BLL.CambioUsuario bllcu = new BLL.CambioUsuario();
            bllcu.Grabar(new CambioUsuario
            {
                Id_Usuario = u.Id_Usuario,
                Nombre = u.Nombre,
                Clave = u.Clave,
                Id_Rol = u.Id_Rol,
                Borrado = 0
            });

            Listar();
            ListarCambios(u);
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
            u.Id_Rol = int.Parse(txtPerfil.Text);
            u.Clave = Servicios.CryptoManager.Hash(txtClave.Text);
            u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Rol}|{0}");

            BLL.Usuario bllu = new BLL.Usuario();
            bllu.Grabar(u);
            //BLL.DVVUsuario blldvv = new BLL.DVVUsuario();
            GestorIntegridad.Actualizar();

            BLL.CambioUsuario bllcu = new BLL.CambioUsuario();
            bllcu.Grabar(new CambioUsuario
            {
                Id_Usuario = u.Id_Usuario,
                Nombre = u.Nombre,
                Clave = u.Clave,
                Id_Rol = u.Id_Rol,
                Borrado = 0
            });


            BitacoraManager.Registrar("Se modifica el usuario " + u.Nombre);
            Listar();
            ListarCambios(u);
            LimpiarCampos();
        }

        private void btnRestaurarVersAnt_Click(object sender, EventArgs e)
        {
            Usuario u = new Usuario();
            u.Id_Usuario = int.Parse(lblid_usu_sel.Text);
            u.Nombre = lblnombre_sel.Text;
            u.Clave = lblclave_usu_sel.Text;
            u.Borrado = int.Parse(lblborr_usu_sel.Text);
            u.Id_Rol = int.Parse(lblid_rol_sel.Text);
            u.DVH = CryptoManager.Hash($"{u.Nombre}|{u.Clave}|{u.Id_Rol}|{u.Borrado}");

            BLL.Usuario bllu = new BLL.Usuario();
            bllu.Grabar(u);
            

            BLL.CambioUsuario bllcu = new BLL.CambioUsuario();
            bllcu.Grabar(new CambioUsuario
            {
                Id_Usuario = u.Id_Usuario,
                Nombre = u.Nombre,
                Clave = u.Clave,
                Id_Rol = u.Id_Rol,
                Borrado = u.Borrado
            });

            BitacoraManager.Registrar("Se modifica el usuario " + u.Nombre);
            Listar();
            ListarCambios(u);
            LimpiarCampos();

        }

        private void dgvVerisionesAnteriores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CambioUsuario u = dgvVerisionesAnteriores.Rows[e.RowIndex].DataBoundItem as BE.CambioUsuario;
            lblid_usu_sel.Text = u.Id_Usuario.ToString();
            lblnombre_sel.Text = u.Nombre;
            lblclave_usu_sel.Text = u.Clave;
            lblborr_usu_sel.Text = u.Borrado.ToString();
            lblid_rol_sel.Text = u.Id_Rol.ToString();           

        }
    }
}
