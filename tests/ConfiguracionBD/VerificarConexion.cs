using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using BE;
using DAL;
using Servicios;

internal static class VerificarConexion
{
    private static int checks;
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); checks++; }
    private static object Scalar(string cadena, string sql)
    { using (var c=new SqlConnection(cadena)) using(var cmd=new SqlCommand(sql,c)) { c.Open(); return cmd.ExecuteScalar(); } }
    [STAThread]
    private static int Main(string[] args)
    {
        string nombre=args[0], servidor=args[1], salida=args[2];
        if (!System.Text.RegularExpressions.Regex.IsMatch(nombre,@"^SistemaLogistica_Config_Test_[a-zA-Z0-9_]+$")) return 2;
        var d=new ConfiguracionBaseDatos { Servidor=servidor,BaseDatos=nombre,ConfiarCertificado=true };
        var m=new MP_ConfiguracionBD();
        try
        {
            string cadena=m.Cadena(d);
            bool sinConfiguracion=false; try { m.Verificar(""); } catch(ConfiguracionBDException ex) { sinConfiguracion=ex.Clave=="BD.SinConfigurar"; }
            Check(sinConfiguracion,"Detectar ausencia de configuracion");
            Check(new SqlConnectionStringBuilder(cadena).Encrypt,"Cifrado requerido");
            Check(!new SqlConnectionStringBuilder(m.Cadena(new ConfiguracionBaseDatos {Servidor="localhost",Puerto=1433,BaseDatos=nombre,AutenticacionWindows=false,Usuario="prueba",Clave="nunca registrar"})).IntegratedSecurity,"Modo SQL");
            m.CrearBase(d);
            Check(m.ListarBases(d).Contains(nombre),"Listado de bases");
            var vacia=m.Verificar(cadena);
            Check(vacia.Vacia && !vacia.Compatible && vacia.Faltantes.Count>0,"Detectar base vacia");
            string hash=CryptoManager.Hash("Inicial-Prueba-2026");
            m.Inicializar(d,hash);
            Check(m.Verificar(cadena).Compatible,"Inicializacion completa");
            Check((string)Scalar(cadena,"SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='admin'")==hash,"Contraseña elegida, no clave seed");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.ROL_COMPONENTE R JOIN dbo.PERMISO P ON P.Id_Permiso=R.Id_Componente WHERE R.Id_Rol=11 AND P.Nombre_Permiso='CONFIGURAR_BD'"))==1,"Permiso administrador");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.TRADUCCION WHERE Clave_Traduccion='FrmConfiguracionBD.Title'"))==3,"Tres idiomas");
            Check((string)Scalar(cadena,"SELECT dvh_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='admin'")==CryptoManager.Hash("admin|"+hash+"|11|0"),"DVH inicial");
            Check((string)Scalar(cadena,"SELECT Valor_DVV FROM dbo.DVV WHERE Tabla_DVV='Usuario'")==CryptoManager.Hash("1admin"+hash+"11"),"DVV inicial");
            bool rechazo=false; try { m.Inicializar(d,hash); } catch(SqlException) { rechazo=true; }
            Check(rechazo,"No reinicializar base con objetos");
            string local=Path.Combine(salida,"conexion-prueba.dat");
            ConexionLocal.Guardar(cadena,local);
            Check(ConexionLocal.Cargar(local)==cadena,"DPAPI ida y vuelta");
            Check(!Encoding.UTF8.GetString(File.ReadAllBytes(local)).Contains(nombre),"No guardar texto plano");
            ConexionLocal.Guardar(cadena+";Pooling=False",local);
            Check(ConexionLocal.Cargar(local).Contains("Pooling=False"),"Reemplazo atomico");
            SessionManager.Login(new Usuario { Nombre="sin-permiso" }); SessionManager.SetPermisos(new List<string>());
            rechazo=false; try { new ConfiguracionBDManager().ListarBases(d); } catch(ConfiguracionBDException) { rechazo=true; }
            Check(rechazo,"Acceso por permiso"); SessionManager.Logout();
            rechazo=false; try { new ConfiguracionBDManager().Inicializar(d,"corta","distinta"); } catch(ConfiguracionBDException ex) { rechazo=ex.Clave=="BD.ClaveInicial"; }
            Check(rechazo,"Clave inicial invalida");
            Check((string)Scalar(cadena,"SELECT LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),CONVERT(VARCHAR(MAX),N'José' COLLATE Latin1_General_100_BIN2_UTF8))),2))")==CryptoManager.Hash("José"),"Hash UTF8 compatible con usuarios acentuados");
            Scalar(cadena,"CREATE TRIGGER dbo.DemoFalloPrueba ON dbo.COMPROBANTE_ENTREGA AFTER INSERT AS THROW 50099,'Fallo demo de prueba',1;");
            rechazo=false; try { m.InicializarDatosPrueba(d,hash); } catch(SqlException) { rechazo=true; }
            Check(rechazo && Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.VIAJE"))==0 && Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.USUARIO"))==1,"Rollback completo de datos demo");
            Scalar(cadena,"DROP TRIGGER dbo.DemoFalloPrueba; SELECT 1;");
            Check(m.InicializarDatosPrueba(d,hash),"Cargar demo");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.USUARIO WHERE Nombre_Usuario LIKE 'demo[_]%'"))==4,"Cuatro usuarios demo");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.VIAJE WHERE Numero LIKE 'DEMO-TD-%'"))==7,"Siete viajes demo");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.ENTREGA"))==21,"Veintiuna entregas demo");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.COMPROBANTE_ENTREGA"))==7,"Siete comprobantes");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.DOCUMENTO_DESPACHO"))==6,"Seis despachos");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.BITACORA"))==22,"Eventos demo");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.DETALLE_CARGA_PLANIFICADA"))==21,"Carga prevista por viaje");
            Check(!m.InicializarDatosPrueba(d,CryptoManager.Hash("OtraClave123")),"Demo idempotente");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.USUARIO"))==5,"Sin usuarios duplicados");
            Check((string)Scalar(cadena,"SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='demo_planificador'")==hash,"Sin restablecer clave demo");
            Check((string)Scalar(cadena,"SELECT Clave_Usuario FROM dbo.USUARIO WHERE Nombre_Usuario='admin'")==hash,"Preservar admin");
            Check(Convert.ToInt32(Scalar(cadena,"SELECT COUNT(*) FROM dbo.ROL_COMPONENTE R JOIN dbo.PERMISO P ON P.Id_Permiso=R.Id_Componente JOIN dbo.USUARIO U ON U.Id_Rol=R.Id_Rol WHERE U.Nombre_Usuario LIKE 'demo[_]%' AND P.Nombre_Permiso='CONFIGURAR_BD'"))==0,"No elevar permisos demo");
            using(var conexion=new SqlConnection(cadena))
            using(var comando=new SqlCommand("SELECT Id_Usuario,Nombre_Usuario,Clave_Usuario,Id_Rol,Borrado_Usuario,dvh_Usuario FROM dbo.USUARIO ORDER BY Id_Usuario",conexion))
            {
                conexion.Open(); var texto=new StringBuilder();
                using(var lector=comando.ExecuteReader()) while(lector.Read())
                {
                    texto.Append(lector[0]).Append(lector[1]).Append(lector[2]).Append(lector[3]);
                    Check((string)lector[5]==CryptoManager.Hash(lector[1]+"|"+lector[2]+"|"+lector[3]+"|"+lector[4]),"DVH demo");
                }
                Check((string)Scalar(cadena,"SELECT Valor_DVV FROM dbo.DVV WHERE Tabla_DVV='Usuario'")==CryptoManager.Hash(texto.ToString()),"DVV demo");
            }
            Scalar(cadena,"DROP PROCEDURE dbo.TD_CERRAR_VIAJE; SELECT 1;");
            var incompleta=m.Verificar(cadena);
            Check(!incompleta.Compatible && !incompleta.Vacia && incompleta.Faltantes.Contains("dbo.TD_CERRAR_VIAJE"),"Detectar objeto faltante");
            Scalar(cadena,"ALTER TABLE dbo.MERCADERIA ALTER COLUMN Descripcion VARCHAR(200) NOT NULL; SELECT 1;");
            Check(m.Verificar(cadena).Faltantes.Contains("MERCADERIA.Descripcion"),"Detectar tipo de columna incorrecto");
            var sistema=new ConfiguracionBaseDatos { Servidor=servidor,BaseDatos="master",ConfiarCertificado=true };
            rechazo=false; try { m.Verificar(m.Cadena(sistema)); } catch(ConfiguracionBDException ex) { rechazo=ex.Clave=="BD.BaseSistema"; }
            Check(rechazo,"Proteger bases sistema");
            string mal=Path.Combine(salida,"paquete-invalido"); Directory.CreateDirectory(mal);
            foreach(var f in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"DatabaseSetup"))) File.Copy(f,Path.Combine(mal,Path.GetFileName(f)),true);
            File.WriteAllText(Path.Combine(mal,"fallo.sql"),"THROW 50003, 'Fallo inyectado de prueba', 1;");
            var manifest=XDocument.Load(Path.Combine(mal,"manifest.xml")); manifest.Root.Add(new XElement("Script",new XAttribute("File","fallo.sql"))); manifest.Save(Path.Combine(mal,"manifest.xml"));
            d.BaseDatos=nombre+"_Rollback"; m.CrearBase(d);
            rechazo=false; try { new MP_ConfiguracionBD(mal).Inicializar(d,hash); } catch(SqlException) { rechazo=true; }
            Check(rechazo && m.Verificar(m.Cadena(d)).Vacia,"Rollback deja base vacia");
            Application.EnableVisualStyles();
            using(var form=new IngSoft.FrmConfiguracionBD())
            { form.ShowInTaskbar=false; form.Show(); Application.DoEvents(); using(var imagen=new System.Drawing.Bitmap(form.Width,form.Height)) { form.DrawToBitmap(imagen,form.ClientRectangle); imagen.Save(Path.Combine(salida,"asistente.png")); } form.Close(); }
            Console.WriteLine("OK: "+checks+" verificaciones. Bases aisladas: "+nombre+" y "+d.BaseDatos);
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally { SessionManager.Logout(); }
    }
}
