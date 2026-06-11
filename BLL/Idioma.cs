using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
	public class Idioma
	{
		public List<BE.Idioma> Listar()
		{
			DAL.MP_Idioma mp = new DAL.MP_Idioma();
			return mp.Listar();
		}

		public int Grabar(BE.Idioma obj)
		{
			DAL.MP_Idioma mp = new DAL.MP_Idioma();
			if (obj.Id_Idioma == 0)
			{
				return mp.Insertar(obj);
			}
			else
			{
				return mp.Editar(obj);
			}
		}

		public int Borrar(BE.Idioma obj)
		{
			DAL.MP_Idioma mp = new DAL.MP_Idioma();
			return mp.Borrar(obj);
		}

		// Traducciones
		public List<BE.Traduccion> ListarTraducciones()
		{
			DAL.MP_Traduccion mp = new DAL.MP_Traduccion();
			return mp.Listar();
		}

		public List<BE.Traduccion> ListarTraduccionesPorIdioma(int idIdioma)
		{
			DAL.MP_Traduccion mp = new DAL.MP_Traduccion();
			return mp.ListarPorIdioma(idIdioma);
		}

		public int GrabarTraduccion(BE.Traduccion t)
		{
			DAL.MP_Traduccion mp = new DAL.MP_Traduccion();
			if (t.Id_Traduccion == 0)
				return mp.Insertar(t);
			else
				return mp.Editar(t);
		}

		public int BorrarTraduccion(BE.Traduccion t)
		{
			DAL.MP_Traduccion mp = new DAL.MP_Traduccion();
			return mp.Borrar(t);
		}
	}
}
