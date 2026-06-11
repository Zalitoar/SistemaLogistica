using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Servicios
{
    public class BitacoraManager
    {
        public static void Registrar(string actividad)
        {
            var sesion = SessionManager.GetInstance();
            var usuario = sesion != null ? sesion.GetUsuario() : null;
            string nombreUsuario = usuario != null ? usuario.Nombre : "Sistema";

            BE.Bitacora bitacora = new BE.Bitacora();
            bitacora.Usuario = nombreUsuario;
            bitacora.Actividad = actividad;
            bitacora.FechaHora = DateTime.Now;

            DAL.MP_Bitacora dalBit = new DAL.MP_Bitacora();
            try { dalBit.Insertar(bitacora); } catch { /* opcional: evitar bubblear errores al loguear errores */ }
        }

        public static List<string> ListarUsuariosAuditados()
        {
            DAL.MP_Bitacora mp = new DAL.MP_Bitacora();
            List<BE.Bitacora> todasLasEntradas = mp.Listar();
            return todasLasEntradas
                .Select(x => x.Usuario)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToList();
        }

        public static List<BE.Bitacora> FiltrarBitacora(string usuario, DateTime fdesde, DateTime fhasta)
        {          
            DAL.MP_Bitacora mp = new DAL.MP_Bitacora();
            List<BE.Bitacora> todasLasEntradas = mp.Listar();

            var consulta = todasLasEntradas.AsEnumerable();

            consulta = consulta.Where(x => x.FechaHora.Date >= fdesde.Date && x.FechaHora.Date <= fhasta.Date);

            if (!string.IsNullOrEmpty(usuario))
            {
                consulta = consulta.Where(x => x.Usuario.Trim().Equals(usuario.Trim(), StringComparison.OrdinalIgnoreCase));
            }
            return consulta.OrderByDescending(x => x.FechaHora).ToList();
        }
    }
}
