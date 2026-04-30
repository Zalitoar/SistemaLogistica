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
    public class MP_Bitacora : MAPPER<BE.Bitacora>
    {
        public override int Borrar(Bitacora objeto)
        {
            throw new NotImplementedException();
        }

        public override int Editar(Bitacora objeteo)
        {
            throw new NotImplementedException();
        }

        public override int Insertar(Bitacora objeto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            //parametros.Add(acceso.CrearParametro("@Id_Bitacora", objeto.Id_Bitacora));
            parametros.Add(acceso.CrearParametro("@Usuario_Bitacora", objeto.Usuario));
            parametros.Add(acceso.CrearParametro("@FechaHora_Bitacora", objeto.FechaHora));
            parametros.Add(acceso.CrearParametro("@Actividad_Bitacora", objeto.Actividad));
            int resultado = acceso.Escribir("INSERTAR_BITACORA", parametros);
            acceso.Cerrar();
            return resultado;
        }

        public override List<Bitacora> Listar()
        {
            List<Bitacora> bitacoras = new List<Bitacora>();
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("LISTAR_BITACORA");

            foreach (DataRow dr in tabla.Rows)
            {
                Bitacora b = new Bitacora();
                b.Id_Bitacora = int.Parse(dr["Id_Bitacora"].ToString());
                b.Usuario = dr["Usuario_Bitacora"].ToString();
                b.FechaHora = DateTime.Parse(dr["FechaHora_Bitacora"].ToString());
                b.Actividad = dr["Actividad_Bitacora"].ToString();
                bitacoras.Add(b);
            }

            return bitacoras;
        }
    }
}   
           
