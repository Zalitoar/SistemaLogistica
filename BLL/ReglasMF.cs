using System;
using BE;
using Servicios;

namespace BLL
{
    internal static class ReglasMF
    {
        internal static void Permiso(string permiso)
        {
            Exigir(SessionManager.GetInstance()?.TienePermiso(permiso) == true, "MF.SinPermiso");
        }

        internal static void Exigir(bool condicion, string clave = "MF.DatosInvalidos")
        {
            if (!condicion)
                throw new ReglaMFException(clave);
        }

        internal static bool Texto(string valor, int max)
        {
            return !string.IsNullOrWhiteSpace(valor) && valor.Trim().Length <= max;
        }

        internal static bool Km(decimal valor)
        {
            return valor >= 0 && valor <= 999999999999999.999m && decimal.Round(valor, 3) == valor;
        }

        internal static void Auditar(string accion, int id)
        {
            BitacoraManager.Registrar("MF: " + accion + " " + id);
        }
    }
}
