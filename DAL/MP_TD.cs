using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    internal static class MP_TD
    {
        internal static SqlParameter Parametro(string nombre, object valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }
        internal static DataTable Leer(string procedimiento, params SqlParameter[] parametros)
        {
            ACCESO acceso = new ACCESO();
            bool abierto = false;
            try
            {
                acceso.Abrir();
                abierto = true;
                return acceso.Leer(procedimiento, new List<SqlParameter>(parametros));
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627) throw new ReglaTDException("TD.Duplicado");
                if (ex.Number == 50001) throw new ReglaTDException("TD.EstadoInvalido");
                if (ex.Number == 50002 || ex.Number == 547) throw new ReglaTDException("TD.DatosInvalidos");
                throw;
            }
            finally { if (abierto) acceso.Cerrar(); }
        }
        internal static int Id(DataRow fila, string columna) { return Convert.ToInt32(fila[columna]); }
        internal static DateTime? FechaOpcional(DataRow fila, string columna)
        {
            return fila.IsNull(columna) ? (DateTime?)null : Convert.ToDateTime(fila[columna]);
        }
    }
}
