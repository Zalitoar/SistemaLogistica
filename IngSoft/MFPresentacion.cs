using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BE;
using Servicios;

namespace IngSoft
{
    internal static class MFPresentacion
    {
        internal static DataGridView Grilla(string nombre, params string[] campos)
        {
            var g = new DataGridView
            {
                Name = nombre,
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };
            foreach (var campo in campos)
            {
                DataGridViewColumn columna = new[]
                {
                    "Disponible",
                    "Vigente",
                    "RequiereMantenimiento",
                    "Activa"
                }.Contains(campo) ? (DataGridViewColumn)new DataGridViewCheckBoxColumn() : new DataGridViewTextBoxColumn();
                columna.Name = campo;
                columna.DataPropertyName = campo;
                columna.HeaderText = campo;
                columna.MinimumWidth = 75;
                g.Columns.Add(columna);
            }

            g.CellFormatting += (s, e) =>
            {
                if (e.Value is string)
                {
                    string clave = "MF.Valor." + (string)e.Value;
                    string texto = IdiomaManager.GetInstance().Traducir(clave);
                    if (texto != clave)
                    {
                        e.Value = texto;
                        e.FormattingApplied = true;
                    }
                }
            };
            return g;
        }

        internal static FlowLayoutPanel Barra()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = true,
                Padding = new Padding(3)
            };
        }

        internal static TextBox Texto(string nombre, int max, int ancho = 240)
        {
            return new TextBox
            {
                Name = nombre,
                MaxLength = max,
                Width = ancho
            };
        }

        internal static NumericUpDown Numero(string nombre, decimal max, int decimales = 3)
        {
            return new NumericUpDown
            {
                Name = nombre,
                Maximum = max,
                DecimalPlaces = decimales,
                Width = 105
            };
        }

        internal static ComboBox Opciones(string nombre, params string[] valores)
        {
            var c = new ComboBox
            {
                Name = nombre,
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Value",
                ValueMember = "Key",
                Tag = valores
            };
            TraducirOpciones(c);
            return c;
        }

        private static void TraducirOpciones(ComboBox combo)
        {
            string seleccion = combo.SelectedValue as string;
            var valores = (string[])combo.Tag;
            combo.DataSource = valores.Select(v =>
            {
                string clave = "MF.Valor." + v;
                string texto = IdiomaManager.GetInstance().Traducir(clave);
                return new KeyValuePair<string, string>(v, texto == clave ? v : texto);
            }).ToList();
            if (seleccion != null)
                combo.SelectedValue = seleccion;
        }

        internal static DateTimePicker Fecha(string nombre)
        {
            return new DateTimePicker
            {
                Name = nombre,
                Width = 180,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm"
            };
        }

        internal static void Campo(FlowLayoutPanel barra, string nombre, string titulo, Control control)
        {
            var par = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(2)
            };
            par.Controls.Add(new Label { Name = nombre, Text = titulo, AutoSize = true, Margin = new Padding(3, 6, 3, 3) });
            par.Controls.Add(control);
            barra.Controls.Add(par);
        }

        internal static Button Boton(FlowLayoutPanel barra, string nombre, string texto, EventHandler accion)
        {
            var b = new Button
            {
                Name = nombre,
                Text = texto,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(80, 27),
                Padding = new Padding(4, 0, 4, 0)
            };
            b.Click += accion;
            barra.Controls.Add(b);
            return b;
        }

        internal static GroupBox Grupo(string nombre, string titulo, Control contenido, bool auto = false)
        {
            var g = new GroupBox
            {
                Name = nombre,
                Text = titulo,
                Dock = DockStyle.Fill,
                AutoSize = auto,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            g.Controls.Add(contenido);
            if (auto && contenido is FlowLayoutPanel)
                TDPresentacion.AjustarAltura(g, (FlowLayoutPanel)contenido);
            return g;
        }

        internal static TableLayoutPanel Disponer(Form form, params GroupBox[] grupos)
        {
            form.AutoScaleMode = AutoScaleMode.Font;
            form.ClientSize = new Size(1120, 720);
            form.MinimumSize = new Size(940, 650);
            var t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = grupos.Length,
                Padding = new Padding(4)
            };
            foreach (var g in grupos)
            {
                t.RowStyles.Add(new RowStyle(g.AutoSize ? SizeType.AutoSize : SizeType.Percent, g.AutoSize ? 0 : 100));
                t.Controls.Add(g, 0, t.Controls.Count);
            }

            form.Controls.Add(t);
            return t;
        }

        internal static T Seleccion<T>(DataGridView grid)
            where T : class
        {
            var item = grid.CurrentRow?.DataBoundItem as T;
            if (item == null)
                throw new ReglaMFException("MF.Seleccionar");
            return item;
        }

        internal static void AlSeleccionar(Form form, DataGridView grid, Action accion)
        {
            bool pendiente = false;
            grid.CurrentCellChanged += (s, e) =>
            {
                if (pendiente || !form.IsHandleCreated || form.IsDisposed)
                    return;
                pendiente = true;
                form.BeginInvoke(new Action(() =>
                {
                    pendiente = false;
                    if (!form.IsDisposed && !form.Disposing)
                        accion();
                }));
            };
        }

        internal static void Ejecutar(Form form, Action accion, Func<string, string, string> texto, bool guardar = false)
        {
            try
            {
                if (guardar && MessageBox.Show(form, texto("MF.Confirmar", "¿Confirma la operación?"), texto("MF.Titulo", "Flota"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                accion();
                if (guardar)
                    MessageBox.Show(form, texto("MF.Guardado", "Operación registrada."), texto("MF.Titulo", "Flota"));
            }
            catch (ReglaMFException ex)
            {
                MessageBox.Show(form, texto(ex.Message, ex.Message), texto("MF.Titulo", "Flota"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(form, texto("MF.Error", "No se pudo completar la operación. Actualice y vuelva a intentar."), texto("MF.Titulo", "Flota"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static void Formatos(Control raiz, Idioma idioma)
        {
            TDPresentacion.Formatos(raiz, idioma);
            foreach (Control c in raiz.Controls)
            {
                var g = c as DataGridView;
                if (g != null)
                    foreach (DataGridViewColumn col in g.Columns)
                    {
                        if (col.DataPropertyName.StartsWith("Fecha"))
                            col.DefaultCellStyle.Format = "g";
                        if (col.DataPropertyName.Contains("Kilometraje") || col.DataPropertyName == "Capacidad")
                            col.DefaultCellStyle.Format = "N3";
                    }

                var combo = c as ComboBox;
                if (combo != null && combo.Tag is string[])
                    TraducirOpciones(combo);
                if (c.HasChildren)
                    Formatos(c, idioma);
            }
        }
    }
}
