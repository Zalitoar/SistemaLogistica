using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using Servicios;

namespace IngSoft
{
	public partial class FrmIdioma : FormularioTraducible
	{
		private DataGridView dgvIdiomas;
		private DataGridView dgvTraducciones;
		private Button btnNuevo, btnEditar, btnBorrar, btnActivar, btnDesactivar, btnSetDefault, btnCargarTraducciones, btnGuardarTraducciones;
		private BLL.Idioma bllIdioma = new BLL.Idioma();
		private int idiomaSeleccionadoId = 0;

		public FrmIdioma()
		{
			InitializeComponent();

			// Seguridad: sólo admin (Id_Rol == 11) puede abrir
			var u = SessionManager.GetInstance()?.GetUsuario();
			if (u == null || u.Id_Rol != 11)
			{
				MessageBox.Show(ObtenerTexto("FrmIdioma.msgAccesoDenegado", "Acceso denegado. Requiere rol administrador."));
				this.Load += (s, e) => this.BeginInvoke(new Action(() => this.Close()));
			}
		}

		private void FrmIdioma_Load(object sender, EventArgs e)
		{
			CargarIdiomas();
		}

		private void CargarIdiomas()
		{
			var lista = bllIdioma.Listar();
			// Marcar EsDefault en objetos BE.Idioma si no existe la propiedad en BE, adaptado según BD
			dgvIdiomas.DataSource = lista;
		}

		private void BtnNuevo_Click(object sender, EventArgs e)
		{
			var dialog = new FrmIdiomaEditor();
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				var nuevo = new BE.Idioma
				{
					Nombre = dialog.Nombre,
					Codigo = dialog.Codigo,
					Habilitado = dialog.Habilitado ? 1 : 0
				};
				bllIdioma.Grabar(nuevo);
				CargarIdiomas();
			}
		}

		private void BtnEditar_Click(object sender, EventArgs e)
		{
			if (dgvIdiomas.SelectedRows.Count == 0) return;
			var fila = dgvIdiomas.SelectedRows[0].DataBoundItem as BE.Idioma;
			if (fila == null) return;

			var dialog = new FrmIdiomaEditor { Nombre = fila.Nombre, Codigo = fila.Codigo, Habilitado = fila.Habilitado == 1 };
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				fila.Nombre = dialog.Nombre;
				fila.Codigo = dialog.Codigo;
				fila.Habilitado = dialog.Habilitado ? 1 : 0;
				bllIdioma.Grabar(fila);
				CargarIdiomas();
			}
		}

		private void BtnBorrar_Click(object sender, EventArgs e)
		{
			if (dgvIdiomas.SelectedRows.Count == 0) return;
			var fila = dgvIdiomas.SelectedRows[0].DataBoundItem as BE.Idioma;
			if (fila == null) return;
			if (MessageBox.Show(
				ObtenerTexto("FrmIdioma.msgBorrarIdioma", "¿Borrar idioma?"),
				ObtenerTexto("FrmIdioma.msgConfirmar", "Confirmar"),
				MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				bllIdioma.Borrar(fila);
				CargarIdiomas();
			}
		}

		private void BtnActivar_Click(object sender, EventArgs e)
		{
			if (dgvIdiomas.SelectedRows.Count == 0) return;
			var fila = dgvIdiomas.SelectedRows[0].DataBoundItem as BE.Idioma;
			if (fila == null) return;
			fila.Habilitado = 1;
			bllIdioma.Grabar(fila);
			CargarIdiomas();
		}

		private void BtnDesactivar_Click(object sender, EventArgs e)
		{
			if (dgvIdiomas.SelectedRows.Count == 0) return;
			var fila = dgvIdiomas.SelectedRows[0].DataBoundItem as BE.Idioma;
			if (fila == null) return;
			fila.Habilitado = 0;
			bllIdioma.Grabar(fila);
			CargarIdiomas();
		}

		private void BtnSetDefault_Click(object sender, EventArgs e)
		{
			if (dgvIdiomas.SelectedRows.Count == 0) return;
			var fila = dgvIdiomas.SelectedRows[0].DataBoundItem as BE.Idioma;
			if (fila == null) return;
			// Para marcar default, actualizar registro (SP EDITAR_IDIOMA maneja EsDefault)
			// Asumimos que BE.Idioma tiene propiedad EsDefault; si no, se puede extender la clase BE.
			try
			{
				fila.GetType().GetProperty("EsDefault")?.SetValue(fila, 1);
				bllIdioma.Grabar(fila);
			}
			catch { }
			CargarIdiomas();
		}

		private void BtnCargarTraducciones_Click(object sender, EventArgs e)
		{
			if (dgvIdiomas.SelectedRows.Count == 0) return;
			var fila = dgvIdiomas.SelectedRows[0].DataBoundItem as BE.Idioma;
			if (fila == null) return;
			idiomaSeleccionadoId = fila.Id_Idioma;
			var lista = bllIdioma.ListarTraduccionesPorIdioma(idiomaSeleccionadoId);
			dgvTraducciones.DataSource = lista;
		}

		private void BtnGuardarTraducciones_Click(object sender, EventArgs e)
		{
			if (idiomaSeleccionadoId == 0) return;
			var lista = dgvTraducciones.DataSource as List<BE.Traduccion>;
			if (lista == null) return;
			foreach (var t in lista)
			{
				t.Id_Idioma = idiomaSeleccionadoId;
				bllIdioma.GrabarTraduccion(t);
			}
			MessageBox.Show(ObtenerTexto("FrmIdioma.msgTraduccionesGuardadas", "Traducciones guardadas."));
			// Recargar traducciones globales en manager
			var todas = bllIdioma.ListarTraducciones();
			Servicios.IdiomaManager.GetInstance().CargarTraducciones(todas);
			// Notificar cambio para que se apliquen nuevas traducciones si el idioma actual fue afectado
			Servicios.IdiomaManager.GetInstance().NotificarObservers();
		}

		private void InitializeComponent()
		{
			this.dgvIdiomas = new DataGridView();
			this.dgvTraducciones = new DataGridView();
			this.btnNuevo = new Button();
			this.btnEditar = new Button();
			this.btnBorrar = new Button();
			this.btnActivar = new Button();
			this.btnDesactivar = new Button();
			this.btnSetDefault = new Button();
			this.btnCargarTraducciones = new Button();
			this.btnGuardarTraducciones = new Button();

			this.SuspendLayout();

			// Configuración básica de controles (puede ajustarse según diseño)
			this.dgvIdiomas.Name = "dgvIdiomas";
			this.dgvIdiomas.Location = new Point(12, 12);
			this.dgvIdiomas.Size = new Size(400, 200);
			this.dgvIdiomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgvIdiomas.MultiSelect = false;

			this.dgvTraducciones.Name = "dgvTraducciones";
			this.dgvTraducciones.Location = new Point(12, 220);
			this.dgvTraducciones.Size = new Size(400, 200);

			this.btnNuevo.Name = "btnNuevo";
			this.btnNuevo.Text = "Nuevo";
			this.btnNuevo.Location = new Point(420, 12);
			this.btnNuevo.Click += BtnNuevo_Click;

			this.btnEditar.Name = "btnEditar";
			this.btnEditar.Text = "Editar";
			this.btnEditar.Location = new Point(420, 42);
			this.btnEditar.Click += BtnEditar_Click;

			this.btnBorrar.Name = "btnBorrar";
			this.btnBorrar.Text = "Borrar";
			this.btnBorrar.Location = new Point(420, 72);
			this.btnBorrar.Click += BtnBorrar_Click;

			this.btnActivar.Name = "btnActivar";
			this.btnActivar.Text = "Activar";
			this.btnActivar.Location = new Point(420, 102);
			this.btnActivar.Click += BtnActivar_Click;

			this.btnDesactivar.Name = "btnDesactivar";
			this.btnDesactivar.Text = "Desactivar";
			this.btnDesactivar.Location = new Point(420, 132);
			this.btnDesactivar.Click += BtnDesactivar_Click;

			this.btnSetDefault.Name = "btnSetDefault";
			this.btnSetDefault.Text = "Set Default";
			this.btnSetDefault.Location = new Point(420, 162);
			this.btnSetDefault.Click += BtnSetDefault_Click;

			this.btnCargarTraducciones.Name = "btnCargarTraducciones";
			this.btnCargarTraducciones.Text = "Cargar Traducciones";
			this.btnCargarTraducciones.Location = new Point(420, 220);
			this.btnCargarTraducciones.Click += BtnCargarTraducciones_Click;

			this.btnGuardarTraducciones.Name = "btnGuardarTraducciones";
			this.btnGuardarTraducciones.Text = "Guardar Traducciones";
			this.btnGuardarTraducciones.Location = new Point(420, 250);
			this.btnGuardarTraducciones.Click += BtnGuardarTraducciones_Click;

			this.Controls.Add(this.dgvIdiomas);
			this.Controls.Add(this.dgvTraducciones);
			this.Controls.Add(this.btnNuevo);
			this.Controls.Add(this.btnEditar);
			this.Controls.Add(this.btnBorrar);
			this.Controls.Add(this.btnActivar);
			this.Controls.Add(this.btnDesactivar);
			this.Controls.Add(this.btnSetDefault);
			this.Controls.Add(this.btnCargarTraducciones);
			this.Controls.Add(this.btnGuardarTraducciones);

			this.ClientSize = new Size(600, 450);
			this.Name = "FrmIdioma";
			this.Load += FrmIdioma_Load;
			this.Text = "Gestión de Idiomas";
			this.ResumeLayout(false);
		}
	}

	// Form auxiliar para crear/editar idioma
	public class FrmIdiomaEditor : FormularioTraducible
	{
		public string Nombre { get; set; }
		public string Codigo { get; set; }
		public bool Habilitado { get; set; }

		private TextBox txtNombre;
		private TextBox txtCodigo;
		private CheckBox chkHabilitado;
		private Button btnOk, btnCancel;

		public FrmIdiomaEditor()
		{
			this.Name = "FrmIdiomaEditor";
			this.Text = "Idioma";
			this.Size = new Size(320, 180);

			txtNombre = new TextBox { Name = "txtNombre", Location = new Point(100, 12), Width = 180 };
			txtCodigo = new TextBox { Name = "txtCodigo", Location = new Point(100, 44), Width = 180 };
			chkHabilitado = new CheckBox { Name = "chkHabilitado", Location = new Point(100, 76), Text = "Habilitado" };

			this.Controls.Add(new Label { Name = "lblNombre", Text = "Nombre:", Location = new Point(12, 12) });
			this.Controls.Add(new Label { Name = "lblCodigo", Text = "Código:", Location = new Point(12, 44) });
			this.Controls.Add(txtNombre);
			this.Controls.Add(txtCodigo);
			this.Controls.Add(chkHabilitado);

			btnOk = new Button { Name = "btnOk", Text = "OK", Location = new Point(100, 108) };
			btnCancel = new Button { Name = "btnCancel", Text = "Cancelar", Location = new Point(188, 108) };
			btnOk.Click += (s, e) => { Nombre = txtNombre.Text; Codigo = txtCodigo.Text; Habilitado = chkHabilitado.Checked; this.DialogResult = DialogResult.OK; this.Close(); };
			btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

			this.Controls.Add(btnOk);
			this.Controls.Add(btnCancel);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			txtNombre.Text = Nombre ?? "";
			txtCodigo.Text = Codigo ?? "";
			chkHabilitado.Checked = Habilitado;
		}
	}
}
