using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ACCESO
    {
        private SqlConnection conexion;

        public void Abrir()
        {
            conexion = new SqlConnection();         
            conexion.ConnectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            conexion.Open();
        }

        public void AbrirMaster()
        {
            var builder = new SqlConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings["SQL"].ConnectionString)
            {
                InitialCatalog = "master"
            };
            conexion = new SqlConnection(builder.ConnectionString);
            conexion.Open();
        }

        public void Cerrar()
        {
            conexion.Close();
            conexion = null;
            GC.Collect();
        }

        public SqlParameter CrearParametro(string nombre, int valor)
        {
            SqlParameter parametro = new SqlParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor;
            parametro.DbType = DbType.Int32;

            return parametro;
        }

        public SqlParameter CrearParametro(string nombre, string valor)
        {
            SqlParameter parametro = new SqlParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor;
            parametro.DbType = DbType.String;

            return parametro;
        }

        public SqlParameter CrearParametro(string nombre, DateTime valor)
        {
            SqlParameter parametro = new SqlParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor;
            parametro.DbType = DbType.DateTime;

            return parametro;
        }

        public SqlCommand CrearComando(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand com = new SqlCommand();
            com.CommandText = sql;
            com.Connection = conexion;
            com.CommandType = CommandType.StoredProcedure;

            if (parametros != null)
            {
                com.Parameters.AddRange(parametros.ToArray());
            }

            return com;
        }

        public int Escribir(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand com = CrearComando(sql, parametros);
            int filas = 0;

            try
            {
                filas = com.ExecuteNonQuery();
            }
            catch (Exception)
            {
                filas = -1;                
            }

            com.Parameters.Clear();
            com = null;
            return filas;
        }

        public DataTable Leer(string sql, List<SqlParameter> parametros = null)
        {
            SqlDataAdapter adaptador = new SqlDataAdapter();
            adaptador.SelectCommand = CrearComando(sql, parametros);

            DataTable tabla = new DataTable();

            adaptador.Fill(tabla);

            return tabla;

        }
    }
}

