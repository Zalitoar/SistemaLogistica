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
    public class MP_Usuario : MAPPER<BE.Usuario>
    {
        public override int Borrar(Usuario objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Usuario", objeto.Id_Usuario));
            int resultado = acceso.Escribir("BORRAR_USUARIO", parametros);
            acceso.Cerrar();
            return resultado;
        }

        public override int Editar(Usuario objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_Usuario", objeto.Id_Usuario));
            parametros.Add(acceso.CrearParametro("@Nombre_Usuario", objeto.Nombre));
            parametros.Add(acceso.CrearParametro("@Clave_Usuario", objeto.Clave));
            int resultado = acceso.Escribir("EDITAR_USUARIO", parametros);
            acceso.Cerrar();
            return resultado;
        }

        public override int Insertar(Usuario objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Nombre_Usuario", objeto.Nombre));
            parametros.Add(acceso.CrearParametro("@Clave_Usuario", objeto.Clave));
            int resultado = acceso.Escribir("INSERTAR_USUARIO", parametros);
            acceso.Cerrar();
            return resultado;
        }

        public override List<Usuario> Listar()
        {
            List<Usuario> usuarios = new List<Usuario>();
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("LISTAR_USUARIO");

            foreach (DataRow dr in tabla.Rows)
            {
                Usuario u = new Usuario();
                u.Nombre = dr["Nombre_Usuario"].ToString();
                u.Clave = dr["Clave_Usuario"].ToString();
                u.Id_Perfil = int.Parse(dr["Perfil_Usuario"].ToString());
                usuarios.Add(u);

            }

            return usuarios;
        }
    }
}
