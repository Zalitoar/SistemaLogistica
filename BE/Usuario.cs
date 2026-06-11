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

		private int id_rol;

		public int Id_Rol
		{
			get { return id_rol; }
			set { id_rol = value; }
		}

		private string descripcionPerfil;

		public string DescripcionPerfil
		{
			get { return descripcionPerfil; }
			set { descripcionPerfil = value; }
		}

		private int borrado;

		public int Borrado
		{
			get { return borrado; }
			set { borrado = value; }
		}


		private string dvh;

		public string DVH
		{
			get { return dvh; }
			set { dvh = value; }
		}




	}
}
