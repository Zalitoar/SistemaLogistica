using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class MP_RolComponente
    {
        public int Asignar(int idRol, int idComponente)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Rol", idRol));
            parametros.Add(acceso.CrearParametro("@Id_Componente", idComponente));
            int resultado = acceso.Escribir("ASIGNAR_COMPONENTE_ROL", parametros);
            acceso.Cerrar();

            return resultado;
        }

        public int Quitar(int idRol, int idComponente)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Rol", idRol));
            parametros.Add(acceso.CrearParametro("@Id_Componente", idComponente));
            int resultado = acceso.Escribir("QUITAR_COMPONENTE_ROL", parametros);
            acceso.Cerrar();

            return resultado;
        }

        public List<int> ListarComponentes(int idRol)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Rol", idRol));
            DataTable tabla = acceso.Leer("LISTAR_COMPONENTES_ROL", parametros);
            acceso.Cerrar();

            List<int> ids = new List<int>();
            foreach (DataRow fila in tabla.Rows)
                ids.Add(int.Parse(fila["Id_Componente"].ToString()));

            return ids;
        }

        public List<int> ListarRolesQueContienen(int idComponente)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Componente", idComponente));
            DataTable tabla = acceso.Leer("LISTAR_ROLES_QUE_CONTIENEN", parametros);
            acceso.Cerrar();

            List<int> ids = new List<int>();
            foreach (DataRow fila in tabla.Rows)
                ids.Add(int.Parse(fila["Id_Rol"].ToString()));

            return ids;
        }
    }
}
