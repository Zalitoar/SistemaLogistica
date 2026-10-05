using System.Windows.Forms;

namespace IngSoft
{
    partial class FrmProgramarMantenimiento
    {
        private DataGridView dgvPartes;
        private DataGridView dgvOrdenes;
        private DateTimePicker dtpProgramada;
        private ComboBox cmbTipo;
        private ComboBox cmbPrioridad;
        private TextBox txtTrabajo;
        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "FrmProgramarMantenimiento";
            Text = "Programar mantenimiento";
            dgvPartes = MFPresentacion.Grilla("dgvPartes", "IdParteEstadoUnidad", "Dominio", "FechaHora", "Kilometraje", "TipoNovedad", "Descripcion");
            var grpdgvPartes = MFPresentacion.Grupo("grpdgvPartes", "Partes pendientes", dgvPartes);
            dgvOrdenes = MFPresentacion.Grilla("dgvOrdenes", "IdOrdenMantenimiento", "Dominio", "FechaProgramada", "TipoMantenimiento", "Prioridad", "DescripcionTrabajo", "EstadoOrden");
            var grpdgvOrdenes = MFPresentacion.Grupo("grpdgvOrdenes", "Órdenes de mantenimiento", dgvOrdenes);
            var programa = MFPresentacion.Barra();
            dtpProgramada = MFPresentacion.Fecha("dtpProgramada");
            MFPresentacion.Campo(programa, "lbldtpProgramada", "Fecha programada", dtpProgramada);
            cmbTipo = MFPresentacion.Opciones("cmbTipo", "Preventivo", "Correctivo");
            MFPresentacion.Campo(programa, "lblcmbTipo", "Tipo", cmbTipo);
            cmbPrioridad = MFPresentacion.Opciones("cmbPrioridad", "Baja", "Media", "Alta", "Urgente");
            MFPresentacion.Campo(programa, "lblcmbPrioridad", "Prioridad", cmbPrioridad);
            txtTrabajo = MFPresentacion.Texto("txtTrabajo", 500, 340);
            MFPresentacion.Campo(programa, "lbltxtTrabajo", "Trabajo previsto", txtTrabajo);
            MFPresentacion.Boton(programa, "btnProgramar", "Programar", (s, e) => Ejecutar(Programar, true));
            MFPresentacion.Boton(programa, "btnActualizar", "Actualizar", (s, e) => Ejecutar(Cargar, false));
            var grpprograma = MFPresentacion.Grupo("grpprograma", "Programación del parte seleccionado", programa, true);
            MFPresentacion.Disponer(this, grpdgvPartes, grpdgvOrdenes, grpprograma);
            ResumeLayout(false);
        }
    }
}
