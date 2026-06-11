using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
	/// <summary>
	/// Manager singleton para el manejo de idiomas y traducciones.
	/// - Mantiene idioma activo.
	/// - Mantiene en memoria las traducciones cargadas desde DAL.
	/// - Permite registrar observadores (formularios) que serán notificados al cambiar el idioma.
	/// NOTA: La persistencia (DAL) debe implementar la carga y llamar a CargarIdiomas / CargarTraducciones.
	/// </summary>
	public class IdiomaManager
	{
		private static IdiomaManager instance;
			
		private List<IIdiomaObserver> observers;
		private List<BE.Idioma> idiomas;
		private List<BE.Traduccion> traducciones;

		private BE.Idioma idiomaActual;

		private IdiomaManager()
		{
			observers = new List<IIdiomaObserver>();
			idiomas = new List<BE.Idioma>();
			traducciones = new List<BE.Traduccion>();
			idiomaActual = null;
		}

		public static IdiomaManager GetInstance()
		{
			if (instance == null)
			{
				instance = new IdiomaManager();
			}
			return instance;
		}

		#region Observers

		public void RegistrarObserver(IIdiomaObserver obs)
		{
			if (obs == null) return;
			if (!observers.Contains(obs))
				observers.Add(obs);
		}

		public void QuitarObserver(IIdiomaObserver obs)
		{
			if (obs == null) return;
			if (observers.Contains(obs))
				observers.Remove(obs);
		}

		private void NotificarObservers()
		{
			// Copiamos la lista para evitar modificación durante la notificación
			var copia = observers.ToArray();
			foreach (var obs in copia)
			{
				try
				{
					obs.ActualizarIdioma(idiomaActual);
				}
				catch
				{
					// No propagar excepciones de un observer; registro depuración en DAL/Bitácora si se desea
				}
			}
		}

		#endregion

		#region Idiomas / Traducciones en memoria

		/// <summary>
		/// Método que DAL puede invocar para cargar los idiomas disponibles desde la base de datos.
		/// Reemplaza la lista interna.
		/// </summary>
		/// <param name="listaIdiomas"></param>
		public void CargarIdiomas(List<BE.Idioma> listaIdiomas)
		{
			if (listaIdiomas == null) listaIdiomas = new List<BE.Idioma>();
			idiomas = listaIdiomas;
		}

		/// <summary>
		/// Método que DAL puede invocar para cargar todas las traducciones desde la base de datos.
		/// Reemplaza la lista interna.
		/// </summary>
		/// <param name="listaTraducciones"></param>
		public void CargarTraducciones(List<BE.Traduccion> listaTraducciones)
		{
			if (listaTraducciones == null) listaTraducciones = new List<BE.Traduccion>();
			traducciones = listaTraducciones;
		}

		/// <summary>
		/// Obtiene la lista de idiomas cargados.
		/// </summary>
		/// <returns></returns>
		public List<BE.Idioma> ListarIdiomas()
		{
			// Devolvemos copia para evitar modificaciones externas
			return new List<BE.Idioma>(idiomas);
		}

		/// <summary>
		/// Obtiene las traducciones cargadas (copia).
		/// </summary>
		/// <returns></returns>
		public List<BE.Traduccion> ListarTraducciones()
		{
			return new List<BE.Traduccion>(traducciones);
		}

		#endregion

		#region Idioma actual y traducción

		public BE.Idioma GetIdiomaActual()
		{
			return idiomaActual;
		}

		/// <summary>
		/// Establece el idioma activo y notifica a los observers.
		/// Si el idioma pasado es null, el idioma activo se desactiva.
		/// </summary>
		/// <param name="idioma"></param>
		public void SetIdiomaActual(BE.Idioma idioma)
		{
			idiomaActual = idioma;
			NotificarObservers();
		}

		/// <summary>
		/// Traduce una clave según el idioma activo.
		/// Si no existe traducción, devuelve la clave como fallback.
		/// </summary>
		/// <param name="clave">Clave de traducción (ej. "frmLogin.btnIngresar.Text")</param>
		/// <returns>Texto traducido o la clave si no hay traducción.</returns>
		public string Traducir(string clave)
		{
			if (string.IsNullOrEmpty(clave)) return string.Empty;
			if (idiomaActual == null) return clave;

			// Buscar traducción en memoria
			var t = traducciones.FirstOrDefault(x => string.Equals(x.Clave, clave, StringComparison.OrdinalIgnoreCase)
													&& x.Id_Idioma == idiomaActual.Id_Idioma);
			if (t != null && !string.IsNullOrEmpty(t.Valor))
				return t.Valor;

			// Fallback: devolver clave
			return clave;
		}

		#endregion
	}
}
