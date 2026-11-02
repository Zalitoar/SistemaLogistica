using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DAL
{
    // La configuración guardada se activa sólo al arrancar, nunca en mitad de una sesión.
    public static class ConexionLocal
    {
        private static string activa;
        public static string Ruta { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemaLogistica", "conexion.dat"); } }
        public static string Actual { get { return activa ?? (activa = Cargar()); } }
        public static void Recargar() { activa = Cargar(); }

        public static string Cargar(string ruta = null)
        {
            ruta = ruta ?? Ruta;
            if (File.Exists(ruta))
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(ruta), null, DataProtectionScope.CurrentUser));
            // Compatibilidad con despliegues previos y ejecutables de pruebas.
            return ConfigurationManager.ConnectionStrings["SQL"]?.ConnectionString ?? "";
        }

        public static void Guardar(string cadena, string ruta = null)
        {
            ruta = ruta ?? Ruta;
            Directory.CreateDirectory(Path.GetDirectoryName(ruta));
            var temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllBytes(temporal, ProtectedData.Protect(Encoding.UTF8.GetBytes(cadena), null, DataProtectionScope.CurrentUser));
                if (File.Exists(ruta)) File.Replace(temporal, ruta, null);
                else File.Move(temporal, ruta);
            }
            finally { if (File.Exists(temporal)) File.Delete(temporal); }
        }
    }
}
