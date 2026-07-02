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

			// Un idioma predeterminado debe estar disponible para poder cargarlo.
			fila.Habilitado = 1;
			fila.EsDefault = true;
			bllIdioma.Grabar(fila);
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
			var layoutPrincipal = new TableLayoutPanel();
			var gbIdiomas = new GroupBox();
			var gbTraducciones = new GroupBox();
			var layoutIdiomas = new TableLayoutPanel();
			var layoutTraducciones = new TableLayoutPanel();
			var accionesIdiomas = new FlowLayoutPanel();
			var accionesTraducciones = new FlowLayoutPanel();

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

			layoutPrincipal.Dock = DockStyle.Fill;
			layoutPrincipal.Padding = new Padding(12);
			layoutPrincipal.ColumnCount = 1;
			layoutPrincipal.RowCount = 2;
			layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
			layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));

			gbIdiomas.Name = "gbIdiomas";
			gbIdiomas.Text = "Idiomas disponibles";
			gbIdiomas.Dock = DockStyle.Fill;
			gbIdiomas.Padding = new Padding(10);

			gbTraducciones.Name = "gbTraducciones";
			gbTraducciones.Text = "Traducciones del idioma seleccionado";
			gbTraducciones.Dock = DockStyle.Fill;
			gbTraducciones.Padding = new Padding(10);

			ConfigurarLayoutConAcciones(layoutIdiomas, accionesIdiomas);
			ConfigurarLayoutConAcciones(layoutTraducciones, accionesTraducciones);

			this.dgvIdiomas.Name = "dgvIdiomas";
			this.dgvIdiomas.Dock = DockStyle.Fill;
			this.dgvIdiomas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvIdiomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgvIdiomas.MultiSelect = false;
			this.dgvIdiomas.ReadOnly = true;

			this.dgvTraducciones.Name = "dgvTraducciones";
			this.dgvTraducciones.Dock = DockStyle.Fill;
			this.dgvTraducciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvTraducciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgvTraducciones.MultiSelect = false;

			this.btnNuevo.Name = "btnNuevo";
			this.btnNuevo.Text = "Nuevo";
			ConfigurarBotonAccion(this.btnNuevo);
			this.btnNuevo.Click += BtnNuevo_Click;

			this.btnEditar.Name = "btnEditar";
			this.btnEditar.Text = "Editar";
			ConfigurarBotonAccion(this.btnEditar);
			this.btnEditar.Click += BtnEditar_Click;

			this.btnBorrar.Name = "btnBorrar";
			this.btnBorrar.Text = "Borrar";
			ConfigurarBotonAccion(this.btnBorrar);
			this.btnBorrar.Click += BtnBorrar_Click;

			this.btnActivar.Name = "btnActivar";
			this.btnActivar.Text = "Activar";
			ConfigurarBotonAccion(this.btnActivar);
			this.btnActivar.Click += BtnActivar_Click;

			this.btnDesactivar.Name = "btnDesactivar";
			this.btnDesactivar.Text = "Desactivar";
			ConfigurarBotonAccion(this.btnDesactivar);
			this.btnDesactivar.Click += BtnDesactivar_Click;

			this.btnSetDefault.Name = "btnSetDefault";
			this.btnSetDefault.Text = "Set Default";
			ConfigurarBotonAccion(this.btnSetDefault);
			this.btnSetDefault.Click += BtnSetDefault_Click;

			this.btnCargarTraducciones.Name = "btnCargarTraducciones";
			this.btnCargarTraducciones.Text = "Cargar Traducciones";
			ConfigurarBotonAccion(this.btnCargarTraducciones);
			this.btnCargarTraducciones.Click += BtnCargarTraducciones_Click;

			this.btnGuardarTraducciones.Name = "btnGuardarTraducciones";
			this.btnGuardarTraducciones.Text = "Guardar Traducciones";
			ConfigurarBotonAccion(this.btnGuardarTraducciones);
			this.btnGuardarTraducciones.Click += BtnGuardarTraducciones_Click;

			accionesIdiomas.Controls.AddRange(new Control[] {
				this.btnNuevo, this.btnEditar, this.btnBorrar,
				this.btnActivar, this.btnDesactivar, this.btnSetDefault
			});
			accionesTraducciones.Controls.AddRange(new Control[] {
				this.btnCargarTraducciones, this.btnGuardarTraducciones
			});

			layoutIdiomas.Controls.Add(this.dgvIdiomas, 0, 0);
			layoutIdiomas.Controls.Add(accionesIdiomas, 1, 0);
			layoutTraducciones.Controls.Add(this.dgvTraducciones, 0, 0);
			layoutTraducciones.Controls.Add(accionesTraducciones, 1, 0);
			gbIdiomas.Controls.Add(layoutIdiomas);
			gbTraducciones.Controls.Add(layoutTraducciones);
			layoutPrincipal.Controls.Add(gbIdiomas, 0, 0);
			layoutPrincipal.Controls.Add(gbTraducciones, 0, 1);
			this.Controls.Add(layoutPrincipal);

			this.BackColor = Color.FromArgb(245, 247, 250);
			this.ClientSize = new Size(1000, 650);
			this.MinimumSize = new Size(800, 520);
			this.Name = "FrmIdioma";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Load += FrmIdioma_Load;
			this.Text = "Gestión de Idiomas";
			this.ResumeLayout(false);
		}

		private static void ConfigurarLayoutConAcciones(TableLayoutPanel layout, FlowLayoutPanel acciones)
		{
			layout.Dock = DockStyle.Fill;
			layout.ColumnCount = 2;
			layout.RowCount = 1;
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F));

			acciones.Dock = DockStyle.Fill;
			acciones.FlowDirection = FlowDirection.TopDown;
			acciones.WrapContents = false;
			acciones.Padding = new Padding(8, 0, 0, 0);
			acciones.AutoScroll = true;
		}

		private static void ConfigurarBotonAccion(Button boton)
		{
			boton.Size = new Size(160, 34);
			boton.Margin = new Padding(4, 0, 4, 8);
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
			this.ClientSize = new Size(310, 150);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.StartPosition = FormStartPosition.CenterParent;

			txtNombre = new TextBox { Name = "txtNombre", Location = new Point(100, 12), Width = 180 };
			txtCodigo = new TextBox { Name = "txtCodigo", Location = new Point(100, 44), Width = 180 };
			chkHabilitado = new CheckBox { Name = "chkHabilitado", Location = new Point(100, 76), Text = "Habilitado" };

			this.Controls.Add(new Label { Name = "lblNombre", Text = "Nombre:", Location = new Point(12, 12) });
			this.Controls.Add(new Label { Name = "lblCodigo", Text = "Código:", Location = new Point(12, 44) });
			this.Controls.Add(txtNombre);
			this.Controls.Add(txtCodigo);
			this.Controls.Add(chkHabilitado);

			btnOk = new Button { Name = "btnOk", Text = "OK", Location = new Point(100, 108) };
			btnCancel = new Button { Name = "btnCancel", Text = "Cancelar", Location = new Point(188, 108), DialogResult = DialogResult.Cancel };
			this.AcceptButton = btnOk;
			this.CancelButton = btnCancel;
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
