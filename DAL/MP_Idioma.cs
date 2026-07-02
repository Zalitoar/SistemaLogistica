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
			parametros.Add(acceso.CrearParametro("@EsDefault", objeto.EsDefault ? 1 : 0));
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
			parametros.Add(acceso.CrearParametro("@EsDefault", objeto.EsDefault ? 1 : 0));
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

		public override List<Idioma> Listar()
		{
			List<Idioma> idiomas = new List<Idioma>();
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			DataTable tabla = acceso.Leer("LISTAR_IDIOMAS");
			acceso.Cerrar();

			foreach (DataRow fila in tabla.Rows)
			{
				idiomas.Add(new Idioma
				{
					Id_Idioma = fila.Field<int>("Id_Idioma"),
					Nombre = fila.Field<string>("Nombre_Idioma"),
					Codigo = fila.Table.Columns.Contains("Codigo_Idioma") ? fila.Field<string>("Codigo_Idioma") : string.Empty,
					Habilitado = ConvertHabilitado(fila, "Habilitado_Idioma"),
					EsDefault = ConvertBooleano(fila, "EsDefault")
				});
			}

			return idiomas;
		}

		// Obtiene el idioma preferido del usuario (SP: OBTENER_IDIOMA_PREFERIDO_USUARIO)
		public Idioma ObtenerIdiomaPreferidoUsuario(int idUsuario)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter> { acceso.CrearParametro("@Id_Usuario", idUsuario) };
			DataTable tabla = acceso.Leer("OBTENER_IDIOMA_PREFERIDO_USUARIO", parametros);
			acceso.Cerrar();

			if (tabla.Rows.Count == 0) return null;

			DataRow fila = tabla.Rows[0];
			return new Idioma
			{
				Id_Idioma = int.Parse(fila["Id_Idioma"].ToString()),
				Codigo = fila.Table.Columns.Contains("Codigo_Idioma") ? fila["Codigo_Idioma"].ToString() : string.Empty,
				Nombre = fila.Table.Columns.Contains("Nombre_Idioma") ? fila["Nombre_Idioma"].ToString() : string.Empty,
				Habilitado = fila.Table.Columns.Contains("Habilitado_Idioma") ? int.Parse(fila["Habilitado_Idioma"].ToString()) : 1,
				EsDefault = ConvertBooleano(fila, "EsDefault")
			};
		}

		// Lista idiomas asociados a usuario (SP: LISTAR_IDIOMAS_POR_USUARIO)
		public List<Idioma> ListarPorUsuario(int idUsuario)
		{
			List<Idioma> idiomas = new List<Idioma>();
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter> { acceso.CrearParametro("@Id_Usuario", idUsuario) };
			DataTable tabla = acceso.Leer("LISTAR_IDIOMAS_POR_USUARIO", parametros);
			acceso.Cerrar();

			foreach (DataRow fila in tabla.Rows)
			{
				idiomas.Add(new Idioma
				{
					Id_Idioma = int.Parse(fila["Id_Idioma"].ToString()),
					Nombre = fila.Table.Columns.Contains("Nombre_Idioma") ? fila["Nombre_Idioma"].ToString() : string.Empty,
					Codigo = fila.Table.Columns.Contains("Codigo_Idioma") ? fila["Codigo_Idioma"].ToString() : string.Empty,
					Habilitado = fila.Table.Columns.Contains("Habilitado_Idioma") ? int.Parse(fila["Habilitado_Idioma"].ToString()) : 1,
					EsDefault = ConvertBooleano(fila, "EsDefault")
				});
			}

			return idiomas;
		}

		// Inserta relación usuario-idioma (SP: INSERTAR_USUARIO_IDIOMA)
		public int InsertarUsuarioIdioma(int idUsuario, int idIdioma)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>
			{
				acceso.CrearParametro("@Id_Usuario", idUsuario),
				acceso.CrearParametro("@Id_Idioma", idIdioma)
			};
			int resultado = acceso.Escribir("INSERTAR_USUARIO_IDIOMA", parametros);
			acceso.Cerrar();
			return resultado;
		}

		// Borra relación usuario-idioma (SP: BORRAR_USUARIO_IDIOMA)
		public int BorrarUsuarioIdioma(int idUsuario, int idIdioma)
		{
			ACCESO acceso = new ACCESO();
			acceso.Abrir();
			var parametros = new List<SqlParameter>
			{
				acceso.CrearParametro("@Id_Usuario", idUsuario),
				acceso.CrearParametro("@Id_Idioma", idIdioma)
			};
			int resultado = acceso.Escribir("BORRAR_USUARIO_IDIOMA", parametros);
			acceso.Cerrar();
			return resultado;
		}

		private int ConvertHabilitado(DataRow fila, string column)
		{
			if (!fila.Table.Columns.Contains(column) || fila.IsNull(column)) return 1;
			var val = fila[column];
			if (val is bool b) return b ? 1 : 0;
			if (val is int i) return i;
			if (int.TryParse(val.ToString(), out int parsed)) return parsed;
			return 1; // valor por defecto prudente
		}

		private bool ConvertBooleano(DataRow fila, string columna)
		{
			if (!fila.Table.Columns.Contains(columna) || fila.IsNull(columna)) return false;
			object valor = fila[columna];
			if (valor is bool) return (bool)valor;
			if (valor is int) return (int)valor != 0;
			bool resultadoBooleano;
			if (bool.TryParse(valor.ToString(), out resultadoBooleano)) return resultadoBooleano;
			int resultadoEntero;
			return int.TryParse(valor.ToString(), out resultadoEntero) && resultadoEntero != 0;
		}
	}
}
