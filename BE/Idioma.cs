using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
	public class Idioma
	{
		private int id_idioma;

		public int Id_Idioma
		{
			get { return id_idioma; }
			set { id_idioma = value; }
		}

		private string nombre;

		public string Nombre
		{
			get { return nombre; }
			set { nombre = value; }
		}

		private string codigo;

		/// <summary>
		/// Código corto del idioma (p. ej. "es", "en").
		/// </summary>
		public string Codigo
		{
			get { return codigo; }
			set { codigo = value; }
		}

		private int habilitado;

		/// <summary>
		/// Indica si el idioma está habilitado (1) o no (0).
		/// </summary>
		public int Habilitado
		{
			get { return habilitado; }
			set { habilitado = value; }
		}
	}
}
