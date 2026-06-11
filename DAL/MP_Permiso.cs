using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class MP_Permiso
    {
        public ComponentePermiso ObtenerComponente(int idPermiso)
        {
            return ObtenerComponente(idPermiso, new HashSet<int>());
        }

        private ComponentePermiso ObtenerComponente(int idPermiso, HashSet<int> visitados)
        {
            if (visitados.Contains(idPermiso))
                return null;

            visitados.Add(idPermiso);

            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Permiso", idPermiso));
            DataTable tabla = acceso.Leer("OBTENER_PERMISO", parametros);
            acceso.Cerrar();

            if (tabla.Rows.Count == 0) return null;

            DataRow fila = tabla.Rows[0];
            string nombre = fila["Nombre_Permiso"].ToString();
            string tipo = fila["Tipo_Permiso"].ToString();

            if (tipo == "PERMISO")
            {
                return new Permiso
                {
                    Id_Permiso = idPermiso,
                    Nombre_Permiso = nombre
                };
            }

            Rol rol = new Rol
            {
                Id_Permiso = idPermiso,
                Nombre_Permiso = nombre
            };

            foreach (int idComponente in ListarIdsComponentes(idPermiso))
            {
                ComponentePermiso hijo = ObtenerComponente(idComponente, visitados);
                if (hijo != null)
                    rol.Componentes.Add(hijo);
            }

            return rol;
        }

        private List<int> ListarIdsComponentes(int idRol)
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
        public List<Permiso> ListarPermisos()
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("LISTAR_PERMISOS");
            acceso.Cerrar();

            List<Permiso> permisos = new List<Permiso>();
            foreach (DataRow fila in tabla.Rows)
            {
                if (fila["Tipo_Permiso"].ToString() != "PERMISO")
                    continue;

                permisos.Add(new Permiso
                {
                    Id_Permiso = int.Parse(fila["Id_Permiso"].ToString()),
                    Nombre_Permiso = fila["Nombre_Permiso"].ToString()
                });
            }
            return permisos;
        }
    }   
}
