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

            Application.ThreadException += (s, e) => MessageBox.Show(
                FrmConfiguracionBD.DescribirError(e.Exception),
                "SistemaLogistica", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (!ConfigurarAntesDelLogin()) return;

            // Inicializar idiomas y traducciones en IdiomaManager
            try
            {
                var bllIdioma = new BLL.Idioma();
                var listaIdiomas = bllIdioma.Listar() ?? new List<BE.Idioma>();
                IdiomaManager.GetInstance().CargarIdiomas(listaIdiomas);

                var listaTraducciones = bllIdioma.ListarTraducciones() ?? new List<BE.Traduccion>();
                IdiomaManager.GetInstance().CargarTraducciones(listaTraducciones);

                // Seleccionar el idioma predeterminado configurado en la base.
                BE.Idioma seleccionado = listaIdiomas
                    .FirstOrDefault(i => i.EsDefault && i.Habilitado == 1)
                    ?? listaIdiomas.FirstOrDefault(i => i.Habilitado == 1)
                    ?? listaIdiomas.FirstOrDefault();

                IdiomaManager.GetInstance().SetIdiomaActual(seleccionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show(FrmConfiguracionBD.DescribirError(ex),
                    "SistemaLogistica", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new FrmLogin());
        }

        private static bool ConfigurarAntesDelLogin()
        {
            var gestor = new ConfiguracionBDManager();
            while (true)
            {
                string error;
                try
                {
                    if (gestor.VerificarInicio().Compatible) return true;
                    error = "BD.Incompatible";
                }
                catch (Exception ex) { error = ConfiguracionBDManager.ClaveError(ex); }
                using (var formulario = new FrmConfiguracionBD(true, error))
                    if (formulario.ShowDialog() != DialogResult.OK) return false;
                try { gestor.ActivarAlInicio(); }
                catch (Exception ex)
                {
                    MessageBox.Show(FrmConfiguracionBD.DescribirError(ex));
                    return false;
                }
            }
        }
    }
}
