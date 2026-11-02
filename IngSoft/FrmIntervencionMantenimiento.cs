using System;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace IngSoft
{
    public partial class FrmIntervencionMantenimiento : FormularioTraducible
    {
        private readonly GestorMantenimiento gestor = new GestorMantenimiento();
        public FrmIntervencionMantenimiento()
        {
            InitializeComponent();
            MFPresentacion.AlSeleccionar(this, dgvOrdenes, () => Ejecutar(Mostrar));
            MFPresentacion.AlSeleccionar(this, dgvIntervenciones, () => Ejecutar(MostrarIntervencion));
            Load += (s, e) => Ejecutar(Cargar);
        }

        private void Ejecutar(Action accion, bool guardar = false)
        {
            MFPresentacion.Ejecutar(this, accion, ObtenerTexto, guardar);
        }

        public override void ActualizarIdioma(BE.Idioma idioma)
        {
            base.ActualizarIdioma(idioma);
            MFPresentacion.Formatos(this, idioma);
        }

        private void Cargar()
        {
            dgvOrdenes.DataSource = gestor.Ordenes();
            Mostrar();
        }

        private void Mostrar()
        {
            var o = dgvOrdenes.CurrentRow?.DataBoundItem as OrdenMantenimiento;
            dgvIntervenciones.DataSource = o == null ? null : gestor.Intervenciones(o.IdOrdenMantenimiento);
            MostrarIntervencion();
        }

        private void MostrarIntervencion()
        {
            var i = dgvIntervenciones.CurrentRow?.DataBoundItem as IntervencionMantenimiento;
            if (i != null)
            {
                nudKm.Value = Math.Min(nudKm.Maximum, i.Kilometraje);
                txtTrabajo.Text = i.TrabajoRealizado;
                txtObservaciones.Text = i.Observaciones;
            }
        }

        private void Iniciar()
        {
            gestor.Iniciar(MFPresentacion.Seleccion<OrdenMantenimiento>(dgvOrdenes).IdOrdenMantenimiento);
            Mostrar();
        }

        private void Finalizar()
        {
            gestor.Finalizar(new IntervencionMantenimiento { IdIntervencionMantenimiento = MFPresentacion.Seleccion<IntervencionMantenimiento>(dgvIntervenciones).IdIntervencionMantenimiento, Kilometraje = nudKm.Value, TrabajoRealizado = txtTrabajo.Text, Resultado = (string)cmbResultado.SelectedValue, Observaciones = txtObservaciones.Text });
            Cargar();
        }
    }
}
