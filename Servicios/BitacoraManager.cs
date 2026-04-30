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
            SessionManager sesion = SessionManager.GetInstance();
            BE.Usuario u = sesion.GetUsuario();

            BE.Bitacora bitacora = new BE.Bitacora();
            bitacora.Usuario = u.Nombre;
            bitacora.Actividad = actividad;
            bitacora.FechaHora = DateTime.Now;

            DAL.MP_Bitacora dalBit = new DAL.MP_Bitacora();
            dalBit.Insertar(bitacora);

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
            if(string.IsNullOrEmpty(usuario))
            {
                return todasLasEntradas
                    .Where(x => x.FechaHora.Date >= fdesde.Date && x.FechaHora.Date <= fhasta)
                    .OrderByDescending(x => x.FechaHora)
                    .ToList();
            }
            return todasLasEntradas
                .Where(x => x.Usuario == usuario && x.FechaHora.Date >= fdesde.Date && x.FechaHora <= fhasta.Date)
                .OrderByDescending(x => x.FechaHora)
                .ToList();

        }
    }
}
