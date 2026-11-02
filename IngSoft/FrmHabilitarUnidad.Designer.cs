using System.Windows.Forms;

namespace IngSoft
{
    partial class FrmHabilitarUnidad
    {
        private DataGridView dgvUnidades;
        private DataGridView dgvIntervenciones;
        private DataGridView dgvInformes;
        private TextBox txtObservaciones;
        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "FrmHabilitarUnidad";
            Text = "Habilitar unidad para operar";
            dgvUnidades = MFPresentacion.Grilla("dgvUnidades", "IdUnidadFlota", "Dominio", "KilometrajeActual", "EstadoOperativo");
            var grpdgvUnidades = MFPresentacion.Grupo("grpdgvUnidades", "Unidades", dgvUnidades);
            dgvIntervenciones = MFPresentacion.Grilla("dgvIntervenciones", "IdOrdenMantenimiento", "IdIntervencionMantenimiento", "FechaFin", "TrabajoRealizado", "Resultado", "Observaciones");
            var grpdgvIntervenciones = MFPresentacion.Grupo("grpdgvIntervenciones", "Intervenciones de la unidad", dgvIntervenciones);
            dgvInformes = MFPresentacion.Grilla("dgvInformes", "IdInformeDisponibilidad", "FechaHora", "Disponible", "EstadoUnidad", "Observaciones", "Vigente");
            var grpdgvInformes = MFPresentacion.Grupo("grpdgvInformes", "Historial de disponibilidad", dgvInformes);
            var habilitacion = MFPresentacion.Barra();
            txtObservaciones = MFPresentacion.Texto("txtObservaciones", 1000, 450);
            MFPresentacion.Campo(habilitacion, "lbltxtObservaciones", "Observaciones", txtObservaciones);
            MFPresentacion.Boton(habilitacion, "btnHabilitar", "Confirmar habilitación", (s, e) => Ejecutar(Habilitar, true));
            MFPresentacion.Boton(habilitacion, "btnActualizar", "Actualizar", (s, e) => Ejecutar(Cargar, false));
            var grphabilitacion = MFPresentacion.Grupo("grphabilitacion", "Verificación de condiciones", habilitacion, true);
            MFPresentacion.Disponer(this, grpdgvUnidades, grpdgvIntervenciones, grpdgvInformes, grphabilitacion);
            ResumeLayout(false);
        }
    }
}
