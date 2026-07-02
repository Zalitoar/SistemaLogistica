using Servicios;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft
{
    /// <summary>
    /// Base común para que cada formulario se actualice al cambiar el idioma.
    /// Se registra al cargarse y se desregistra al cerrarse, por lo que también
    /// traduce formularios que se abren después de haber elegido el idioma.
    /// </summary>
    public class FormularioTraducible : Form, IIdiomaObserver
    {
        protected FormularioTraducible()
        {
            Font = SystemFonts.MessageBoxFont;
            BackColor = Color.FromArgb(245, 247, 250);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AplicarEstiloVisual(Controls);

            IdiomaManager manager = IdiomaManager.GetInstance();
            manager.RegistrarObserver(this);
            ActualizarIdioma(manager.GetIdiomaActual());
        }

        private static void AplicarEstiloVisual(Control.ControlCollection controles)
        {
            foreach (Control control in controles)
            {
                DataGridView grilla = control as DataGridView;
                if (grilla != null)
                {
                    grilla.BackgroundColor = Color.White;
                    grilla.BorderStyle = BorderStyle.FixedSingle;
                    grilla.GridColor = Color.FromArgb(225, 228, 232);
                    grilla.RowHeadersVisible = false;
                    grilla.EnableHeadersVisualStyles = false;
                    grilla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 241);
                    grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(35, 45, 55);
                    grilla.ColumnHeadersDefaultCellStyle.Font = new Font(grilla.Font, FontStyle.Bold);
                    grilla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
                    grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }

                if (control.HasChildren)
                    AplicarEstiloVisual(control.Controls);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            IdiomaManager.GetInstance().QuitarObserver(this);
            base.OnFormClosed(e);
        }

        public virtual void ActualizarIdioma(BE.Idioma nuevoIdioma)
        {
            if (IsDisposed || Disposing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<BE.Idioma>(ActualizarIdioma), nuevoIdioma);
                return;
            }

            IdiomaManager manager = IdiomaManager.GetInstance();
            string formulario = string.IsNullOrWhiteSpace(Name) ? GetType().Name : Name;

            AsignarSiExiste(formulario + ".Title", valor => Text = valor, manager);
            TraducirControles(Controls, formulario, manager);
        }

        protected string ObtenerTexto(string clave, string valorPredeterminado)
        {
            string traduccion = IdiomaManager.GetInstance().Traducir(clave);
            return EsTraduccion(clave, traduccion) ? traduccion : valorPredeterminado;
        }

        private static void TraducirControles(Control.ControlCollection controles, string formulario, IdiomaManager manager)
        {
            foreach (Control control in controles)
            {
                if (!string.IsNullOrWhiteSpace(control.Name))
                {
                    string clave = formulario + "." + control.Name + ".Text";
                    AsignarSiExiste(clave, valor => control.Text = valor, manager);
                }

                DataGridView grilla = control as DataGridView;
                if (grilla != null)
                    TraducirColumnas(grilla, formulario, manager);

                ToolStrip barra = control as ToolStrip;
                if (barra != null)
                    TraducirItems(barra.Items, formulario, manager);

                if (control.ContextMenuStrip != null)
                    TraducirItems(control.ContextMenuStrip.Items, formulario, manager);

                if (control.HasChildren)
                    TraducirControles(control.Controls, formulario, manager);
            }
        }

        private static void TraducirColumnas(DataGridView grilla, string formulario, IdiomaManager manager)
        {
            if (string.IsNullOrWhiteSpace(grilla.Name))
                return;

            foreach (DataGridViewColumn columna in grilla.Columns)
            {
                string nombreColumna = !string.IsNullOrWhiteSpace(columna.DataPropertyName)
                    ? columna.DataPropertyName
                    : columna.Name;

                if (string.IsNullOrWhiteSpace(nombreColumna))
                    continue;

                string clave = formulario + "." + grilla.Name + "." + nombreColumna + ".HeaderText";
                AsignarSiExiste(clave, valor => columna.HeaderText = valor, manager);
            }
        }

        private static void TraducirItems(ToolStripItemCollection items, string formulario, IdiomaManager manager)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.Name) && !(item is ToolStripComboBox))
                {
                    string clave = formulario + "." + item.Name + ".Text";
                    AsignarSiExiste(clave, valor => item.Text = valor, manager);
                }

                ToolStripMenuItem menu = item as ToolStripMenuItem;
                if (menu != null && menu.DropDownItems.Count > 0)
                    TraducirItems(menu.DropDownItems, formulario, manager);
            }
        }

        private static void AsignarSiExiste(string clave, Action<string> asignar, IdiomaManager manager)
        {
            string traduccion = manager.Traducir(clave);
            if (EsTraduccion(clave, traduccion))
                asignar(traduccion);
        }

        private static bool EsTraduccion(string clave, string traduccion)
        {
            return !string.IsNullOrEmpty(traduccion)
                && !string.Equals(clave, traduccion, StringComparison.OrdinalIgnoreCase);
        }
    }
}
