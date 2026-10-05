using System.Windows.Forms;

namespace IngSoft
{
    partial class FrmDisponibilidadFlota
    {
        private DataGridView dgvViajes;
        private DataGridView dgvUnidades;
        private DataGridView dgvInformes;
        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "FrmDisponibilidadFlota";
            Text = "Consultar disponibilidad y asignar unidad";
            dgvViajes = MFPresentacion.Grilla("dgvViajes", "IdViaje", "Numero", "FechaPrevista", "Estado", "IdUnidadFlota", "IdInformeDisponibilidad");
            var grpdgvViajes = MFPresentacion.Grupo("grpdgvViajes", "Viajes por iniciar", dgvViajes);
            dgvUnidades = MFPresentacion.Grilla("dgvUnidades", "IdUnidadFlota", "Dominio", "Tipo", "Capacidad", "KilometrajeActual", "EstadoOperativo");
            var grpdgvUnidades = MFPresentacion.Grupo("grpdgvUnidades", "Unidades disponibles para el viaje seleccionado", dgvUnidades);
            dgvInformes = MFPresentacion.Grilla("dgvInformes", "IdInformeDisponibilidad", "IdUnidadFlota", "FechaHora", "Disponible", "Observaciones");
            var grpdgvInformes = MFPresentacion.Grupo("grpdgvInformes", "Informes vigentes de las unidades disponibles", dgvInformes);
            var asignacion = MFPresentacion.Barra();
            MFPresentacion.Boton(asignacion, "btnAsignar", "Asignar unidad", (s, e) => Ejecutar(Asignar, true));
            MFPresentacion.Boton(asignacion, "btnActualizar", "Actualizar", (s, e) => Ejecutar(Cargar, false));
            var grpasignacion = MFPresentacion.Grupo("grpasignacion", "Asignación de unidad al viaje seleccionado", asignacion, true);
            MFPresentacion.Disponer(this, grpdgvViajes, grpdgvUnidades, grpdgvInformes, grpasignacion);
            ResumeLayout(false);
        }
    }
}
