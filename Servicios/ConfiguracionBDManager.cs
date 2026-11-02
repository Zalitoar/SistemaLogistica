using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;

namespace Servicios
{
    public class ConfiguracionBDManager
    {
        public const string Permiso = "CONFIGURAR_BD";
        private readonly MP_ConfiguracionBD mapper = new MP_ConfiguracionBD();
        private static void Autorizar()
        {
            // Sin sesión se usa exclusivamente el asistente previo al login.
            var sesion = SessionManager.GetInstance();
            if (sesion != null && !sesion.TienePermiso(Permiso)) throw new ConfiguracionBDException("BD.SinPermiso");
        }
        public ConfiguracionBaseDatos Actual() { Autorizar(); return mapper.Describir(ConexionLocal.Actual); }
        public List<string> Descubrir() { Autorizar(); return mapper.Descubrir(); }
        public List<string> ListarBases(ConfiguracionBaseDatos d) { Autorizar(); return mapper.ListarBases(d); }
        public DiagnosticoBaseDatos Verificar(ConfiguracionBaseDatos d) { Autorizar(); return mapper.Verificar(mapper.Cadena(d)); }
        public DiagnosticoBaseDatos VerificarInicio() { return mapper.Verificar(ConexionLocal.Actual); }
        public void ActivarAlInicio()
        {
            if (SessionManager.GetInstance() != null) throw new ConfiguracionBDException("BD.SinPermiso");
            ConexionLocal.Recargar();
        }
        public void CrearBase(ConfiguracionBaseDatos d) { Autorizar(); mapper.CrearBase(d); }
        public void Inicializar(ConfiguracionBaseDatos d, string clave, string repetida)
        {
            Autorizar();
            if (string.IsNullOrWhiteSpace(clave) || clave.Length < 8 || clave != repetida)
                throw new ConfiguracionBDException("BD.ClaveInicial");
            if (!Verificar(d).Vacia) throw new ConfiguracionBDException("BD.NoVacia");
            mapper.Inicializar(d, CryptoManager.Hash(clave));
        }
        public string InicializarDatosPrueba(ConfiguracionBaseDatos d)
        {
            Autorizar();
            if (!Verificar(d).Compatible) throw new ConfiguracionBDException("BD.Incompatible");
            string clave="Demo-"+Guid.NewGuid().ToString("N").Substring(0,16);
            return mapper.InicializarDatosPrueba(d,CryptoManager.Hash(clave)) ? clave : null;
        }
        public string InicializarDatosPruebaMF(ConfiguracionBaseDatos d)
        {
            Autorizar();
            if (!Verificar(d).Compatible) throw new ConfiguracionBDException("BD.Incompatible");
            string clave="Demo-"+Guid.NewGuid().ToString("N").Substring(0,16);
            return mapper.InicializarDatosPruebaMF(d,CryptoManager.Hash(clave)) ? clave : null;
        }
        public void Guardar(ConfiguracionBaseDatos d)
        {
            Autorizar();
            if (!Verificar(d).Compatible) throw new ConfiguracionBDException("BD.Incompatible");
            ConexionLocal.Guardar(mapper.Cadena(d));
            // Auditar en la conexión aún activa; nunca registrar la cadena o las credenciales.
            if (SessionManager.GetInstance() != null) BitacoraManager.Registrar("Configuración de base de datos guardada para el próximo inicio");
        }
        public static string ClaveError(Exception ex)
        {
            var regla = ex as ConfiguracionBDException;
            if (regla != null) return regla.Clave;
            var sql = ex as SqlException;
            if (sql != null)
            {
                if (sql.Number == 18456) return "BD.Autenticacion";
                if (sql.Number == 4060 || sql.Number == 911) return "BD.BaseInaccesible";
                if (sql.Number == 262 || sql.Number == 229 || sql.Number == 15247) return "BD.PermisosSQL";
                if (sql.Number == 1801) return "BD.BaseExiste";
                if (sql.Number == 50001) return "BD.NoVacia";
                if (sql.Number == 50010) return sql.Message.StartsWith("BD.Demo") ? sql.Message : "BD.DemoError";
                if (sql.Number == 102 || sql.Number == 156 || sql.Number == 207 || sql.Number == 208 || sql.Number == 2714) return "BD.Error";
                return "BD.ConexionFallida";
            }
            if (ex is CryptographicException || ex is IOException || ex is UnauthorizedAccessException) return "BD.ConfiguracionLocal";
            return "BD.Error";
        }
    }
}
