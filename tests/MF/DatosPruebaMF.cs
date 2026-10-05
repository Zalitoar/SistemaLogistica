using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using BE;
using DAL;
using Servicios;
using BLL;
class DatosPruebaMF
{
    static int checks;
    static string cadena;
    static object Sql(string sql) { using(var c=new SqlConnection(cadena)) using(var cmd=new SqlCommand(sql,c)) {c.Open();return cmd.ExecuteScalar();} }
    static void Check(bool ok,string texto) { if(!ok)throw new Exception(texto);checks++; }
    static void Rechaza(Action a,string mensaje) {try{a();}catch(SqlException ex){Check(ex.Message.Contains(mensaje),mensaje);return;}throw new Exception("No rechazó "+mensaje);}
    [STAThread] static int Main(string[] args)
    {
        try {
            var config=new ConfiguracionBaseDatos {Servidor=@".\SQLEXPRESS",BaseDatos="SistemaLogistica_MF_Demo_Test_"+DateTime.Now.ToString("yyyyMMddHHmmss"),ConfiarCertificado=true};
            var m=new MP_ConfiguracionBD();m.CrearBase(config);m.Inicializar(config,CryptoManager.Hash("Inicial-Prueba-2026"));cadena=m.Cadena(config);
            typeof(ConexionLocal).GetField("activa",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,cadena);
            Console.WriteLine("Base aislada: "+config.BaseDatos);
            string hash=CryptoManager.Hash("Demo-Prueba-2026"),admin=(string)Sql("SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='admin'");
            var service=new ConfiguracionBDManager();
            SessionManager.Login(new BE.Usuario {Nombre="PruebaSinPermiso"});SessionManager.SetPermisos(new List<string>());
            bool denegado=false;try{service.InicializarDatosPruebaMF(config);}catch(ConfiguracionBDException){denegado=true;}Check(denegado,"Autorización configuración");SessionManager.Logout();
            Sql("CREATE TRIGGER dbo.MF_FalloDemo ON dbo.INFORME_DISPONIBILIDAD AFTER INSERT AS THROW 50099,'fallo prueba',1;");
            Rechaza(()=>m.InicializarDatosPruebaMF(config,hash),"fallo prueba");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.USUARIO")==1 && (int)Sql("SELECT COUNT(*) FROM dbo.UNIDAD_FLOTA")==0 && (int)Sql("SELECT COUNT(*) FROM dbo.PERMISO WHERE Nombre_Permiso LIKE 'DEMO MF %'")==0,"Rollback integral");
            Sql("DROP TRIGGER dbo.MF_FalloDemo");
            string clave=service.InicializarDatosPruebaMF(config);Check(!string.IsNullOrEmpty(clave),"Clave generada por servicio");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.UNIDAD_FLOTA WHERE Dominio LIKE 'MFDEMO%'")==7,"Siete escenarios");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.PARTE_ESTADO_UNIDAD")==7,"Siete partes");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.ORDEN_MANTENIMIENTO")==5,"Cinco órdenes");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.INTERVENCION_MANTENIMIENTO")==4,"Cuatro intervenciones");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.INFORME_DISPONIBILIDAD")==8,"Ocho informes");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.VIAJE WHERE Numero LIKE 'DEMO-MF-%'")==2,"Dos viajes propios");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora LIKE 'DEMO MF %'")==22,"Auditoría de escenarios");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.USUARIO WHERE Nombre_Usuario IN ('demo_flota','demo_mantenimiento','demo_trafico_mf')")==3,"Tres usuarios");
            foreach(string usuario in new[]{"demo_flota","demo_mantenimiento","demo_trafico_mf"})Check((string)Sql("SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='"+usuario+"'")==CryptoManager.Hash(clave),"Clave común MF");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.ROL_COMPONENTE R JOIN dbo.PERMISO P ON P.Id_Permiso=R.Id_Rol WHERE P.Nombre_Permiso LIKE 'DEMO MF %'")==5,"Permisos específicos de roles");
            Check((string)Sql("SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='admin'")==admin,"Admin sin cambios");
            Check(!m.InicializarDatosPruebaMF(config,hash),"Repetición sin cambios");
            Check(m.InicializarDatosPrueba(config,hash),"TD se puede cargar después de MF");
            Check(!m.InicializarDatosPrueba(config,hash),"TD sigue idempotente");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.USUARIO")==8,"TD y MF coexisten");
            // El validador TD verifica DVH/DVV de todos los usuarios antes de insertar sus propios datos.
            Check((string)Sql("SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='demo_flota'")==CryptoManager.Hash(clave),"TD no restablece clave MF");
            SessionManager.Login(new BE.Usuario {Nombre="MF_DEMO_TEST"});SessionManager.SetPermisos(new List<string>{GestorFlota.Permiso,GestorMantenimiento.PermisoProgramar,GestorMantenimiento.PermisoIntervenir,GestorDisponibilidad.PermisoHabilitar,GestorDisponibilidad.PermisoAsignar});
            var datos=new MP_MF();var mantenimiento=new GestorMantenimiento();var disponibilidad=new GestorDisponibilidad();
            Check(mantenimiento.PartesPendientes().Any(p=>p.Dominio=="MFDEMO02"),"MF02 listo para programar");
            int u6=datos.Unidades().Single(u=>u.Dominio=="MFDEMO06").IdUnidadFlota;disponibilidad.Habilitar(u6,"Prueba de habilitación");
            Check(disponibilidad.Disponibles().Any(u=>u.IdUnidadFlota==u6),"MF04 permite habilitar ejemplo");
            Check(!m.InicializarDatosPruebaMF(config,hash) && datos.Unidades().Single(u=>u.IdUnidadFlota==u6).EstadoOperativo=="Disponible","Repetir conserva avance");
            var v=new MP_Viaje().Listar().Single(x=>x.Numero=="DEMO-MF-VIAJE-1");disponibilidad.Asignar(u6,v.IdViaje);
            Check(new MP_Viaje().Listar().Single(x=>x.IdViaje==v.IdViaje).IdUnidadFlota==u6,"MF05 asigna ejemplo");
            SessionManager.Logout();
            config.BaseDatos+="_TDPrimero";m.CrearBase(config);m.Inicializar(config,hash);cadena=m.Cadena(config);m.InicializarDatosPrueba(config,hash);
            Check(m.InicializarDatosPruebaMF(config,hash),"MF se puede cargar después de TD");
            Check((int)Sql("SELECT COUNT(*) FROM dbo.VIAJE WHERE Numero LIKE 'DEMO-TD-%'")==7,"MF conserva viajes TD");
            // Captura del nuevo botón y sus traducciones sin tocar la conexión guardada.
            typeof(ConexionLocal).GetField("activa",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,cadena);
            var idiomas=IdiomaManager.GetInstance();idiomas.CargarIdiomas(new MP_Idioma().Listar());idiomas.CargarTraducciones(new MP_Traduccion().Listar());
            Application.EnableVisualStyles();Directory.CreateDirectory(args[0]);
            using(var form=new IngSoft.FrmConfiguracionBD()) {
                form.ShowInTaskbar=false;form.Opacity=0;form.Show();Application.DoEvents();
                var boton=(Button)typeof(IngSoft.FrmConfiguracionBD).GetField("btnDatosPruebaMF",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form);
                Check(!boton.Enabled,"Botón requiere verificar base");
                foreach(var idioma in idiomas.ListarIdiomas()) {idiomas.SetIdiomaActual(idioma);form.ActualizarIdioma(idioma);Application.DoEvents();Check(boton.Text==idiomas.Traducir("FrmConfiguracionBD.btnDatosPruebaMF.Text"),"Botón traducido");Check(boton.Bottom<=boton.Parent.ClientSize.Height,"Botón visible");using(var bmp=new System.Drawing.Bitmap(form.Width,form.Height)){form.DrawToBitmap(bmp,new System.Drawing.Rectangle(System.Drawing.Point.Empty,form.Size));bmp.Save(Path.Combine(args[0],"DemoMF-"+idioma.Codigo+".png"));}}
            }
            Console.WriteLine("Datos MF: "+checks+" comprobaciones aprobadas.");return 0;
        }catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}
