using System.Drawing;
using System.Windows.Forms;

namespace IngSoft
{
    partial class FrmConfiguracionBD
    {
        private ComboBox cmbServidor, cmbBase;
        private NumericUpDown nudPuerto;
        private TextBox txtUsuario, txtClave, txtAdmin, txtRepetir, txtEstado;
        private CheckBox chkWindows, chkCertificado;
        private Button btnBuscar, btnConectar, btnCrear, btnVerificar, btnInicializar, btnGuardar, btnCancelar;
        private TableLayoutPanel campos;
        private GroupBox grpAdmin;
        private Button btnDatosPrueba;
        private void InitializeComponent()
        {
            Name = "FrmConfiguracionBD"; Text = "Configurar base de datos";
            ClientSize = new Size(840, 640); MinimumSize = new Size(760, 620);
            StartPosition = FormStartPosition.CenterParent; MaximizeBox = false; MinimizeBox = false;
            campos = new TableLayoutPanel { Dock=DockStyle.Top, AutoSize=true, ColumnCount=2, Padding=new Padding(8) };
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            cmbServidor = new ComboBox { Name="cmbServidor", Dock=DockStyle.Fill, DropDownStyle=ComboBoxStyle.DropDown };
            nudPuerto = new NumericUpDown { Name="nudPuerto", Maximum=65535, Dock=DockStyle.Left, Width=100 };
            chkWindows = new CheckBox { Name="chkWindows", Text="Usar autenticación de Windows", Checked=true, AutoSize=true };
            txtUsuario = new TextBox { Name="txtUsuario", Dock=DockStyle.Fill, MaxLength=128 };
            txtClave = new TextBox { Name="txtClave", Dock=DockStyle.Fill, UseSystemPasswordChar=true };
            chkCertificado = new CheckBox { Name="chkCertificado", Text="Confiar en el certificado del servidor", AutoSize=true };
            cmbBase = new ComboBox { Name="cmbBase", Dock=DockStyle.Fill, DropDownStyle=ComboBoxStyle.DropDown, MaxLength=128 };
            txtAdmin = new TextBox { Name="txtAdmin", Dock=DockStyle.Fill, UseSystemPasswordChar=true };
            txtRepetir = new TextBox { Name="txtRepetir", Dock=DockStyle.Fill, UseSystemPasswordChar=true };
            AgregarCampo("lblServidor", "Servidor o servidor\\instancia", cmbServidor);
            AgregarCampo("lblPuerto", "Puerto TCP (0 = predeterminado)", nudPuerto);
            AgregarCampo("lblAutenticacion", "Autenticación", chkWindows);
            AgregarCampo("lblUsuario", "Usuario SQL Server", txtUsuario);
            AgregarCampo("lblClave", "Contraseña SQL Server", txtClave);
            AgregarCampo("lblCifrado", "Conexión cifrada", chkCertificado);
            AgregarCampo("lblBase", "Base existente o nombre nuevo", cmbBase);
            grpAdmin = new GroupBox { Name="grpAdmin", Text="Administrador inicial de SistemaLogistica", Dock=DockStyle.Top, Height=132, Padding=new Padding(8), Enabled=false };
            var admin = new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=2, RowCount=3 };
            admin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,38)); admin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,62));
            var notaAdmin = new Label { Name="lblNotaAdmin", Text="Sólo para una base vacía: cree la clave del usuario admin de la aplicación. No es una credencial de SQL Server.", AutoSize=true, Dock=DockStyle.Fill };
            admin.Controls.Add(notaAdmin,0,0); admin.SetColumnSpan(notaAdmin,2);
            admin.Controls.Add(new Label { Name="lblAdmin", Text="Contraseña nueva (mínimo 8 caracteres)", AutoSize=true },0,1); admin.Controls.Add(txtAdmin,1,1);
            admin.Controls.Add(new Label { Name="lblRepetir", Text="Repetir contraseña nueva", AutoSize=true },0,2); admin.Controls.Add(txtRepetir,1,2);
            grpAdmin.Controls.Add(admin);
            var ayuda = new Label { Name="lblAyuda", Text="1. Conecte al servidor. 2. Seleccione o cree una base. 3. Verifique sus tablas y procedimientos. Si está vacía, inicialícela. 4. Guarde la conexión. La búsqueda de instancias es opcional.", AutoSize=true, Dock=DockStyle.Top, Padding=new Padding(8), MaximumSize=new Size(800,0) };
            var botones = new FlowLayoutPanel { AutoSize=true, Dock=DockStyle.Top, Padding=new Padding(8), WrapContents=true };
            btnBuscar = Boton("btnBuscar", "Buscar instancias", botones);
            btnConectar = Boton("btnConectar", "Conectar y listar bases", botones);
            btnCrear = Boton("btnCrear", "Crear base vacía", botones);
            btnVerificar = Boton("btnVerificar", "Verificar estructura de la base", botones);
            btnInicializar = Boton("btnInicializar", "Inicializar base", botones); btnInicializar.Enabled=false;
            btnGuardar = Boton("btnGuardar", "Guardar conexión", botones); btnGuardar.Enabled=false;
            btnDatosPrueba = Boton("btnDatosPrueba", "Inicializar datos de prueba", botones); btnDatosPrueba.Enabled=false;
            btnCancelar = Boton("btnCancelar", "Cancelar", botones);
            txtEstado = new TextBox { Name="txtEstado", Dock=DockStyle.Fill, Multiline=true, ReadOnly=true, TabStop=false, ScrollBars=ScrollBars.Vertical, BackColor=Color.White };
            Controls.Add(txtEstado); Controls.Add(botones); Controls.Add(grpAdmin); Controls.Add(ayuda); Controls.Add(campos);
        }
        private void AgregarCampo(string nombre, string texto, Control control)
        {
            int fila=campos.RowCount++;
            campos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            campos.Controls.Add(new Label { Name=nombre, Text=texto, AutoSize=true, Margin=new Padding(3,4,3,4) },0,fila);
            campos.Controls.Add(control,1,fila);
        }
        private static Button Boton(string nombre, string texto, FlowLayoutPanel panel)
        {
            var b=new Button { Name=nombre, Text=texto, AutoSize=true, AutoSizeMode=AutoSizeMode.GrowAndShrink, MinimumSize=new Size(80,28), Padding=new Padding(4,0,4,0), Margin=new Padding(3) }; panel.Controls.Add(b); return b;
        }
    }
}
