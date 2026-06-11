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
    public class MP_CambioUsuario : MAPPER<BE.CambioUsuario>
    {
        public override int Borrar(CambioUsuario objeto)
        {
            throw new NotImplementedException();
        }

        public override int Editar(CambioUsuario objeto)
        {
            throw new NotImplementedException();
        }

        public override int Insertar(CambioUsuario objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Id_UsuarioCU", objeto.Id_Usuario));
            parametros.Add(acceso.CrearParametro("@Nombre_UsuarioCU", objeto.Nombre));
            parametros.Add(acceso.CrearParametro("@Clave_UsuarioCU", objeto.Clave));
            parametros.Add(acceso.CrearParametro("@Borrado_UsuarioCU", objeto.Borrado));
            parametros.Add(acceso.CrearParametro("@Id_RolCU", objeto.Id_Rol));            
            int resultado = acceso.Escribir("INSERTAR_USUARIO_CU", parametros);
            acceso.Cerrar();
            return resultado;
        }

        public override List<CambioUsuario> Listar()
        {
            List<CambioUsuario> usuarios = new List<CambioUsuario>();
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("LISTAR_CAMBIOUSUARIO");

            foreach (DataRow dr in tabla.Rows)
            {
                CambioUsuario u = new CambioUsuario();
                u.Id_CU = int.Parse(dr["Id_CU"].ToString());
                u.Id_Usuario = int.Parse(dr["Id_UsuarioCU"].ToString());
                u.Nombre = dr["Nombre_UsuarioCU"].ToString();
                u.Clave = dr["Clave_UsuarioCU"].ToString();
                u.Id_Rol = int.Parse(dr["Id_RolCU"].ToString());
                u.Borrado = int.Parse(dr["Borrado_UsuarioCU"].ToString());                
                usuarios.Add(u);
            }
            return usuarios;
        }
    }
}
