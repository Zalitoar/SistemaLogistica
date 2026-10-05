using System;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace IngSoft
{
    public partial class FrmProgramarMantenimiento : FormularioTraducible
    {
        private readonly GestorMantenimiento gestor = new GestorMantenimiento();
        public FrmProgramarMantenimiento()
        {
            InitializeComponent();
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
            dgvPartes.DataSource = gestor.PartesPendientes();
            dgvOrdenes.DataSource = gestor.OrdenesProgramacion();
        }

        private void Programar()
        {
            gestor.Programar(new OrdenMantenimiento { IdParteEstadoUnidad = MFPresentacion.Seleccion<ParteEstadoUnidad>(dgvPartes).IdParteEstadoUnidad, FechaProgramada = dtpProgramada.Value, TipoMantenimiento = (string)cmbTipo.SelectedValue, Prioridad = (string)cmbPrioridad.SelectedValue, DescripcionTrabajo = txtTrabajo.Text });
            Cargar();
        }
    }
}
