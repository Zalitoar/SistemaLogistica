using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
namespace IngSoft
{
    public partial class FrmRegistroEntrega : FormularioTraducible
    {
        private readonly GestorEntrega gestor=new GestorEntrega();
        public FrmRegistroEntrega()
        {
            InitializeComponent();
            TDPresentacion.TraducirEstados(dgvViajes,ObtenerTexto);
            TDPresentacion.TraducirEstados(dgvEntregas,ObtenerTexto);
            dgvViajes.CurrentCellChanged += (s,e) => Ejecutar(Mostrar);
            dgvEntregas.CurrentCellChanged += (s,e) => MostrarEntrega();
            Load += (s,e) => Ejecutar(Cargar);
        }
        private void Ejecutar(Action accion) { TDPresentacion.Ejecutar(this,accion,ObtenerTexto); }
        private void Cargar() { dgvViajes.DataSource=gestor.ObtenerViajes(); Mostrar(); }
        private void Mostrar()
        {
            var viaje=dgvViajes.CurrentRow?.DataBoundItem as Viaje;
            dgvEntregas.DataSource=viaje==null ? null : gestor.ObtenerEntregas(viaje.IdViaje);
            MostrarEntrega();
        }
        private void MostrarEntrega()
        {
            var entrega=dgvEntregas.CurrentRow?.DataBoundItem as Entrega;
            txtObservaciones.Text=entrega?.Observaciones ?? "";
            dtpRealizada.Value=entrega?.FechaRealizada ?? DateTime.Now;
            bool habilitada=GestorEntrega.PuedeRegistrar(dgvViajes.CurrentRow?.DataBoundItem as Viaje,entrega);
            btnRegistrar.Enabled=habilitada;
            cmbResultado.Enabled=habilitada;
            dtpRealizada.Enabled=habilitada;
            txtObservaciones.ReadOnly=!habilitada;
            cmbResultado.SelectedValue=habilitada ? "Entregada" : entrega?.Estado ?? "Entregada";
        }
        public override void ActualizarIdioma(BE.Idioma idioma)
        {
            base.ActualizarIdioma(idioma);
            TDPresentacion.Formatos(this,idioma);
            if(cmbResultado==null) return;
            string seleccionado=cmbResultado.SelectedValue as string;
            if(seleccionado==null)
            {
                var actual=dgvEntregas.CurrentRow?.DataBoundItem as Entrega;
                seleccionado=new[]{"Entregada","Parcial","Incumplida"}.Contains(actual?.Estado) ? actual.Estado : "Entregada";
            }
            cmbResultado.DataSource=new[]{"Entregada","Parcial","Incumplida"}
                .Select(v => new KeyValuePair<string,string>(v,ObtenerTexto("TD.Estado."+v,v))).ToList();
            if(seleccionado!=null) cmbResultado.SelectedValue=seleccionado;
            dgvViajes.Refresh(); dgvEntregas.Refresh();
        }
        private void btnActualizar_Click(object s,EventArgs e) { Ejecutar(Cargar); }
        private void btnRegistrar_Click(object s,EventArgs e) { Ejecutar(() =>
        {
            var viaje=TDPresentacion.Seleccion<Viaje>(dgvViajes);
            var entrega=TDPresentacion.Seleccion<Entrega>(dgvEntregas);
            string resultado=cmbResultado.SelectedValue as string;
            GestorEntrega.ValidarResultado(viaje,entrega,resultado,dtpRealizada.Value,txtObservaciones.Text);
            if(!TDPresentacion.Confirmar(this,ObtenerTexto)) return;
            gestor.Registrar(viaje.IdViaje,entrega.IdEntrega,resultado,dtpRealizada.Value,txtObservaciones.Text);
            TDPresentacion.Guardado(this,ObtenerTexto); Mostrar();
        }); }
    }
}
