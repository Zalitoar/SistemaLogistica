using System;
using System.ComponentModel;
using System.Linq;
using BE;
using BLL;
using System.Windows.Forms;
namespace IngSoft
{
    public partial class FrmPrepararDespacho : FormularioTraducible
    {
        private readonly GestorDespacho gestor=new GestorDespacho();
        private BindingList<CargaDespacho> carga=new BindingList<CargaDespacho>();
        private string estadoDespacho;
        public FrmPrepararDespacho()
        {
            InitializeComponent();
            TDPresentacion.TraducirEstados(dgvViajes,ObtenerTexto);
            dgvViajes.CurrentCellChanged += (s,e) => Ejecutar(Mostrar);
            dgvCarga.DataError += (s,e) => TDPresentacion.ErrorDato(this,e,ObtenerTexto);
            dgvCarga.CellEndEdit += (s,e) => dgvCarga.Refresh();
            Load += (s,e) => Ejecutar(Cargar);
        }
        private void Ejecutar(Action accion) { TDPresentacion.Ejecutar(this,accion,ObtenerTexto); }
        private void Cargar() { dgvViajes.DataSource=gestor.ObtenerViajes(); Mostrar(); }
        private void Mostrar()
        {
            var viaje=dgvViajes.CurrentRow?.DataBoundItem as Viaje;
            var doc=viaje==null ? null : gestor.ObtenerDespacho(viaje.IdViaje);
            carga=new BindingList<CargaDespacho>(viaje==null ? new System.Collections.Generic.List<CargaDespacho>() : gestor.ObtenerCarga(viaje.IdViaje));
            dgvCarga.DataSource=carga;
            txtNumero.Text=doc?.Numero ?? "";
            txtNumero.ReadOnly=doc!=null;
            txtObservaciones.Text=doc?.Observaciones ?? "";
            estadoDespacho=doc?.Estado;
            ActualizarIdioma(Servicios.IdiomaManager.GetInstance().GetIdiomaActual());
        }
        public override void ActualizarIdioma(BE.Idioma idioma)
        {
            base.ActualizarIdioma(idioma);
            TDPresentacion.Formatos(this,idioma);
            if (lblEstado != null) lblEstado.Text=estadoDespacho==null ? "" : ObtenerTexto("TD.Estado."+estadoDespacho,estadoDespacho);
            dgvViajes?.Refresh();
        }
        private void btnActualizar_Click(object s,EventArgs e) { Ejecutar(Cargar); }
        private void btnGuardar_Click(object s,EventArgs e) { Ejecutar(() =>
        {
            if (!dgvCarga.EndEdit()) return;
            if(!TDPresentacion.Confirmar(this,ObtenerTexto)) return;
            var doc=gestor.Guardar(TDPresentacion.Seleccion<Viaje>(dgvViajes).IdViaje,txtNumero.Text,txtObservaciones.Text,carga.ToList());
            MessageBox.Show(this,ObtenerTexto(doc.Estado=="Pendiente" ? "TD.DespachoPendiente" : "TD.Guardado",doc.Estado));
            Cargar();
        }); }
    }
}
