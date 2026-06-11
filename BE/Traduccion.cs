using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
	public class Traduccion
	{
		private int id_traduccion;

		public int Id_Traduccion
		{
			get { return id_traduccion; }
			set { id_traduccion = value; }
		}

		private int id_idioma;

		/// <summary>
		/// FK al idioma (BE.Idioma.Id_Idioma)
		/// </summary>
		public int Id_Idioma
		{
			get { return id_idioma; }
			set { id_idioma = value; }
		}

		private string clave;

		/// <summary>
		/// Identificador de la cadena (p. ej. "frmLogin.btnIngresar.Text").
		/// </summary>
		public string Clave
		{
			get { return clave; }
			set { clave = value; }
		}

		private string valor;

		/// <summary>
		/// Texto traducido.
		/// </summary>
		public string Valor
		{
			get { return valor; }
			set { valor = value; }
		}
	}
}
