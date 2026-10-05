using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DAL
{
    public class MP_ConfiguracionBD
    {
        private readonly string paquete;
        public MP_ConfiguracionBD(string directorio = null)
        { paquete = directorio ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseSetup"); }

        public string Cadena(ConfiguracionBaseDatos datos, bool master = false)
        {
            if (datos == null || string.IsNullOrWhiteSpace(datos.Servidor) || datos.Puerto < 0 || datos.Puerto > 65535 ||
                (!master && (string.IsNullOrWhiteSpace(datos.BaseDatos) || datos.BaseDatos.Length > 128)) ||
                (!datos.AutenticacionWindows && string.IsNullOrWhiteSpace(datos.Usuario)))
                throw new ConfiguracionBDException("BD.DatosInvalidos");
            string servidor = datos.Servidor.Trim();
            if (datos.Puerto > 0)
            {
                if (servidor.Contains("\\") || servidor.Contains(",")) throw new ConfiguracionBDException("BD.DatosInvalidos");
                servidor = "tcp:" + servidor + "," + datos.Puerto;
            }
            return new SqlConnectionStringBuilder {
                DataSource = servidor, InitialCatalog = master ? "master" : datos.BaseDatos.Trim(),
                IntegratedSecurity = datos.AutenticacionWindows,
                UserID = datos.AutenticacionWindows ? "" : datos.Usuario,
                Password = datos.AutenticacionWindows ? "" : datos.Clave,
                Encrypt = true, TrustServerCertificate = datos.ConfiarCertificado,
                PersistSecurityInfo = false, ConnectTimeout = 8, ApplicationName = "SistemaLogistica"
            }.ConnectionString;
        }

        public ConfiguracionBaseDatos Describir(string cadena)
        {
            if (string.IsNullOrWhiteSpace(cadena)) return new ConfiguracionBaseDatos();
            var b = new SqlConnectionStringBuilder(cadena);
            return new ConfiguracionBaseDatos { Servidor = b.DataSource, BaseDatos = b.InitialCatalog,
                AutenticacionWindows = b.IntegratedSecurity, Usuario = b.UserID, Clave = b.Password,
                ConfiarCertificado = b.TrustServerCertificate };
        }

        public List<string> Descubrir()
        {
            return SqlDataSourceEnumerator.Instance.GetDataSources().AsEnumerable()
                .Select(r => Convert.ToString(r["ServerName"]) + (string.IsNullOrEmpty(Convert.ToString(r["InstanceName"])) ? "" : "\\" + r["InstanceName"]))
                .Distinct().OrderBy(s => s).ToList();
        }

        public List<string> ListarBases(ConfiguracionBaseDatos datos)
        {
            using (var c = new SqlConnection(Cadena(datos, true)))
            using (var cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4 AND state = 0 AND HAS_DBACCESS(name) = 1 ORDER BY name", c))
            {
                c.Open(); var bases = new List<string>();
                using (var r = cmd.ExecuteReader()) while (r.Read()) bases.Add(r.GetString(0));
                return bases;
            }
        }

        private XDocument Manifesto()
        {
            string archivo = Path.Combine(paquete, "manifest.xml");
            if (!File.Exists(archivo)) throw new ConfiguracionBDException("BD.PaqueteAusente");
            return XDocument.Load(archivo);
        }

        private static void BaseUsuario(SqlConnection c)
        {
            using (var cmd = new SqlCommand("SELECT DB_ID()", c))
                if (Convert.ToInt32(cmd.ExecuteScalar()) <= 4) throw new ConfiguracionBDException("BD.BaseSistema");
        }

        public DiagnosticoBaseDatos Verificar(string cadena)
        {
            if (string.IsNullOrWhiteSpace(cadena)) throw new ConfiguracionBDException("BD.SinConfigurar");
            var manifest = Manifesto();
            using (var c = new SqlConnection(cadena))
            {
                c.Open(); BaseUsuario(c);
                using (var permiso = new SqlCommand("SELECT HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DEFINITION')", c))
                    if (Convert.ToInt32(permiso.ExecuteScalar()) != 1) throw new ConfiguracionBDException("BD.Metadatos");
                var resultado = new DiagnosticoBaseDatos();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.objects WHERE is_ms_shipped = 0", c))
                    resultado.Vacia = Convert.ToInt32(cmd.ExecuteScalar()) == 0;
                foreach (var obj in manifest.Root.Elements("Object"))
                {
                    string nombre = (string)obj.Attribute("Name");
                    using (var cmd = new SqlCommand("SELECT OBJECT_ID(@nombre, @tipo)", c))
                    {
                        cmd.Parameters.AddWithValue("@nombre", "dbo." + nombre);
                        cmd.Parameters.AddWithValue("@tipo", (string)obj.Attribute("Type"));
                        if (cmd.ExecuteScalar() == DBNull.Value) { resultado.Faltantes.Add("dbo." + nombre); continue; }
                    }
                    foreach (var col in obj.Elements("Column"))
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(@nombre) AND name = @columna AND TYPE_NAME(system_type_id) = @tipo", c))
                    {
                        cmd.Parameters.AddWithValue("@nombre", "dbo." + nombre);
                        cmd.Parameters.AddWithValue("@columna", (string)col.Attribute("Name"));
                        cmd.Parameters.AddWithValue("@tipo", (string)col.Attribute("SqlType"));
                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0) resultado.Faltantes.Add(nombre + "." + (string)col.Attribute("Name"));
                    }
                    foreach (var requerido in obj.Elements().Where(e => e.Name == "Constraint" || e.Name == "Index"))
                    {
                        string consulta = requerido.Name == "Index"
                            ? "SELECT COUNT(*) FROM sys.indexes WHERE object_id=OBJECT_ID(@tabla) AND name=@nombre AND is_disabled=0"
                            : "SELECT COUNT(*) FROM sys.objects WHERE parent_object_id=OBJECT_ID(@tabla) AND name=@nombre";
                        using (var cmd = new SqlCommand(consulta, c))
                        {
                            cmd.Parameters.AddWithValue("@tabla", "dbo." + nombre);
                            cmd.Parameters.AddWithValue("@nombre", (string)requerido.Attribute("Name"));
                            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0) resultado.Faltantes.Add(nombre+"."+(string)requerido.Attribute("Name"));
                        }
                    }
                }
                if (resultado.Faltantes.Count == 0)
                {
                    using (var cmd = new SqlCommand("SELECT CASE WHEN EXISTS(SELECT 1 FROM dbo.IDIOMA WHERE Habilitado_Idioma=1) AND EXISTS(SELECT 1 FROM dbo.USUARIO WHERE Borrado_Usuario=0) AND EXISTS(SELECT 1 FROM dbo.DVV WHERE Tabla_DVV='Usuario') THEN 1 ELSE 0 END", c))
                        if (Convert.ToInt32(cmd.ExecuteScalar()) != 1) resultado.Faltantes.Add("Seeds / datos iniciales");
                }
                return resultado;
            }
        }

        public bool InicializarDatosPrueba(ConfiguracionBaseDatos datos, string hash)
        {
            var archivo=Manifesto().Root.Element("DemoScript");
            string ruta=archivo==null ? "" : Path.Combine(paquete,(string)archivo.Attribute("File"));
            if (!File.Exists(ruta)) throw new ConfiguracionBDException("BD.PaqueteAusente");
            using (var c=new SqlConnection(Cadena(datos)))
            using (var cmd=new SqlCommand(File.ReadAllText(ruta),c))
            {
                c.Open(); BaseUsuario(c); cmd.CommandTimeout=120;
                cmd.Parameters.Add("@ClaveDemo",SqlDbType.VarChar,64).Value=hash;
                return Convert.ToInt32(cmd.ExecuteScalar())==1;
            }
        }

        public void CrearBase(ConfiguracionBaseDatos datos)
        {
            // Cadena valida también el nombre; QuoteIdentifier evita inyección en DDL.
            Cadena(datos);
            using (var c = new SqlConnection(Cadena(datos, true)))
            using (var quoting = new SqlCommandBuilder())
            using (var cmd = new SqlCommand("CREATE DATABASE " + quoting.QuoteIdentifier(datos.BaseDatos.Trim()), c))
            { c.Open(); cmd.CommandTimeout = 60; cmd.ExecuteNonQuery(); }
        }

        public void Inicializar(ConfiguracionBaseDatos datos, string hashInicial)
        {
            var manifest = Manifesto();
            using (var c = new SqlConnection(Cadena(datos)))
            {
                c.Open(); BaseUsuario(c);
                using (var tx = c.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'SistemaLogistica.Bootstrap',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000; IF @r<0 THROW 50000,'Bootstrap ocupado',1; IF EXISTS(SELECT 1 FROM sys.objects WHERE is_ms_shipped=0) THROW 50001,'Base no vacia',1;", c, tx)) cmd.ExecuteNonQuery();
                        foreach (var archivo in manifest.Root.Elements("Script"))
                        {
                            string ruta = Path.Combine(paquete, (string)archivo.Attribute("File"));
                            if (!File.Exists(ruta)) throw new ConfiguracionBDException("BD.PaqueteAusente");
                            string texto = File.ReadAllText(ruta);
                            foreach (string lote in Regex.Split(texto, @"^\s*GO\s*(?:--[^\r\n]*)?$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                            {
                                if (string.IsNullOrWhiteSpace(lote)) continue;
                                using (var cmd = new SqlCommand(lote, c, tx))
                                {
                                    cmd.CommandTimeout = 120;
                                    // CREATE PROCEDURE debe enviarse como lote DDL sin parámetros externos.
                                    if (lote.Contains("@ClaveInicial"))
                                        cmd.Parameters.Add("@ClaveInicial", SqlDbType.VarChar, 64).Value = hashInicial;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        tx.Commit();
                    }
                    catch { try { tx.Rollback(); } catch { } throw; }
                }
            }
        }
    }
}
