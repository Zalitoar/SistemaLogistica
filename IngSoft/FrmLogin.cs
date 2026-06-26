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
	public partial class FrmLogin : Form, Servicios.IIdiomaObserver
	{
		private bool integridadok = true;
		private ComboBox cmbIdiomasLogin;

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
			// Mantener la validación de integridad existente
			ValidarIntegridad();

			// Añadir selector de idiomas dinámicamente (si no existe en el diseñador)
			AgregarComboIdiomas();

			// Registrarse en IdiomaManager y aplicar idioma actual
			try
			{
				Servicios.IdiomaManager.GetInstance().RegistrarObserver(this);
				ActualizarIdioma(Servicios.IdiomaManager.GetInstance().GetIdiomaActual());
			}
			catch
			{
				// No bloquear la carga si el manager falla
			}
		}

		private void AgregarComboIdiomas()
		{
			if (cmbIdiomasLogin != null) return;

			cmbIdiomasLogin = new ComboBox();
			cmbIdiomasLogin.Name = "cmbIdiomasLogin";
			cmbIdiomasLogin.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbIdiomasLogin.Width = 180;
			// Colocar en la esquina superior derecha del formulario
			cmbIdiomasLogin.Location = new Point(Math.Max(8, this.ClientSize.Width - cmbIdiomasLogin.Width - 8), 8);
			cmbIdiomasLogin.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			cmbIdiomasLogin.SelectedIndexChanged += CmbIdiomasLogin_SelectedIndexChanged;

			this.Controls.Add(cmbIdiomasLogin);

			// Cargar items
			try
			{
				var lista = IdiomaManager.GetInstance().ListarIdiomas();
				cmbIdiomasLogin.DisplayMember = "Nombre";
				cmbIdiomasLogin.ValueMember = "Id_Idioma";
				cmbIdiomasLogin.DataSource = lista;

				// Seleccionar idioma activo si existe
				var activo = IdiomaManager.GetInstance().GetIdiomaActual();
				if (activo != null)
				{
					for (int i = 0; i < cmbIdiomasLogin.Items.Count; i++)
					{
						var it = cmbIdiomasLogin.Items[i] as BE.Idioma;
						if (it != null && it.Id_Idioma == activo.Id_Idioma)
						{
							cmbIdiomasLogin.SelectedIndex = i;
							break;
						}
					}
				}
			}
			catch
			{
				// ignorar errores de carga
			}
		}

		private void CmbIdiomasLogin_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				var sel = cmbIdiomasLogin.SelectedItem as BE.Idioma;
				if (sel != null)
				{
					IdiomaManager.GetInstance().SetIdiomaActual(sel);
				}
			}
			catch
			{
				// ignorar
			}
		}

        private void ValidarIntegridad()
        {
            try
            {
                // Validación DVV
                string nuevoCalculoDvv = GestorIntegridad.Calcular();
                List<BE.DVVUsuario> listaDVV = GestorIntegridad.Listar();

                if (listaDVV == null || listaDVV.Count == 0)
                {
                    integridadok = false;
                    return;
                }

                if (nuevoCalculoDvv != listaDVV[0].Valor_DVV)
                {
                    integridadok = false;
                }

                // Validación DVH
                var registrosConError = GestorIntegridad.ValidarIntegridadDVH();

                if (registrosConError != null && registrosConError.Count > 0)
                {
                    integridadok = false;
                }
            }
            catch
            {
                integridadok = false;
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
		{            
			if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtClave.Text))
			{
				MessageBox.Show(ObtenerTexto("FrmLogin.msgCamposRequeridos", "Debe completar todos los campos."));
				return; 
			}            

			BLL.Usuario bllu = new BLL.Usuario();
			BE.Usuario usuario = bllu.ValidarIngreso(txtUsuario.Text, txtClave.Text);

			if (usuario == null)
			{
				MessageBox.Show(ObtenerTexto("FrmLogin.msgCredencialesInvalidas", "Usuario y/o Clave incorrecta o inexistente."));
				return;
			}

			if (!integridadok && usuario.Id_Rol != 11) // 11 = administrador
			{
				MessageBox.Show(ObtenerTexto("FrmLogin.msgSistemaNoDisponible", "El sistema no esta disponible en este momento. Por favor, intente más tarde."));
				return;
			}

			// Inicio de sesión autorizado
			Servicios.SessionManager.Login(usuario);
			BitacoraManager.Registrar("Inicio de Sesión");
			List<string> permisos = new BLL.Rol().ObtenerArbol(usuario.Id_Rol).ObtenerPermisos().Select(p => p.Nombre_Permiso).ToList();
			Servicios.SessionManager.SetPermisos(permisos);

			// Persistir preferencia de idioma seleccionada (si hay una)
			try
			{
				if (cmbIdiomasLogin != null && cmbIdiomasLogin.SelectedItem is BE.Idioma idiSel)
				{
					new BLL.Idioma().SetIdiomaPreferidoUsuario(usuario.Id_Usuario, idiSel.Id_Idioma);
				}
			}
			catch
			{
				// ignorar errores al guardar preferencia
			}

			MessageBox.Show(ObtenerTexto("FrmLogin.msgIngresoExitoso", "Ingreso exitoso."));

			if (!integridadok)
			{
				frmRestore res = new frmRestore();
				res.Show();
				this.Hide();
			}
			else
			{
				FrmApp App = new FrmApp();
				App.Show();
				this.Hide();
			}                
		}

		/// <summary>
		/// Obtiene traducción por clave con fallback a textoPorDefecto.
		/// </summary>
		private string ObtenerTexto(string clave, string textoPorDefecto)
		{
			try
			{
				var mgr = Servicios.IdiomaManager.GetInstance();
				if (mgr != null)
				{
					string t = mgr.Traducir(clave);
					if (!string.IsNullOrEmpty(t) && !string.Equals(t, clave, StringComparison.OrdinalIgnoreCase))
						return t;
				}
			}
			catch
			{
				// ignorar y retornar por defecto
			}
			return textoPorDefecto;
		}

		/// <summary>
		/// IIdiomaObserver: actualiza UI usando la convención.
		/// </summary>
		public void ActualizarIdioma(BE.Idioma nuevoIdioma)
		{
			var mgr = Servicios.IdiomaManager.GetInstance();
			if (mgr == null) return;

			try
			{
				string baseName = string.IsNullOrWhiteSpace(this.Name) ? this.GetType().Name : this.Name;

				// Título
				string titulo = mgr.Traducir(baseName + ".Title");
				if (!string.IsNullOrEmpty(titulo) && !string.Equals(titulo, baseName + ".Title", StringComparison.OrdinalIgnoreCase))
					this.Text = titulo;

				// Traducir controles
				TraducirControlesRecursivo(this, baseName, mgr);

				// Actualizar combo de idiomas (si existe) para reflejar cambios en nombres si corresponde
				if (cmbIdiomasLogin != null)
				{
					var lista = mgr.ListarIdiomas();
					cmbIdiomasLogin.DataSource = null;
					cmbIdiomasLogin.DataSource = lista;
					cmbIdiomasLogin.DisplayMember = "Nombre";
					cmbIdiomasLogin.ValueMember = "Id_Idioma";

					if (nuevoIdioma != null)
					{
						for (int i = 0; i < cmbIdiomasLogin.Items.Count; i++)
						{
							var it = cmbIdiomasLogin.Items[i] as BE.Idioma;
							if (it != null && it.Id_Idioma == nuevoIdioma.Id_Idioma)
							{
								cmbIdiomasLogin.SelectedIndex = i;
								break;
							}
						}
					}
				}
			}
			catch
			{
				// No propagar errores
			}
		}

		private void TraducirControlesRecursivo(Control padre, string baseName, Servicios.IdiomaManager mgr)
		{
			foreach (Control c in padre.Controls)
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(c.Name))
					{
						string clave = baseName + "." + c.Name + ".Text";
						string tradu = mgr.Traducir(clave);
						if (!string.IsNullOrEmpty(tradu) && !string.Equals(tradu, clave, StringComparison.OrdinalIgnoreCase))
						{
							c.Text = tradu;
						}
					}

					if (c.HasChildren)
						TraducirControlesRecursivo(c, baseName, mgr);

					if (c is ToolStrip ts)
						TraducirToolStripItemsRecursivo(ts.Items, baseName, mgr);
				}
				catch { }
			}
		}

		private void TraducirToolStripItemsRecursivo(ToolStripItemCollection items, string baseName, Servicios.IdiomaManager mgr)
		{
			foreach (ToolStripItem item in items)
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(item.Name))
					{
						string clave = baseName + "." + item.Name + ".Text";
						string tradu = mgr.Traducir(clave);
						if (!string.IsNullOrEmpty(tradu) && !string.Equals(tradu, clave, StringComparison.OrdinalIgnoreCase))
						{
							item.Text = tradu;
						}
					}

					if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
						TraducirToolStripItemsRecursivo(menuItem.DropDownItems, baseName, mgr);
				}
				catch { }
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			try
			{
				Servicios.IdiomaManager.GetInstance().QuitarObserver(this);
			}
			catch { }
			base.OnFormClosed(e);
		}

		public bool ClaveValida(string _c)
		{
			if (string.IsNullOrEmpty(_c)) return false;
			string patron = @"^(?=.*[A-Z])(?=.*\d).{6,}$";
			return Regex.IsMatch(_c, patron);
		}
	}
}
