using System;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace IngSoft
{
    public partial class FrmHabilitarUnidad : FormularioTraducible
    {
        private readonly GestorDisponibilidad gestor = new GestorDisponibilidad();
        public FrmHabilitarUnidad()
        {
            InitializeComponent();
            MFPresentacion.AlSeleccionar(this, dgvUnidades, () => Ejecutar(Mostrar));
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
            dgvUnidades.DataSource = gestor.Unidades();
            Mostrar();
        }

        private void Mostrar()
        {
            var u = dgvUnidades.CurrentRow?.DataBoundItem as UnidadFlota;
            dgvIntervenciones.DataSource = u == null ? null : gestor.Intervenciones(u.IdUnidadFlota);
            dgvInformes.DataSource = u == null ? null : gestor.Informes(u.IdUnidadFlota);
        }

        private void Habilitar()
        {
            gestor.Habilitar(MFPresentacion.Seleccion<UnidadFlota>(dgvUnidades).IdUnidadFlota, txtObservaciones.Text);
            Cargar();
        }
    }
}
