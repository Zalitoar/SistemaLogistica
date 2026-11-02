using System;
using BE;
using BLL;
namespace IngSoft
{
    public partial class FrmCumplimientoViaje : FormularioTraducible
    {
        private readonly GestorCumplimiento gestor=new GestorCumplimiento();
        private CumplimientoViaje resumen;
        private bool cargando;
        private bool actualizacionPendiente;
        public FrmCumplimientoViaje()
        {
            InitializeComponent();
            TDPresentacion.TraducirEstados(dgvViajes,ObtenerTexto);
            TDPresentacion.TraducirEstados(dgvEntregas,ObtenerTexto);
            dgvViajes.CurrentCellChanged += (s,e) => ProgramarActualizacion();
            Load += (s,e) => Ejecutar(Cargar);
        }
        private void Ejecutar(Action accion) { TDPresentacion.Ejecutar(this,accion,ObtenerTexto); }
        private void Cargar()
        {
            cargando=true;
            try { dgvViajes.DataSource=gestor.ObtenerViajes(); }
            finally { cargando=false; }
            ProgramarActualizacion();
        }
        private void ProgramarActualizacion()
        {
            if(cargando || actualizacionPendiente || IsDisposed || Disposing || !IsHandleCreated) return;
            actualizacionPendiente=true;
            // Cambiar Enabled puede mover el foco a la grilla. No hacerlo dentro
            // de CurrentCellChanged, mientras SetCurrentCellAddressCore está activo.
            BeginInvoke(new Action(() =>
            {
                actualizacionPendiente=false;
                if(IsDisposed || Disposing || !IsHandleCreated) return;
                // Leer la selección vigente, no una fila capturada antes de la recarga.
                Ejecutar(Mostrar);
            }));
        }
        private void Mostrar()
        {
            var viaje=dgvViajes.CurrentRow?.DataBoundItem as Viaje;
            resumen=viaje==null ? null : gestor.Analizar(viaje.IdViaje);
            dgvEntregas.DataSource=resumen?.Entregas;
            btnCerrar.Enabled=resumen?.PuedeCerrar==true;
            ActualizarIdioma(Servicios.IdiomaManager.GetInstance().GetIdiomaActual());
        }
        public override void ActualizarIdioma(BE.Idioma idioma)
        {
            base.ActualizarIdioma(idioma);
            TDPresentacion.Formatos(this,idioma);
            if(lblResumen==null) return;
            lblResumen.Text=resumen==null ? "" : string.Format(TDPresentacion.Cultura(idioma),ObtenerTexto("FrmCumplimientoViaje.Resumen",
                "Cumplimiento: {0:N2}% | Total: {1} | Entregadas: {2} | Parciales: {3} | Incumplidas: {4} | Pendientes: {5} | Comprobantes: {6}"),
                resumen.Porcentaje,resumen.Total,resumen.Entregadas,resumen.Parciales,resumen.Incumplidas,resumen.Pendientes,resumen.Comprobantes);
            lblAviso.Text=resumen==null || resumen.PuedeCerrar || resumen.Viaje.Estado=="Cerrado" ? "" :
                ObtenerTexto("TD.CierreNoHabilitado","El viaje no puede cerrarse.");
            dgvViajes.Refresh(); dgvEntregas.Refresh();
        }
        private void btnActualizar_Click(object s,EventArgs e) { Ejecutar(Cargar); }
        private void btnCerrar_Click(object s,EventArgs e) { Ejecutar(() =>
        {
            var viaje=TDPresentacion.Seleccion<Viaje>(dgvViajes);
            if(!TDPresentacion.Confirmar(this,ObtenerTexto)) return;
            gestor.Cerrar(viaje.IdViaje);
            TDPresentacion.Guardado(this,ObtenerTexto); Cargar();
        }); }
    }
}
