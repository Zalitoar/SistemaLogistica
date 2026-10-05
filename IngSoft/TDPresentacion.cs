using System;
using System.Globalization;
using System.Windows.Forms;
using BE;

namespace IngSoft
{
    internal static class TDPresentacion
    {
        internal static CultureInfo Cultura(Idioma idioma)
        {
            try { return CultureInfo.GetCultureInfo(idioma?.Codigo ?? "es-AR"); }
            catch (CultureNotFoundException) { return CultureInfo.CurrentCulture; }
        }
        internal static void Formatos(Control raiz, Idioma idioma)
        {
            var cultura = Cultura(idioma);
            foreach (Control control in raiz.Controls)
            {
                var grilla = control as DataGridView;
                if (grilla != null)
                    foreach (DataGridViewColumn columna in grilla.Columns)
                        columna.DefaultCellStyle.FormatProvider = cultura;
                var fecha = control as DateTimePicker;
                if (fecha != null) fecha.CustomFormat = cultura.DateTimeFormat.ShortDatePattern + " " + cultura.DateTimeFormat.ShortTimePattern;
                if (control.HasChildren) Formatos(control, idioma);
            }
        }
        internal static void TraducirEstados(DataGridView grilla, Func<string, string, string> texto)
        {
            grilla.CellFormatting += (s, e) =>
            {
                string propiedad = grilla.Columns[e.ColumnIndex].DataPropertyName;
                if ((propiedad == "Estado" || propiedad == "Resultado") && e.Value is string)
                {
                    e.Value = texto("TD.Estado." + e.Value, (string)e.Value);
                    e.FormattingApplied = true;
                }
            };
        }
        internal static void Ejecutar(IWin32Window ventana, Action accion, Func<string, string, string> texto)
        {
            try { accion(); }
            catch (ReglaTDException ex)
            {
                MessageBox.Show(ventana, texto(ex.Message, ex.Message), texto("Common.msgError", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(ventana, texto("TD.Error", "No se pudo completar la operación."), texto("Common.msgError", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        internal static bool Confirmar(IWin32Window ventana, Func<string, string, string> texto)
        {
            return MessageBox.Show(ventana, texto("TD.Confirmar", "¿Confirma la operación?"), texto("Common.msgConfirmar", "Confirmar"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        internal static void Guardado(IWin32Window ventana, Func<string, string, string> texto)
        {
            MessageBox.Show(ventana, texto("TD.Guardado", "Operación registrada correctamente."));
        }
        internal static T Seleccion<T>(DataGridView grilla) where T : class
        {
            var item = grilla.CurrentRow?.DataBoundItem as T;
            if (item == null) throw new ReglaTDException("TD.Seleccionar");
            return item;
        }
        internal static void ErrorDato(IWin32Window ventana, DataGridViewDataErrorEventArgs e, Func<string, string, string> texto)
        {
            e.ThrowException = false;
            e.Cancel = true;
            MessageBox.Show(ventana, texto("TD.Formato", "Formato inválido."));
        }
    }
}
