using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class MP_GestorIdentidad
    {
        public int Restore()
        {
            var builder = new SqlConnectionStringBuilder(
                ConexionLocal.Actual);

            ACCESO acceso = new ACCESO();
            acceso.AbrirMaster();
            var parametros = new List<SqlParameter>
            {
                acceso.CrearParametro("@Ruta", @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\NUEVAIngSoftv3.bak"),
                acceso.CrearParametro("@BaseDatos", builder.InitialCatalog)
            };
            SqlCommand com = acceso.CrearComando("BACKUP_BD", parametros);

            int ok;
            try
            {
                com.ExecuteNonQuery();
                ok = 1;
            }
            catch (Exception)
            {
                ok = -1;
            }

            acceso.Cerrar();
            return ok;            
        }
    }
}
