using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace IngSoft
{
    public partial class FrmPlanificacionDistribucion : FormularioTraducible
    {
        private readonly GestorPlanificacion gestor = new GestorPlanificacion();
        private readonly BindingList<Viaje> viajes = new BindingList<Viaje>();
        public FrmPlanificacionDistribucion()
        {
            InitializeComponent();
            cmbRequerimiento.SelectedIndexChanged += (s, e) => MostrarOrigen();
            txtOrigenSeleccionado.ReadOnly = true;
            dgvViajes.DataSource = viajes;
            dgvViajes.CurrentCellChanged += (s, e) => MostrarViaje();
            foreach (var grilla in new[] { dgvViajes, dgvEntregas, dgvCarga })
                grilla.DataError += (s, e) => TDPresentacion.ErrorDato(this, e, ObtenerTexto);
            Load += (s, e) => Ejecutar(Cargar);
        }
        private void Ejecutar(Action accion) { TDPresentacion.Ejecutar(this, accion, ObtenerTexto); }
        public override void ActualizarIdioma(BE.Idioma idioma)
        {
            base.ActualizarIdioma(idioma);
            TDPresentacion.Formatos(this, idioma);
        }
        private void Cargar()
        {
            cmbRequerimiento.DataSource = gestor.ObtenerPendientes();
            CargarMercaderia();
            MostrarOrigen();
        }
        private void MostrarOrigen()
        {
            txtOrigenSeleccionado.Text = (cmbRequerimiento.SelectedItem as RequerimientoDistribucion)?.Origen ?? "";
        }
        private void CargarMercaderia()
        {
            var catalogo = gestor.ObtenerMercaderia();
            dgvCargaIdMercaderia.DisplayMember = "Etiqueta";
            dgvCargaIdMercaderia.ValueMember = "IdMercaderia";
            dgvCargaIdMercaderia.DataSource = catalogo;
            cmbMercaderia.DataSource = catalogo;
        }
        private void MostrarViaje()
        {
            var viaje = dgvViajes.CurrentRow?.DataBoundItem as Viaje;
            dgvEntregas.DataSource = viaje == null ? null : new BindingList<Entrega>(viaje.Entregas);
            dgvCarga.DataSource = viaje == null ? null : new BindingList<DetalleCargaPlanificada>(viaje.CargaPlanificada);
            ActualizarIdioma(Servicios.IdiomaManager.GetInstance().GetIdiomaActual());
        }
        private void btnRecibir_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            int id = gestor.RecibirRequerimiento(txtRequerimiento.Text, txtOrigen.Text);
            Cargar(); cmbRequerimiento.SelectedValue = id;
        }); }
        private void btnProducto_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            int id = gestor.CrearMercaderia(txtCodigo.Text, txtDescripcion.Text);
            CargarMercaderia(); cmbMercaderia.SelectedValue = id;
        }); }
        private void btnViaje_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            viajes.Add(new Viaje { Numero = txtViaje.Text, FechaPrevista = dtpViaje.Value });
            dgvViajes.ClearSelection();
            dgvViajes.CurrentCell = dgvViajes.Rows[viajes.Count - 1].Cells[0];
            MostrarViaje();
        }); }
        private void btnQuitarViaje_Click(object sender, EventArgs e) { Ejecutar(() => viajes.Remove(TDPresentacion.Seleccion<Viaje>(dgvViajes))); }
        private void btnEntrega_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            TDPresentacion.Seleccion<Viaje>(dgvViajes).Entregas.Add(new Entrega { Destino = txtDestino.Text, FechaPrevista = dtpEntrega.Value });
            MostrarViaje();
        }); }
        private void btnQuitarEntrega_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            TDPresentacion.Seleccion<Viaje>(dgvViajes).Entregas.Remove(TDPresentacion.Seleccion<Entrega>(dgvEntregas));
            MostrarViaje();
        }); }
        private void btnCarga_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            var producto = cmbMercaderia.SelectedItem as Mercaderia;
            if (producto == null) throw new ReglaTDException("TD.Seleccionar");
            TDPresentacion.Seleccion<Viaje>(dgvViajes).CargaPlanificada.Add(new DetalleCargaPlanificada
                { IdMercaderia = producto.IdMercaderia, CantidadPrevista = nudCantidad.Value });
            MostrarViaje();
        }); }
        private void btnQuitarCarga_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            TDPresentacion.Seleccion<Viaje>(dgvViajes).CargaPlanificada.Remove(TDPresentacion.Seleccion<DetalleCargaPlanificada>(dgvCarga));
            MostrarViaje();
        }); }
        private void btnConfirmar_Click(object sender, EventArgs e) { Ejecutar(() =>
        {
            if (!dgvViajes.EndEdit() || !dgvEntregas.EndEdit() || !dgvCarga.EndEdit()) return;
            var req = cmbRequerimiento.SelectedItem as RequerimientoDistribucion;
            var plan = new PlanDistribucion { IdRequerimiento = req?.IdRequerimiento ?? 0, Numero = txtPlan.Text, Viajes = viajes.ToList() };
            gestor.Validar(plan);
            if (!TDPresentacion.Confirmar(this, ObtenerTexto)) return;
            gestor.Confirmar(plan);
            viajes.Clear(); txtPlan.Clear();
            TDPresentacion.Guardado(this, ObtenerTexto);
            Cargar();
        }); }
        private void btnActualizar_Click(object sender, EventArgs e) { Ejecutar(Cargar); }
    }
}
