using BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IngSoft
{
    public partial class FrmConfiguracionBD : FormularioTraducible
    {
        private readonly ConfiguracionBDManager gestor = new ConfiguracionBDManager();
        private readonly bool inicio;
        private bool ocupado;
        private DiagnosticoBaseDatos diagnostico;
        public FrmConfiguracionBD() : this(true, null) { }
        public FrmConfiguracionBD(bool primerInicio, string error = null)
        {
            inicio = primerInicio;
            InitializeComponent();
            try
            {
                var d=gestor.Actual(); cmbServidor.Text=d.Servidor; cmbBase.Text=d.BaseDatos;
                chkWindows.Checked=d.AutenticacionWindows; txtUsuario.Text=d.Usuario; txtClave.Text=d.Clave;
                chkCertificado.Checked=d.ConfiarCertificado;
            }
            catch (Exception ex) { error = ConfiguracionBDManager.ClaveError(ex); }
            txtEstado.Text=T(error ?? "BD.Instrucciones");
            foreach (Control c in new Control[] { cmbServidor,cmbBase,txtUsuario,txtClave }) c.TextChanged += (s,e) => Invalidar();
            nudPuerto.ValueChanged += (s,e) => Invalidar();
            chkWindows.CheckedChanged += (s,e) => { Invalidar(); Habilitar(); };
            chkCertificado.CheckedChanged += (s,e) => Invalidar();
            btnCancelar.Click += (s,e) => { DialogResult=DialogResult.Cancel; Close(); };
            FormClosing += (s,e) => { if (ocupado) e.Cancel=true; };
            btnBuscar.Click += (s,e) => Trabajo(() => gestor.Descubrir(), o =>
            { cmbServidor.Items.Clear(); cmbServidor.Items.AddRange(((List<string>)o).ToArray()); txtEstado.Text=T("BD.Busqueda"); });
            btnConectar.Click += (s,e) =>
            {
                var d=Datos(); Invalidar();
                Trabajo(() => gestor.ListarBases(d), o => { cmbBase.Items.Clear(); cmbBase.Items.AddRange(((List<string>)o).ToArray()); txtEstado.Text=T("BD.Conectado"); });
            };
            btnVerificar.Click += (s,e) => Verificar();
            btnCrear.Click += (s,e) =>
            {
                var d=Datos();
                if (!Confirmar("BD.ConfirmarCrear")) return;
                Trabajo(() => { gestor.CrearBase(d); return gestor.Verificar(d); }, Mostrar);
            };
            btnInicializar.Click += (s,e) =>
            {
                var d=Datos(); string clave=txtAdmin.Text, repetida=txtRepetir.Text;
                if (!Confirmar("BD.ConfirmarInicializar")) return;
                Trabajo(() => { gestor.Inicializar(d,clave,repetida); return gestor.Verificar(d); }, o =>
                { txtAdmin.Clear(); txtRepetir.Clear(); Mostrar(o); txtEstado.AppendText(Environment.NewLine+T("BD.AdminCreado")); });
            };
            btnGuardar.Click += (s,e) =>
            {
                var d=Datos();
                if (!Confirmar(inicio ? "BD.ConfirmarGuardar" : "BD.ConfirmarReinicio")) return;
                Trabajo(() => { gestor.Guardar(d); return null; }, o => { DialogResult=DialogResult.OK; Close(); });
            };
            btnDatosPrueba.Click += (s,e) =>
            {
                var d=Datos();
                if (!Confirmar("BD.ConfirmarDemo")) return;
                Trabajo(() => gestor.InicializarDatosPrueba(d), o =>
                {
                    txtEstado.Text=o==null ? T("BD.DemoExistente") : string.Format(T("BD.DemoCargado"),(string)o);
                    if(o!=null) MessageBox.Show(this,txtEstado.Text,Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
                });
            };
            btnDatosPruebaMF.Click += (s,e) =>
            {
                var d=Datos();
                if (!Confirmar("BD.ConfirmarDemoMF")) return;
                Trabajo(() => gestor.InicializarDatosPruebaMF(d), o =>
                {
                    txtEstado.Text=o==null ? T("BD.DemoExistente") : string.Format(T("BD.DemoMFCargado"),(string)o);
                    if(o!=null) MessageBox.Show(this,txtEstado.Text,Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
                });
            };
            Habilitar();
        }
        private ConfiguracionBaseDatos Datos() { return new ConfiguracionBaseDatos { Servidor=cmbServidor.Text,Puerto=(int)nudPuerto.Value,BaseDatos=cmbBase.Text,AutenticacionWindows=chkWindows.Checked,Usuario=txtUsuario.Text,Clave=txtClave.Text,ConfiarCertificado=chkCertificado.Checked }; }
        private void Invalidar() { diagnostico=null; Habilitar(); }
        private void Habilitar()
        {
            campos.Enabled=!ocupado;
            grpAdmin.Enabled=!ocupado && diagnostico?.Vacia==true;
            txtUsuario.Enabled=!chkWindows.Checked; txtClave.Enabled=!chkWindows.Checked;
            foreach (var b in new[] { btnBuscar,btnConectar,btnCrear,btnVerificar,btnCancelar }) b.Enabled=!ocupado;
            btnGuardar.Enabled=!ocupado && diagnostico?.Compatible==true;
            btnDatosPrueba.Enabled=!ocupado && diagnostico?.Compatible==true;
            btnDatosPruebaMF.Enabled=!ocupado && diagnostico?.Compatible==true;
            btnInicializar.Enabled=!ocupado && diagnostico?.Vacia==true;
        }
        private async void Trabajo(Func<object> operacion, Action<object> resultado)
        {
            if (ocupado) return;
            ocupado=true; Habilitar(); txtEstado.Text=T("BD.Trabajando");
            try
            {
                object valor=await Task.Run(operacion);
                ocupado=false;
                resultado(valor);
            }
            catch (Exception ex) { diagnostico=null; txtEstado.Text=T(ConfiguracionBDManager.ClaveError(ex)); }
            finally { ocupado=false; if (!IsDisposed) Habilitar(); }
        }
        private void Verificar() { var d=Datos(); Trabajo(() => gestor.Verificar(d), Mostrar); }
        private void Mostrar(object valor)
        {
            diagnostico=(DiagnosticoBaseDatos)valor;
            txtEstado.Text=T(diagnostico.Compatible ? "BD.Compatible" : diagnostico.Vacia ? "BD.Vacia" : "BD.Incompatible");
            if (!diagnostico.Compatible) txtEstado.AppendText(Environment.NewLine+string.Join(Environment.NewLine,diagnostico.Faltantes));
        }
        private bool Confirmar(string clave) { return MessageBox.Show(this,T(clave),Text,MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes; }
        private string T(string clave) { return ObtenerTexto(clave,TextoPredeterminado(clave)); }
        internal static string DescribirError(Exception ex)
        {
            string clave=ConfiguracionBDManager.ClaveError(ex);
            string texto=IdiomaManager.GetInstance().Traducir(clave);
            return string.IsNullOrEmpty(texto) || texto==clave ? TextoPredeterminado(clave) : texto;
        }
        internal static string TextoPredeterminado(string clave)
        {
            switch (clave)
            {
                case "BD.ConfirmarDemo": return "¿Agregar usuarios y operaciones ficticias TD en la base seleccionada? Use una base de pruebas. No se restablecerán datos existentes. Se generará una contraseña para los cuatro usuarios demo.";
                case "BD.DemoCargado": return "Datos de prueba cargados. Usuarios: demo_planificador, demo_expedicion, demo_transporte y demo_consulta. Contraseña común: {0}. Anótela antes de cerrar; no se mostrará nuevamente. El usuario admin conserva su contraseña.";
                case "BD.DemoExistente": return "Los datos de prueba ya fueron cargados. No se duplicaron registros ni se restablecieron contraseñas o avances.";
                case "BD.DemoColision": return "Existen registros con nombres reservados para DEMO. No se modificó la base. Utilice otra base de pruebas.";
                case "BD.DemoIntegridad": return "La integridad de usuarios no es válida. No se cargaron datos ni se recalcularon sus verificadores.";
                case "BD.DemoError": return "No se pudo cargar el conjunto de prueba. La operación fue revertida.";
                case "BD.SinConfigurar": return "No hay una conexión a base de datos configurada. Configure el servidor y la base antes de iniciar sesión.";
                case "BD.ConexionFallida": return "No se pudo conectar a SQL Server. Revise servidor, puerto, servicio, red y certificado. No desactive la validación del certificado salvo que confíe en ese servidor.";
                case "BD.Autenticacion": return "SQL Server rechazó las credenciales. Revise el usuario, la contraseña y el modo de autenticación habilitado.";
                case "BD.BaseInaccesible": return "La base no existe o su cuenta no tiene acceso. Conecte al servidor y seleccione una base accesible.";
                case "BD.PermisosSQL": return "La cuenta de SQL Server no tiene permisos suficientes para la operación.";
                case "BD.BaseExiste": return "Ya existe una base con ese nombre. Selecciónela y verifíquela.";
                case "BD.Metadatos": return "Se requiere permiso VIEW DEFINITION en la base para comprobar sus objetos.";
                case "BD.BaseSistema": return "No se permite utilizar ni inicializar una base de sistema.";
                case "BD.NoVacia": return "La base contiene objetos. No se ejecutó la inicialización. Use Database para una actualización controlada.";
                case "BD.Incompatible": return "La base no está lista: faltan objetos o datos iniciales. Si contiene objetos, solicite una actualización con Database; no se sobrescribirán datos.";
                case "BD.ConfiguracionLocal": return "No se pudo leer o guardar la configuración local protegida. Revise los permisos del perfil de Windows y vuelva a configurar la conexión.";
                case "BD.PaqueteAusente": return "Falta el paquete DatabaseSetup. Repare o vuelva a instalar la aplicación.";
                case "BD.DatosInvalidos": return "Complete servidor y base. Para puerto TCP use sólo el host, sin instancia ni coma. Complete el usuario cuando utilice autenticación SQL Server.";
                case "BD.ClaveInicial": return "Para crear el usuario admin de SistemaLogistica, complete una contraseña nueva de al menos 8 caracteres y repítala exactamente. No es la contraseña de SQL Server. Luego pulse Inicializar base.";
                case "BD.SinPermiso": return "No tiene permiso para configurar la base de datos.";
                case "BD.Instrucciones": return "Conecte al servidor, seleccione una base y pulse Verificar estructura de la base. Esta opción comprueba tablas, procedimientos y datos iniciales. Guarde sólo después de una verificación correcta.";
                case "BD.Busqueda": return "Búsqueda finalizada. La lista puede estar incompleta; escriba manualmente el servidor si no aparece.";
                case "BD.ConfirmarDemoMF": return "¿Agregar usuarios y operaciones ficticias MF? Use una base de pruebas. Los registros existentes no se restablecen.";
                case "BD.DemoMFCargado": return "Datos MF cargados. Usuarios: demo_flota, demo_mantenimiento y demo_trafico_mf. Contraseña común: {0}. Anótela antes de cerrar; no se mostrará nuevamente. Las contraseñas anteriores no cambian.";
                case "BD.Conectado": return "Conexión al servidor correcta. Seleccione una base de la lista, o escriba un nombre nuevo y pulse Crear base vacía.";
                case "BD.Trabajando": return "Operación en curso. Espere a que finalice.";
                case "BD.Compatible": return "Conexión correcta. Los objetos y datos iniciales requeridos están presentes. Puede guardar la conexión.";
                case "BD.Vacia": return "La base está vacía: todavía no puede guardar la conexión. En Administrador inicial de SistemaLogistica escriba una contraseña nueva de al menos 8 caracteres y repítala. Pulse Inicializar base para crear los objetos y habilitar Guardar conexión.";
                case "BD.AdminCreado": return "Usuario inicial: admin. Ingrese con la contraseña elegida en este formulario.";
                case "BD.ConfirmarCrear": return "¿Crear una base vacía con el nombre indicado en el servidor seleccionado?";
                case "BD.ConfirmarInicializar": return "¿Crear las tablas, procedimientos y datos iniciales en la base vacía seleccionada?";
                case "BD.ConfirmarGuardar": return "¿Guardar esta conexión protegida para este usuario de Windows y continuar al login?";
                case "BD.ConfirmarReinicio": return "¿Guardar esta conexión y reiniciar la aplicación? Se cerrará la sesión y se perderán los cambios sin guardar en las ventanas abiertas.";
                default: return "No se pudo completar la operación de base de datos. Revise la configuración y los recursos de instalación.";
            }
        }
    }
}
