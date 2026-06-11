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
	public class MP_Idioma : MAPPER<BE.Idioma>
	{
		public override int Borrar(Idioma objeto)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Id_Idioma", objeto.Id_Idioma));
			int resultado = acceso.Escribir("BORRAR_IDIOMA", parametros);
			acceso.Cerrar();
			return resultado;
		}

		public override int Editar(Idioma objeto)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Id_Idioma", objeto.Id_Idioma));
			parametros.Add(acceso.CrearParametro("@Nombre_Idioma", objeto.Nombre));
			parametros.Add(acceso.CrearParametro("@Codigo_Idioma", objeto.Codigo));
			parametros.Add(acceso.CrearParametro("@Habilitado_Idioma", objeto.Habilitado));
			int resultado = acceso.Escribir("EDITAR_IDIOMA", parametros);
			acceso.Cerrar();
			return resultado;
		}

		public override int Insertar(Idioma objeto)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>();
			parametros.Add(acceso.CrearParametro("@Nombre_Idioma", objeto.Nombre));
			parametros.Add(acceso.CrearParametro("@Codigo_Idioma", objeto.Codigo));
			parametros.Add(acceso.CrearParametro("@Habilitado_Idioma", objeto.Habilitado));
			DataTable tabla = acceso.Leer("INSERTAR_IDIOMA", parametros);
			acceso.Cerrar();

			// Si el SP devuelve el id insertado en la primera fila.
			if (tabla != null && tabla.Rows.Count > 0)
			{
				int id = int.Parse(tabla.Rows[0]["Id_Idioma"].ToString());
				return id;
			}

			return 0;
		}

		public override List<Idio­ma> Listar()
		{
			// Nota: el nombre de la clase BE es 'Idioma' — usarlo correctamente.
			List<Idioma> idiomas = new List<Idioma>();
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			DataTable tabla = acceso.Leer("LISTAR_IDIOMAS");
			acceso.Cerrar();

			foreach (DataRow fila in tabla.Rows)
			{
				idiomas.Add(new Idioma
				{
					Id_Idioma = int.Parse(fila["Id_Idioma"].ToString()),
					Nombre = fila["Nombre_Idioma"].ToString(),
					Codigo = fila.Table.Columns.Contains("Codigo_Idioma") ? fila["Codigo_Idioma"].ToString() : string.Empty,
					Habilitado = fila.Table.Columns.Contains("Habilitado_Idioma") ? int.Parse(fila["Habilitado_Idioma"].ToString()) : 1
				});
			}

			return idiomas;
		}
	}
}
