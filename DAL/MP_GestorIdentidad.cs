using System;
using System.Collections.Generic;
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
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            var parametros = new List<SqlParameter> { acceso.CrearParametro("@Ruta", @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\Backup IngSoftDB.bak") };
            int ok = acceso.Escribir("BACKUP_BD", parametros);
            acceso.Cerrar();
            return ok;
        }
    }
}
