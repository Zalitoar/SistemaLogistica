using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BE;
using BLL;
using Servicios;

internal static class ReentranciaCumplimientoTD
{
    private static readonly List<Exception> errores = new List<Exception>();
    private static int checks;
    private static void Check(bool condicion, string texto)
    {
        if (!condicion) throw new Exception(texto);
        if (errores.Count > 0) throw new AggregateException("Error durante cambio de foco", errores);
        checks++;
        Console.WriteLine("OK: " + texto);
    }
    private static Viaje CrearViaje(string numero, int producto, int horas)
    {
        return new Viaje {
            Numero = numero, FechaPrevista = DateTime.Now.AddHours(horas),
            CargaPlanificada = new List<DetalleCargaPlanificada> {
                new DetalleCargaPlanificada { IdMercaderia = producto, CantidadPrevista = 1 }
            },
            Entregas = new List<Entrega> {
                new Entrega { Destino = numero, FechaPrevista = DateTime.Now.AddHours(horas + 1) }
            }
        };
    }
    private static DataGridViewRow Fila(DataGridView grid, int id)
    {
        return grid.Rows.Cast<DataGridViewRow>().Single(r => ((Viaje)r.DataBoundItem).IdViaje == id);
    }
    [STAThread]
    private static int Main()
    {
        try
        {
            var cadena = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SQL"].ConnectionString);
            if (!cadena.InitialCatalog.StartsWith("SistemaLogistica_TD_Test_", StringComparison.Ordinal))
                throw new Exception("Se requiere una base aislada de pruebas.");
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => errores.Add(e.Exception);
            Application.EnableVisualStyles();
            SessionManager.Login(new BE.Usuario { Nombre = "TD_FOCO_TEST" });
            SessionManager.SetPermisos(new List<string> { GestorPlanificacion.Permiso, GestorDespacho.Permiso,
                GestorViaje.Permiso, GestorEntrega.Permiso, GestorCumplimiento.Permiso });
            var idiomas = IdiomaManager.GetInstance();
            idiomas.CargarIdiomas(new DAL.MP_Idioma().Listar());
            idiomas.CargarTraducciones(new DAL.MP_Traduccion().Listar());
            idiomas.SetIdiomaActual(idiomas.ListarIdiomas().First());
            var planificador = new GestorPlanificacion();
            var numero = "FOCO-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            int producto = planificador.CrearMercaderia(numero, "Prueba de foco");
            int req = planificador.RecibirRequerimiento(numero, "Origen");
            int plan = planificador.Confirmar(new PlanDistribucion {
                IdRequerimiento = req, Numero = numero,
                Viajes = new List<Viaje> { CrearViaje(numero + "-LISTO", producto, 2), CrearViaje(numero + "-PENDIENTE", producto, 1) }
            });
            var viajes = new GestorCumplimiento().ObtenerViajes().Where(v => v.IdPlan == plan).ToList();
            int listo = viajes.Single(v => v.Numero.EndsWith("-LISTO")).IdViaje;
            int pendiente = viajes.Single(v => v.Numero.EndsWith("-PENDIENTE")).IdViaje;
            new GestorDespacho().Guardar(listo, numero, "", new List<CargaDespacho> {
                new CargaDespacho { IdMercaderia = producto, CantidadPreparada = 1 }
            });
            new GestorViaje().Iniciar(listo);
            var entrega = new GestorEntrega().ObtenerEntregas(listo).Single();
            new GestorEntrega().Registrar(listo, entrega.IdEntrega, "Entregada", DateTime.Now, "");

            using (var mdi = new Form { IsMdiContainer = true, ShowInTaskbar = false, Opacity = 0,
                StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), Size = new Size(1300, 1000) })
            using (var form = new IngSoft.FrmCumplimientoViaje())
            {
                mdi.Show();
                form.MdiParent = mdi;
                form.Show();
                Application.DoEvents();
                var grid = (DataGridView)form.Controls.Find("dgvViajes", true).Single();
                var detalle = (DataGridView)form.Controls.Find("dgvEntregas", true).Single();
                var cerrar = (Button)form.Controls.Find("btnCerrar", true).Single();
                var recargar = typeof(IngSoft.FrmCumplimientoViaje).GetMethod("Cargar", BindingFlags.Instance | BindingFlags.NonPublic);
                grid.CurrentCell = Fila(grid, listo).Cells[0];
                Application.DoEvents();
                Check(cerrar.Enabled, "viaje completo habilita cierre");
                cerrar.Focus();
                Check(cerrar.ContainsFocus, "botón Cerrar tiene el foco antes de cambiar la celda");

                // DataSource y navegación pueden quitar la celda actual mientras Cerrar tiene foco.
                // Deshabilitar el botón dentro de CurrentCellChanged reentra en DataGridView.OnEnter.
                grid.CurrentCell = null;
                Application.DoEvents();
                // Al recibir foco Windows puede seleccionar automáticamente la primera fila.
                // El botón debe reflejar la selección resultante, sin entrar de nuevo en OnEnter.
                var seleccionado = grid.CurrentRow?.DataBoundItem as Viaje;
                Check(cerrar.Enabled == (seleccionado != null && new GestorCumplimiento().Analizar(seleccionado.IdViaje).PuedeCerrar),
                    "quitar selección con foco en Cerrar no causa reentrancia");
                grid.CurrentCell = Fila(grid, listo).Cells[0];
                Application.DoEvents();
                cerrar.Focus();
                recargar.Invoke(form, null);
                Application.DoEvents();
                Check(cerrar.Enabled, "recargar con foco en Cerrar conserva estado válido");
                Check(detalle.Rows.Count == 1, "recarga muestra entregas del viaje seleccionado");

                for (int i = 0; i < 10; i++)
                {
                    grid.CurrentCell = Fila(grid, listo).Cells[0];
                    grid.CurrentCell = Fila(grid, pendiente).Cells[0];
                }
                Application.DoEvents();
                Check(!cerrar.Enabled, "selección rápida respeta el último viaje");
                Check(((Entrega)detalle.Rows[0].DataBoundItem).IdViaje == pendiente, "detalle corresponde al último viaje");
                foreach (var idioma in idiomas.ListarIdiomas())
                {
                    idiomas.SetIdiomaActual(idioma);
                    Application.DoEvents();
                    Check(form.Text == idiomas.Traducir(form.Name + ".Title"), "idioma " + idioma.Codigo);
                }

                grid.CurrentCell = Fila(grid, listo).Cells[0];
                Application.DoEvents();
                cerrar.Focus();
                new GestorCumplimiento().Cerrar(listo);
                recargar.Invoke(form, null);
                Application.DoEvents();
                Check(!cerrar.Enabled, "cierre y recarga deshabilitan el botón sin error de foco");
                Check(new GestorCumplimiento().Analizar(listo).Viaje.Estado == "Cerrado", "cierre persistido");
                grid.CurrentCell = null;
                form.Dispose();
                Application.DoEvents();
                Check(errores.Count == 0, "cerrar formulario con actualización pendiente es seguro");
            }
            Console.WriteLine("TOTAL: " + checks + " comprobaciones de reentrancia correctas.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
