using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Usuario
    {
		private int id_usuario;

		public int Id_Usuario
		{
			get { return id_usuario; }
			set { id_usuario = value; }
		}

		private string nombre;

		public string Nombre
		{
			get { return nombre; }
			set { nombre = value; }
		}

		private string clave;

		public string Clave
		{
			get { return clave; }
			set { clave = value; }
		}

		private int id_perfil;

		public int Id_Perfil
		{
			get { return id_perfil; }
			set { id_perfil = value; }
		}

		private string descripcionPerfil;

		public string DescripcionPerfil
		{
			get { return descripcionPerfil; }
			set { descripcionPerfil = value; }
		}





	}
}
