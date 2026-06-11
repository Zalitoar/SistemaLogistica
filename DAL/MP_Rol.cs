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
    public class MP_Rol : MAPPER<Rol>
    {
        public override int Insertar(Rol objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Nombre_Permiso", objeto.Nombre_Permiso));
            DataTable tabla = acceso.Leer("INSERTAR_ROL", parametros);
            acceso.Cerrar();

            return int.Parse(tabla.Rows[0]["Id_Permiso"].ToString());
        }

        public override int Editar(Rol objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Permiso", objeto.Id_Permiso));
            parametros.Add(acceso.CrearParametro("@Nombre_Permiso", objeto.Nombre_Permiso));
            int resultado = acceso.Escribir("EDITAR_ROL", parametros);
            acceso.Cerrar();

            return resultado;
        }

        public override int Borrar(Rol objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Permiso", objeto.Id_Permiso));
            int resultado = acceso.Escribir("BORRAR_ROL", parametros);
            acceso.Cerrar();

            return resultado;
        }

        public override List<Rol> Listar()
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("LISTAR_ROLES");
            acceso.Cerrar();

            List<Rol> roles = new List<Rol>();
            foreach (DataRow fila in tabla.Rows)
            {
                roles.Add(new Rol
                {
                    Id_Permiso = int.Parse(fila["Id_Permiso"].ToString()),
                    Nombre_Permiso = fila["Nombre_Permiso"].ToString()
                });
            }
            return roles;
        }
    }
}
