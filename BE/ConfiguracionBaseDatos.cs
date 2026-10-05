using System;
using System.Collections.Generic;

namespace BE
{
    public class ConfiguracionBaseDatos
    {
        public string Servidor { get; set; }
        public int Puerto { get; set; }
        public string BaseDatos { get; set; }
        public bool AutenticacionWindows { get; set; } = true;
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public bool ConfiarCertificado { get; set; }
    }

    public class DiagnosticoBaseDatos
    {
        public bool Vacia { get; set; }
        public List<string> Faltantes { get; } = new List<string>();
        public bool Compatible { get { return !Vacia && Faltantes.Count == 0; } }
    }

    public class ConfiguracionBDException : Exception
    {
        public string Clave { get; private set; }
        public ConfiguracionBDException(string clave) : base(clave) { Clave = clave; }
    }
}
