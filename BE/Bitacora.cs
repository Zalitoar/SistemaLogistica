using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Bitacora
    {
		private int id_bitacora;

		public int Id_Bitacora
		{
			get { return id_bitacora; }
			set { id_bitacora = value; }
		}

		private DateTime fechahora;

		public DateTime FechaHora
		{
			get { return fechahora; }
			set { fechahora = value; }
		}

		private string usuario;

		public string Usuario
		{
			get { return usuario; }
			set { usuario = value; }
		}

		private string actividad;
			
		public string Actividad
		{
			get { return actividad; }
			set { actividad = value; }
		}


	}
}
