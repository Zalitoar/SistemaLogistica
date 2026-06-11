using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
	/// <summary>
	/// Implementar en formularios/controles que deban actualizar sus textos
	/// cuando cambie el idioma activo.
	/// </summary>
	public interface IIdiomaObserver
	{
		/// <summary>
		/// Se invoca cuando el idioma activo cambia.
		/// Implementación típica: volver a leer las traducciones necesarias y actualizar UI.
		/// </summary>
		/// <param name="nuevoIdioma">Idioma activo (puede ser null si no hay ninguno).</param>
		void ActualizarIdioma(BE.Idioma nuevoIdioma);
	}
}
