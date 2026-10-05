using System;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace IngSoft
{
    public partial class FrmDisponibilidadFlota : FormularioTraducible
    {
        private readonly GestorDisponibilidad gestor = new GestorDisponibilidad();
        public FrmDisponibilidadFlota()
        {
            InitializeComponent();
            MFPresentacion.AlSeleccionar(this, dgvViajes, () => Ejecutar(Mostrar));
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
            dgvViajes.DataSource = gestor.Viajes();
            Mostrar();
        }

        private void Mostrar()
        {
            var v = dgvViajes.CurrentRow?.DataBoundItem as Viaje;
            var unidades = gestor.Disponibles(v?.IdViaje);
            dgvUnidades.DataSource = unidades;
            dgvInformes.DataSource = gestor.InformesDisponibles().Where(i => unidades.Any(u => u.IdUnidadFlota == i.IdUnidadFlota)).ToList();
        }

        private void Asignar()
        {
            gestor.Asignar(MFPresentacion.Seleccion<UnidadFlota>(dgvUnidades).IdUnidadFlota, MFPresentacion.Seleccion<Viaje>(dgvViajes).IdViaje);
            Cargar();
        }
    }
}
