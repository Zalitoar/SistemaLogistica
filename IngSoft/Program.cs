using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using BE;
using Servicios;

namespace IngSoft
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicializar idiomas y traducciones en IdiomaManager
            try
            {
                var bllIdioma = new BLL.Idioma();
                var listaIdiomas = bllIdioma.Listar() ?? new List<BE.Idioma>();
                IdiomaManager.GetInstance().CargarIdiomas(listaIdiomas);

                var listaTraducciones = bllIdioma.ListarTraducciones() ?? new List<BE.Traduccion>();
                IdiomaManager.GetInstance().CargarTraducciones(listaTraducciones);

                // Seleccionar idioma por defecto:
                BE.Idioma seleccionado = listaIdiomas
                    .FirstOrDefault(i => string.Equals(i.Codigo, "es", StringComparison.OrdinalIgnoreCase) && i.Habilitado == 1)
                    ?? listaIdiomas.FirstOrDefault(i => i.Habilitado == 1)
                    ?? listaIdiomas.FirstOrDefault();

                IdiomaManager.GetInstance().SetIdiomaActual(seleccionado);
            }
            catch (Exception ex)
            {
                try { BitacoraManager.Registrar("Error inicializando idiomas: " + ex.Message); } catch { }
            }

            Application.Run(new FrmLogin());
        }
    }
}
