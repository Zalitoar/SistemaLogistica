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

		// Preferencias de usuario
		public BE.Idioma ObtenerIdiomaPreferido(int idUsuario)
		{
			DAL.MP_Idioma mp = new DAL.MP_Idioma();
			return mp.ObtenerIdiomaPreferidoUsuario(idUsuario);
		}

		public List<BE.Idioma> ListarIdiomasPorUsuario(int idUsuario)
		{
			DAL.MP_Idioma mp = new DAL.MP_Idioma();
			return mp.ListarPorUsuario(idUsuario);
		}

		/// <summary>
		/// Establece la preferencia única del usuario eliminando otras y añadiendo la seleccionada.
		/// </summary>
		public void SetIdiomaPreferidoUsuario(int idUsuario, int idIdioma)
		{
			DAL.MP_Idioma mp = new DAL.MP_Idioma();
			// Eliminar relaciones existentes
			var existentes = mp.ListarPorUsuario(idUsuario);
			foreach (var ex in existentes)
			{
				if (ex.Id_Idioma != idIdioma)
				{
					mp.BorrarUsuarioIdioma(idUsuario, ex.Id_Idioma);
				}
			}
			// Insertar si no existe
			mp.InsertarUsuarioIdioma(idUsuario, idIdioma);
		}
	}
}
