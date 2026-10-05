using System.Windows.Forms;

namespace IngSoft
{
    partial class FrmIntervencionMantenimiento
    {
        private DataGridView dgvOrdenes;
        private DataGridView dgvIntervenciones;
        private NumericUpDown nudKm;
        private TextBox txtTrabajo;
        private ComboBox cmbResultado;
        private TextBox txtObservaciones;
        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "FrmIntervencionMantenimiento";
            Text = "Registrar intervención de mantenimiento";
            dgvOrdenes = MFPresentacion.Grilla("dgvOrdenes", "IdOrdenMantenimiento", "Dominio", "FechaProgramada", "TipoMantenimiento", "Prioridad", "DescripcionTrabajo", "EstadoOrden");
            var grpdgvOrdenes = MFPresentacion.Grupo("grpdgvOrdenes", "Órdenes de mantenimiento", dgvOrdenes);
            dgvIntervenciones = MFPresentacion.Grilla("dgvIntervenciones", "IdIntervencionMantenimiento", "FechaInicio", "FechaFin", "Kilometraje", "TrabajoRealizado", "Resultado", "Observaciones");
            var grpdgvIntervenciones = MFPresentacion.Grupo("grpdgvIntervenciones", "Intervenciones de la orden", dgvIntervenciones);
            var intervencion = MFPresentacion.Barra();
            nudKm = MFPresentacion.Numero("nudKm", 999999999m, 3);
            MFPresentacion.Campo(intervencion, "lblnudKm", "Kilometraje", nudKm);
            txtTrabajo = MFPresentacion.Texto("txtTrabajo", 1000, 300);
            MFPresentacion.Campo(intervencion, "lbltxtTrabajo", "Trabajo realizado", txtTrabajo);
            cmbResultado = MFPresentacion.Opciones("cmbResultado", "Satisfactoria", "RequiereTareas");
            MFPresentacion.Campo(intervencion, "lblcmbResultado", "Resultado", cmbResultado);
            txtObservaciones = MFPresentacion.Texto("txtObservaciones", 1000, 300);
            MFPresentacion.Campo(intervencion, "lbltxtObservaciones", "Observaciones", txtObservaciones);
            MFPresentacion.Boton(intervencion, "btnIniciar", "Iniciar intervención", (s, e) => Ejecutar(Iniciar, true));
            MFPresentacion.Boton(intervencion, "btnFinalizar", "Finalizar intervención", (s, e) => Ejecutar(Finalizar, true));
            MFPresentacion.Boton(intervencion, "btnActualizar", "Actualizar", (s, e) => Ejecutar(Cargar, false));
            var grpintervencion = MFPresentacion.Grupo("grpintervencion", "Tareas y resultado", intervencion, true);
            MFPresentacion.Disponer(this, grpdgvOrdenes, grpdgvIntervenciones, grpintervencion);
            ResumeLayout(false);
        }
    }
}
