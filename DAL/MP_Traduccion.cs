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
	public class MP_Traduccion : MAPPER<BE.Traduccion>
	{
		public override int Borrar(Traduccion objeto)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Id_Traduccion", objeto.Id_Traduccion));
			int resultado = acceso.Escribir("BORRAR_TRADUCCION", parametros);
			acceso.Cerrar();
			return resultado;
		}

		public override int Editar(Traduccion objeto)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Id_Traduccion", objeto.Id_Traduccion));
			parametros.Add(acceso.CrearParametro("@Id_Idioma", objeto.Id_Idioma));
			parametros.Add(acceso.CrearParametro("@Clave_Traduccion", objeto.Clave));
			parametros.Add(acceso.CrearParametro("@Valor_Traduccion", objeto.Valor));
			int resultado = acceso.Escribir("EDITAR_TRADUCCION", parametros);
			acceso.Cerrar();
			return resultado;
		}

		public override int Insertar(Traduccion objeto)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Id_Idioma", objeto.Id_Idioma));
			parametros.Add(acceso.CrearParametro("@Clave_Traduccion", objeto.Clave));
			parametros.Add(acceso.CrearParametro("@Valor_Traduccion", objeto.Valor));
			DataTable tabla = acceso.Leer("INSERTAR_TRADUCCION", parametros);
			acceso.Cerrar();

			if (tabla != null && tabla.Rows.Count > 0)
			{
				int id = int.Parse(tabla.Rows[0]["Id_Traduccion"].ToString());
				return id;
			}

			return 0;
		}

		public override List<Traduccion> Listar()
		{
			List<Traduccion> lista = new List<Traduccion>();
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			DataTable tabla = acceso.Leer("LISTAR_TRADUCCIONES");
			acceso.Cerrar();

			foreach (DataRow fila in tabla.Rows)
			{
				lista.Add(new Traduccion
				{
					Id_Traduccion = int.Parse(fila["Id_Traduccion"].ToString()),
					Id_Idioma = int.Parse(fila["Id_Idioma"].ToString()),
					Clave = fila["Clave_Traduccion"].ToString(),
					Valor = fila["Valor_Traduccion"].ToString()
				});
			}

			return lista;
		}

		/// <summary>
		/// Lista traducciones para un idioma específico.
		/// </summary>
		public List<Traduccion> ListarPorIdioma(int idIdioma)
		{
			List<Traduccion> lista = new List<Traduccion>();
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Id_Idioma", idIdioma));
			DataTable tabla = acceso.Leer("LISTAR_TRADUCCIONES_POR_IDIOMA", parametros);
			acceso.Cerrar();

			foreach (DataRow fila in tabla.Rows)
			{
				lista.Add(new Traduccion
				{
					Id_Traduccion = int.Parse(fila["Id_Traduccion"].ToString()),
					Id_Idioma = int.Parse(fila["Id_Idioma"].ToString()),
					Clave = fila["Clave_Traduccion"].ToString(),
					Valor = fila["Valor_Traduccion"].ToString()
				});
			}

			return lista;
		}
	}
}
