using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;
using BLL;
using DAL;
using Servicios;
using IngSoft;

internal static class FlujoMF
{
    private static int checks;
    private static string cadena;
    private static readonly string[] permisos =
    {
        GestorFlota.Permiso,
        GestorMantenimiento.PermisoProgramar,
        GestorMantenimiento.PermisoIntervenir,
        GestorDisponibilidad.PermisoHabilitar,
        GestorDisponibilidad.PermisoAsignar,
        GestorPlanificacion.Permiso,
        GestorDespacho.Permiso,
        GestorViaje.Permiso,
        GestorEntrega.Permiso,
        GestorCumplimiento.Permiso
    };
    private static void Check(bool ok, string nombre)
    {
        if (!ok)
            throw new Exception(nombre);
        checks++;
    }

    private static void Rechaza(Action accion, string clave)
    {
        try
        {
            accion();
        }
        catch (ReglaMFException ex)
        {
            Check(ex.Message == clave, "Esperaba " + clave + ", obtuvo " + ex.Message);
            return;
        }

        throw new Exception("Debía rechazar " + clave);
    }

    private static int Count(string sql)
    {
        using (var c = new SqlConnection(cadena))
        using (var cmd = new SqlCommand(sql, c))
        {
            c.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }

    private static IEnumerable<Control> Controles(Control p)
    {
        foreach (Control c in p.Controls)
        {
            yield return c;
            foreach (var h in Controles(c))
                yield return h;
        }
    }

    private sealed class Host : FrmApp
    {
        protected override void OnLoad(EventArgs e)
        {
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
        }
    }

    private static void Interfaz(string salida)
    {
        var manager = IdiomaManager.GetInstance();
        manager.CargarIdiomas(new MP_Idioma().Listar());
        manager.CargarTraducciones(new MP_Traduccion().Listar());
        var tipos = new[]
        {
            typeof(FrmEstadoFlota),
            typeof(FrmProgramarMantenimiento),
            typeof(FrmIntervencionMantenimiento),
            typeof(FrmHabilitarUnidad),
            typeof(FrmDisponibilidadFlota)
        };
        Directory.CreateDirectory(salida);
        foreach (var tipo in tipos)
            using (var form = (FormularioTraducible)Activator.CreateInstance(tipo))
            {
                form.ShowInTaskbar = false;
                form.Opacity = 0;
                form.Show();
                Application.DoEvents();
                foreach (var idioma in manager.ListarIdiomas())
                {
                    manager.SetIdiomaActual(idioma);
                    form.ActualizarIdioma(idioma);
                    form.ClientSize = new Size(1040, 700);
                    Application.DoEvents();
                    Check(form.Text == manager.Traducir(form.Name + ".Title"), "Título traducido");
                    foreach (var c in Controles(form))
                    {
                        if (c is Button || c is GroupBox || c is Label)
                            Check(c.Text == manager.Traducir(form.Name + "." + c.Name + ".Text"), "Texto " + form.Name + " " + c.Name);
                        if (c is ComboBox)
                        {
                            var combo = (ComboBox)c;
                            Check(combo.Text == manager.Traducir("MF.Valor." + combo.SelectedValue), "Opción traducida " + combo.Name);
                        }

                        if (c is FlowLayoutPanel)
                            Check(c.Bottom <= c.Parent.ClientSize.Height, "Panel recortado " + form.Name + " " + c.Name);
                        var grid = c as DataGridView;
                        if (grid != null)
                        {
                            Check(grid.Height >= 55, "Grilla legible " + form.Name);
                            foreach (DataGridViewColumn col in grid.Columns)
                                Check(col.HeaderText == manager.Traducir(form.Name + "." + grid.Name + "." + col.DataPropertyName + ".HeaderText"), "Columna traducida");
                        }
                    }

                    using (var bmp = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(bmp, new Rectangle(Point.Empty, form.Size));
                        bmp.Save(Path.Combine(salida, form.Name + "-" + idioma.Codigo + ".png"));
                    }
                }

                Console.WriteLine("UI OK: " + tipo.Name);
            }

        using (var app = new Host())
        {
            app.ShowInTaskbar = false;
            app.Opacity = 0;
            app.Show();
            SessionManager.SetPermisos(new List<string> { GestorFlota.Permiso });
            typeof(FrmApp).GetMethod("ValidarPermisosMF", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(app, null);
            var menu = app.MainMenuStrip.Items.OfType<ToolStripMenuItem>().Single(m => m.Name == "menuMF");
            Check(menu.DropDownItems.Cast<ToolStripItem>().Count(m => m.Enabled) == 1, "Menú por permisos");
            menu.DropDownItems[0].PerformClick();
            Application.DoEvents();
            var primero = app.ActiveMdiChild;
            menu.DropDownItems[0].PerformClick();
            Check(ReferenceEquals(primero, app.ActiveMdiChild) && app.MdiChildren.Length == 1, "Una instancia MDI");
            app.ActiveMdiChild.Close();
        }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            if (args.Length > 1)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(args[1], @"^SistemaLogistica_MF_Test_[0-9]+$"))
                    return 2;
                cadena = new MP_ConfiguracionBD().Cadena(new ConfiguracionBaseDatos { Servidor = @".\SQLEXPRESS", BaseDatos = args[1], ConfiarCertificado = true });
                typeof(ConexionLocal).GetField("activa", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, cadena);
                SessionManager.Login(new BE.Usuario { Nombre = "MF_UI_TEST" });
                SessionManager.SetPermisos(permisos.ToList());
                Interfaz(args[0]);
                Console.WriteLine("UI MF: " + checks + " comprobaciones aprobadas.");
                return 0;
            }

            string nombre = "SistemaLogistica_MF_Test_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            var config = new ConfiguracionBaseDatos
            {
                Servidor = @".\SQLEXPRESS",
                BaseDatos = nombre,
                ConfiarCertificado = true
            };
            var bd = new MP_ConfiguracionBD();
            bd.CrearBase(config);
            bd.Inicializar(config, CryptoManager.Hash("Prueba-MF-2026"));
            cadena = bd.Cadena(config);
            typeof(ConexionLocal).GetField("activa", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, cadena);
            Check(bd.Verificar(cadena).Compatible, "Esquema MF compatible con asistente");
            bd.InicializarDatosPrueba(config, CryptoManager.Hash("Prueba-MF-2026"));
            Console.WriteLine("Base aislada: " + nombre);
            var flota = new GestorFlota();
            var mant = new GestorMantenimiento();
            var disp = new GestorDisponibilidad();
            var dal = new MP_MF();
            Rechaza(() => flota.Unidades(), "MF.SinPermiso");
            SessionManager.Login(new BE.Usuario { Nombre = "MF_TEST" });
            SessionManager.SetPermisos(new List<string>());
            Rechaza(() => flota.Crear(null), "MF.SinPermiso");
            Rechaza(() => mant.Programar(null), "MF.SinPermiso");
            Rechaza(() => mant.Iniciar(0), "MF.SinPermiso");
            Rechaza(() => disp.Habilitar(0, ""), "MF.SinPermiso");
            Rechaza(() => disp.Asignar(0, 0), "MF.SinPermiso");
            SessionManager.SetPermisos(permisos.ToList());
            Func<string, int> crear = d => flota.Crear(new UnidadFlota { Dominio = d, Tipo = "Camión", Marca = "Prueba", Modelo = "MF", Anio = 2024, Capacidad = 15000, KilometrajeActual = 100 });
            int unidad = crear("MF000AA");
            Check(flota.Unidades().Single().EstadoOperativo == "FueraServicio", "Alta no habilita sin control");
            Rechaza(() => crear("MF000AA"), "MF.Duplicado");
            Rechaza(() => flota.Registrar(new ParteEstadoUnidad { IdUnidadFlota = unidad, Kilometraje = 99, TipoNovedad = "Control", Descripcion = "Control" }), "MF.Kilometraje");
            Func<int, bool, int> parte = (u, m) => flota.Registrar(new ParteEstadoUnidad { IdUnidadFlota = u, Kilometraje = 110, TipoNovedad = "Control", Descripcion = "Revisión de frenos", RequiereMantenimiento = m });
            int p = parte(unidad, true);
            Check(!disp.Disponibles().Any(), "Parte con falla bloquea disponibilidad");
            Rechaza(() => parte(unidad, false), "MF.Pendientes");
            Func<int, int> programar = id => mant.Programar(new OrdenMantenimiento { IdParteEstadoUnidad = id, FechaProgramada = DateTime.Today.AddDays(1), TipoMantenimiento = "Correctivo", Prioridad = "Alta", DescripcionTrabajo = "Revisar frenos" });
            int orden = programar(p);
            Rechaza(() => programar(p), "MF.EstadoInvalido");
            Rechaza(() => disp.Habilitar(unidad, ""), "MF.Pendientes");
            int interv = mant.Iniciar(orden);
            Rechaza(() => mant.Iniciar(orden), "MF.EstadoInvalido");
            var i = new IntervencionMantenimiento
            {
                IdIntervencionMantenimiento = interv,
                Kilometraje = 110,
                TrabajoRealizado = "Ajuste de frenos",
                Resultado = "RequiereTareas",
                Observaciones = ""
            };
            Rechaza(() => mant.Finalizar(i), "MF.DatosInvalidos");
            i.Observaciones = "Cambiar componente";
            mant.Finalizar(i);
            Rechaza(() => disp.Habilitar(unidad, ""), "MF.Pendientes");
            i.IdIntervencionMantenimiento = mant.Iniciar(orden);
            i.Resultado = "Satisfactoria";
            i.TrabajoRealizado = "Reemplazo y prueba";
            mant.Finalizar(i);
            Check(!disp.Disponibles().Any(), "Resultado satisfactorio requiere habilitación");
            Rechaza(() => mant.Finalizar(i), "MF.EstadoInvalido");
            int informe = disp.Habilitar(unidad, "Prueba final conforme");
            Check(disp.Disponibles().Single().IdUnidadFlota == unidad, "Habilitada");
            Check(dal.Ordenes().Single().EstadoOrden == "Cerrada" && dal.Partes().Single().EstadoParte == "Resuelto", "Cierre de orden y parte");
            Check(dal.Intervenciones(orden).Count == 2, "Historial de tareas adicionales");
            Rechaza(() => disp.Habilitar(unidad, ""), "MF.EstadoInvalido");
            var viajes = new MP_Viaje().Listar();
            int v = viajes.Single(x => x.Estado == "Preparado").IdViaje;
            disp.Asignar(unidad, v);
            Check(new MP_Viaje().Listar().Single(x => x.IdViaje == v).IdInformeDisponibilidad == informe, "Asignación respaldada por informe");
            Rechaza(() => disp.Asignar(unidad, viajes.First(x => x.Estado == "Planificado").IdViaje), "MF.Asignada");
            Check(!disp.Disponibles().Any() && disp.Disponibles(v).Any(), "Reserva exclusiva y visualización para viaje actual");
            int nueva = parte(unidad, true);
            bool rechazoTD = false;
            try
            {
                new GestorViaje().Iniciar(v);
            }
            catch (ReglaTDException)
            {
                rechazoTD = true;
            }

            Check(rechazoTD, "TD revalida disponibilidad asignada");
            int nuevaOrden = programar(nueva);
            i.IdIntervencionMantenimiento = mant.Iniciar(nuevaOrden);
            mant.Finalizar(i);
            disp.Habilitar(unidad, "Conforme");
            disp.Asignar(unidad, v);
            new GestorViaje().Iniciar(v);
            Rechaza(() => parte(unidad, true), "MF.EnViaje");
            foreach (var entrega in new MP_Entrega().Obtener(v))
                new GestorEntrega().Registrar(v, entrega.IdEntrega, "Entregada", DateTime.Now, "Completa");
            new GestorCumplimiento().Cerrar(v);
            Check(disp.Disponibles().Any(x => x.IdUnidadFlota == unidad), "Cierre TD libera reserva");
            int sinMantenimiento = crear("MF001AA");
            parte(sinMantenimiento, false);
            Check(dal.Informes(sinMantenimiento).Single().IdIntervencionMantenimiento == null && disp.Disponibles().Any(x => x.IdUnidadFlota == sinMantenimiento), "Camino sin mantenimiento genera informe");
            int[] destinos = viajes.Where(x => x.Estado == "Planificado").Select(x => x.IdViaje).Take(2).ToArray();
            int exitos = 0;
            Parallel.ForEach(destinos, d =>
            {
                try
                {
                    new MP_MF().Asignar(sinMantenimiento, d);
                    System.Threading.Interlocked.Increment(ref exitos);
                }
                catch (ReglaMFException ex)
                {
                    if (ex.Message != "MF.Asignada")
                        throw;
                }
            });
            Check(exitos == 1, "Asignaciones simultáneas no duplican reserva");
            Check(Count("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora LIKE 'MF:%'") >= 15, "Auditoría MF");
            Check(Count("SELECT COUNT(*) FROM dbo.PERMISO WHERE Nombre_Permiso LIKE 'MF[_]%'") == 5, "Cinco permisos");
            Interfaz(args[0]);
            Console.WriteLine("MF: " + checks + " comprobaciones aprobadas.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}
