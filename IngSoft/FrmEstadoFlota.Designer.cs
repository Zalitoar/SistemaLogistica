using System.Windows.Forms;

namespace IngSoft
{
    partial class FrmEstadoFlota
    {
        private DataGridView dgvUnidades;
        private DataGridView dgvPartes;
        private TextBox txtDominio;
        private TextBox txtTipo;
        private TextBox txtMarca;
        private TextBox txtModelo;
        private NumericUpDown nudAnio;
        private NumericUpDown nudCapacidad;
        private NumericUpDown nudKmInicial;
        private NumericUpDown nudKm;
        private TextBox txtNovedad;
        private TextBox txtDescripcion;
        private CheckBox chkMantenimiento;
        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "FrmEstadoFlota";
            Text = "Registrar estado de flota";
            dgvUnidades = MFPresentacion.Grilla("dgvUnidades", "IdUnidadFlota", "Dominio", "Tipo", "Marca", "Modelo", "Anio", "Capacidad", "KilometrajeActual", "EstadoOperativo");
            var grpdgvUnidades = MFPresentacion.Grupo("grpdgvUnidades", "Unidades", dgvUnidades);
            dgvPartes = MFPresentacion.Grilla("dgvPartes", "IdParteEstadoUnidad", "FechaHora", "Kilometraje", "TipoNovedad", "Descripcion", "RequiereMantenimiento", "EstadoParte");
            var grpdgvPartes = MFPresentacion.Grupo("grpdgvPartes", "Partes de la unidad", dgvPartes);
            var alta = MFPresentacion.Barra();
            txtDominio = MFPresentacion.Texto("txtDominio", 10, 110);
            MFPresentacion.Campo(alta, "lbltxtDominio", "Dominio", txtDominio);
            txtTipo = MFPresentacion.Texto("txtTipo", 50, 110);
            MFPresentacion.Campo(alta, "lbltxtTipo", "Tipo", txtTipo);
            txtMarca = MFPresentacion.Texto("txtMarca", 50, 110);
            MFPresentacion.Campo(alta, "lbltxtMarca", "Marca", txtMarca);
            txtModelo = MFPresentacion.Texto("txtModelo", 50, 110);
            MFPresentacion.Campo(alta, "lbltxtModelo", "Modelo", txtModelo);
            nudAnio = MFPresentacion.Numero("nudAnio", 2100m, 0);
            MFPresentacion.Campo(alta, "lblnudAnio", "Año", nudAnio);
            nudCapacidad = MFPresentacion.Numero("nudCapacidad", 999999999m, 3);
            MFPresentacion.Campo(alta, "lblnudCapacidad", "Capacidad", nudCapacidad);
            nudKmInicial = MFPresentacion.Numero("nudKmInicial", 999999999m, 3);
            MFPresentacion.Campo(alta, "lblnudKmInicial", "Kilometraje inicial", nudKmInicial);
            MFPresentacion.Boton(alta, "btnCrear", "Crear unidad", (s, e) => Ejecutar(Crear, true));
            var grpalta = MFPresentacion.Grupo("grpalta", "Nueva unidad", alta, true);
            var estado = MFPresentacion.Barra();
            nudKm = MFPresentacion.Numero("nudKm", 999999999m, 3);
            MFPresentacion.Campo(estado, "lblnudKm", "Kilometraje", nudKm);
            txtNovedad = MFPresentacion.Texto("txtNovedad", 30, 140);
            MFPresentacion.Campo(estado, "lbltxtNovedad", "Novedad", txtNovedad);
            txtDescripcion = MFPresentacion.Texto("txtDescripcion", 500, 300);
            MFPresentacion.Campo(estado, "lbltxtDescripcion", "Descripción", txtDescripcion);
            chkMantenimiento = new CheckBox
            {
                Name = "chkMantenimiento",
                AutoSize = true
            };
            MFPresentacion.Campo(estado, "lblchkMantenimiento", "Requiere mantenimiento", chkMantenimiento);
            MFPresentacion.Boton(estado, "btnRegistrar", "Registrar estado", (s, e) => Ejecutar(Registrar, true));
            MFPresentacion.Boton(estado, "btnActualizar", "Actualizar", (s, e) => Ejecutar(Cargar, false));
            var grpestado = MFPresentacion.Grupo("grpestado", "Estado de la unidad seleccionada", estado, true);
            MFPresentacion.Disponer(this, grpdgvUnidades, grpdgvPartes, grpalta, grpestado);
            ResumeLayout(false);
        }
    }
}
