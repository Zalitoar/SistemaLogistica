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
    public class MP_DVVUsuario : MAPPER<BE.DVVUsuario>
    {
        public override int Borrar(DVVUsuario objeto)
        {
            throw new NotImplementedException();
        }

        public override int Editar(DVVUsuario objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            var parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@Valor_DVV", objeto.Valor_DVV));
            int resultado = acceso.Escribir("ACTUALIZAR_DVVUSUARIO", parametros);
            acceso.Cerrar();
            return resultado;
        }

        public override int Insertar(DVVUsuario objeto)
        {
            throw new NotImplementedException();
        }

        public override List<DVVUsuario> Listar()
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("LEER_DVVUSUARIO");
            acceso.Cerrar();

            List<BE.DVVUsuario> lista = new List<BE.DVVUsuario>();
            foreach (DataRow dr in tabla.Rows)
            {
                lista.Add(new BE.DVVUsuario
                {
                    Tabla_DVV = dr["Tabla_DVV"].ToString(),
                    Valor_DVV = dr["Valor_DVV"].ToString()
                });
            }
            return lista;
        }
        
    }
}
