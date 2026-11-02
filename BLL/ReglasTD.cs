using System;
using System.Linq;
using BE;
using Servicios;

namespace BLL
{
    internal static class ReglasTD
    {
        internal static void Permiso(string permiso)
        {
            if (SessionManager.GetInstance()?.TienePermiso(permiso) != true)
                throw new ReglaTDException("TD.SinPermiso");
        }
        internal static void Exigir(bool condicion, string clave = "TD.DatosInvalidos")
        {
            if (!condicion) throw new ReglaTDException(clave);
        }
        internal static bool Texto(string texto, int maximo)
        {
            return !string.IsNullOrWhiteSpace(texto) && texto.Trim().Length <= maximo;
        }
        internal static bool Fecha(DateTime fecha)
        {
            return fecha >= new DateTime(1753, 1, 1) && fecha <= new DateTime(9999, 12, 30);
        }
        internal static bool Cantidad(decimal cantidad, bool admiteCero = false)
        {
            return (admiteCero ? cantidad >= 0 : cantidad > 0) && cantidad <= 999999999999999.999m
                && decimal.Round(cantidad, 3) == cantidad;
        }
    }
}
