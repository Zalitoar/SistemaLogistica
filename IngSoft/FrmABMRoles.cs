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
    public partial class FrmABMRoles : Form
    {
        public FrmABMRoles()
        {
            InitializeComponent();
        }

        private void FrmABMRoles_Load(object sender, EventArgs e)
        {
            Listar();
            LimpiarCampos();
        }
        public void Listar()
        {
            tvRoles.Nodes.Clear();

            BLL.Rol bllRol = new BLL.Rol();
            foreach (BE.Rol rol in bllRol.Listar())
            {
                ComponentePermiso arbol = bllRol.ObtenerArbol(rol.Id_Permiso);
                tvRoles.Nodes.Add(CrearNodo(arbol));
            }

            tvRoles.ExpandAll();
        }
        private TreeNode CrearNodo(ComponentePermiso componente)
        {
            TreeNode nodo = new TreeNode(componente.Nombre_Permiso);
            nodo.Tag = componente;

            if (componente is BE.Rol rol)
            {
                foreach (ComponentePermiso hijo in rol.Componentes)
                    nodo.Nodes.Add(CrearNodo(hijo));
            }

            return nodo;
        }

        private void btnlistar_Click(object sender, EventArgs e)
        {
            Listar();
        }
        private void tvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ComponentePermiso seleccionado = e.Node.Tag as ComponentePermiso;

            txtIdRol.Text = seleccionado.Id_Permiso.ToString();
            txtNombreRol.Text = seleccionado.Nombre_Permiso;

            bool esRol = seleccionado is BE.Rol;
            btnmodificar.Enabled = esRol;
            btnborrar.Enabled = esRol;
            btnAsignar.Enabled = esRol;

            btnQuitar.Enabled = e.Node.Parent != null;
        }

        private void LimpiarCampos()
        {
            txtIdRol.Text = "";
            txtNombreRol.Text = "";
            btnmodificar.Enabled = false;
            btnborrar.Enabled = false;
            btnAsignar.Enabled = false;
            btnQuitar.Enabled = false;
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            if (txtIdRol.Text == "" && txtNombreRol.Text != "")
            {
                BLL.Rol bllRol = new BLL.Rol();
                bllRol.Insertar(txtNombreRol.Text);

                BitacoraManager.Registrar("Se crea el rol: " + txtNombreRol.Text);
                Listar();
                LimpiarCampos();
            }
        }

        private void btnmodificar_Click(object sender, EventArgs e)
        {
            BLL.Rol bllRol = new BLL.Rol();
            bllRol.Editar(int.Parse(txtIdRol.Text), txtNombreRol.Text);

            BitacoraManager.Registrar("Se modifica el rol: " + txtNombreRol.Text);
            Listar();
            LimpiarCampos();
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            BLL.Rol bllRol = new BLL.Rol();
            string error = bllRol.Borrar(int.Parse(txtIdRol.Text));

            if (error != null)
            {
                MessageBox.Show(error);
                return;
            }

            BitacoraManager.Registrar("Se borra el rol: " + txtNombreRol.Text);
            Listar();
            LimpiarCampos();
        }
    }
}
