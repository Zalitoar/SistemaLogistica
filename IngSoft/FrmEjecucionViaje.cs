using System;
using BE;
using BLL;
namespace IngSoft
{
    public partial class FrmEjecucionViaje : FormularioTraducible
    {
        private readonly GestorViaje gestor=new GestorViaje();
        private DocumentoDespacho despacho;
        public FrmEjecucionViaje()
        {
            InitializeComponent();
            TDPresentacion.TraducirEstados(dgvViajes,ObtenerTexto);
            TDPresentacion.TraducirEstados(dgvEntregas,ObtenerTexto);
            dgvViajes.CurrentCellChanged += (s,e) => Ejecutar(Mostrar);
            Load += (s,e) => Ejecutar(Cargar);
        }
        private void Ejecutar(Action accion) { TDPresentacion.Ejecutar(this,accion,ObtenerTexto); }
        private void Cargar() { dgvViajes.DataSource=gestor.ObtenerViajes(); Mostrar(); }
        private void Mostrar()
        {
            var viaje=dgvViajes.CurrentRow?.DataBoundItem as Viaje;
            despacho=viaje==null ? null : gestor.ObtenerDespacho(viaje.IdViaje);
            dgvCarga.DataSource=viaje==null ? null : gestor.ObtenerCarga(viaje.IdViaje);
            dgvEntregas.DataSource=viaje==null ? null : gestor.ObtenerEntregas(viaje.IdViaje);
            ActualizarIdioma(Servicios.IdiomaManager.GetInstance().GetIdiomaActual());
        }
        public override void ActualizarIdioma(BE.Idioma idioma)
        {
            base.ActualizarIdioma(idioma);
            TDPresentacion.Formatos(this,idioma);
            if(lblDocumento!=null) lblDocumento.Text=despacho==null ? ObtenerTexto("TD.SinDespacho","Sin despacho") :
                despacho.Numero+" | "+despacho.Fecha.ToString("g",TDPresentacion.Cultura(idioma))+" | "+ObtenerTexto("TD.Estado."+despacho.Estado,despacho.Estado);
            dgvViajes?.Refresh(); dgvEntregas?.Refresh();
        }
        private void btnActualizar_Click(object s,EventArgs e) { Ejecutar(Cargar); }
        private void btnIniciar_Click(object s,EventArgs e) { Ejecutar(() =>
        {
            var viaje=TDPresentacion.Seleccion<Viaje>(dgvViajes);
            if(!TDPresentacion.Confirmar(this,ObtenerTexto)) return;
            gestor.Iniciar(viaje.IdViaje);
            TDPresentacion.Guardado(this,ObtenerTexto); Cargar();
        }); }
    }
}
