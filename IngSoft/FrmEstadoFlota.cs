using System;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace IngSoft
{
    public partial class FrmEstadoFlota : FormularioTraducible
    {
        private readonly GestorFlota gestor = new GestorFlota();
        public FrmEstadoFlota()
        {
            InitializeComponent();
            MFPresentacion.AlSeleccionar(this, dgvUnidades, () => Ejecutar(Mostrar));
            nudAnio.Value = DateTime.Today.Year;
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
            dgvPartes.DataSource = u == null ? null : gestor.Partes(u.IdUnidadFlota);
            if (u != null)
                nudKm.Value = Math.Min(nudKm.Maximum, u.KilometrajeActual);
        }

        private void Crear()
        {
            gestor.Crear(new UnidadFlota { Dominio = txtDominio.Text, Tipo = txtTipo.Text, Marca = txtMarca.Text, Modelo = txtModelo.Text, Anio = (int)nudAnio.Value, Capacidad = nudCapacidad.Value, KilometrajeActual = nudKmInicial.Value });
            Cargar();
        }

        private void Registrar()
        {
            gestor.Registrar(new ParteEstadoUnidad { IdUnidadFlota = MFPresentacion.Seleccion<UnidadFlota>(dgvUnidades).IdUnidadFlota, Kilometraje = nudKm.Value, TipoNovedad = txtNovedad.Text, Descripcion = txtDescripcion.Text, RequiereMantenimiento = chkMantenimiento.Checked });
            Cargar();
        }
    }
}
