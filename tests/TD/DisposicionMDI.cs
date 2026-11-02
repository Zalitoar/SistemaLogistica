using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using IngSoft;
using Servicios;

internal static class DisposicionMDI
{
    private sealed class HostMDI : FrmApp
    {
        protected override void OnLoad(EventArgs e) { }
        protected override void OnFormClosing(FormClosingEventArgs e) { }
    }
    private static int checks;
    private static void Check(bool ok,string message)
    {
        if(!ok) throw new Exception(message);
        checks++;
    }
    private static IEnumerable<Control> Controles(Control padre)
    {
        foreach(Control c in padre.Controls) { yield return c; foreach(var hijo in Controles(c)) yield return hijo; }
    }
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            string db = args.Length > 1 ? args[1] : "SistemaLogistica_TD_Test_UI_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (!System.Text.RegularExpressions.Regex.IsMatch(db,@"^SistemaLogistica_TD_Test_UI_[0-9]+$")) return 2;
            var config = new BE.ConfiguracionBaseDatos { Servidor=@".\SQLEXPRESS", BaseDatos=db, ConfiarCertificado=true };
            var bd = new DAL.MP_ConfiguracionBD();
            if(args.Length < 2) {
                bd.CrearBase(config);
                bd.Inicializar(config, CryptoManager.Hash("Prueba-UI-2026"));
                bd.InicializarDatosPrueba(config, CryptoManager.Hash("Prueba-UI-2026"));
            }
            // Sólo este proceso; nunca leer/escribir la conexión del perfil del usuario.
            typeof(DAL.ConexionLocal).GetField("activa",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,bd.Cadena(config));
            Console.WriteLine("Base aislada: " + db);
            SessionManager.Login(new BE.Usuario { Nombre="TD_UI_TEST" });
            SessionManager.SetPermisos(new List<string> { BLL.GestorPlanificacion.Permiso,BLL.GestorDespacho.Permiso,BLL.GestorViaje.Permiso,BLL.GestorEntrega.Permiso,BLL.GestorCumplimiento.Permiso });
            var manager=IdiomaManager.GetInstance();
            manager.CargarIdiomas(new DAL.MP_Idioma().Listar());
            manager.CargarTraducciones(new DAL.MP_Traduccion().Listar());
            var tipos=new[]{typeof(FrmPlanificacionDistribucion),typeof(FrmPrepararDespacho),typeof(FrmEjecucionViaje),typeof(FrmRegistroEntrega),typeof(FrmCumplimientoViaje)};
            string salida=args[0];
            Directory.CreateDirectory(salida);
            foreach(var tipo in tipos)
            using(var form=(FormularioTraducible)Activator.CreateInstance(tipo))
            {
                form.ShowInTaskbar=false;
                form.Opacity=0;
                form.StartPosition=FormStartPosition.Manual;
                form.Location=new Point(-20000,-20000);
                form.Show();
                Application.DoEvents();
                if(form is FrmPlanificacionDistribucion)
                {
                    var controles=Controles(form).Where(c => !string.IsNullOrEmpty(c.Name)).ToDictionary(c => c.Name,c => c);
                    ((TextBox)controles["txtViaje"]).Text="VISTA-01";
                    ((DateTimePicker)controles["dtpViaje"]).Value=DateTime.Now.AddHours(1);
                    ((Button)controles["btnViaje"]).PerformClick();
                    ((TextBox)controles["txtDestino"]).Text="Depósito central";
                    ((DateTimePicker)controles["dtpEntrega"]).Value=DateTime.Now.AddHours(2);
                    ((Button)controles["btnEntrega"]).PerformClick();
                    ((NumericUpDown)controles["nudCantidad"]).Value=5.125m;
                    ((Button)controles["btnCarga"]).PerformClick();
                    ((TextBox)controles["txtViaje"]).Text="VISTA-02";
                    ((Button)controles["btnViaje"]).PerformClick();
                    Check(((DataGridView)controles["dgvCarga"]).Rows.Count==0,"carga separada por viaje");
                    Check(((DataGridView)controles["dgvEntregas"]).Rows.Count==0,"entregas separadas por viaje");
                    ((DataGridView)controles["dgvViajes"]).CurrentCell=((DataGridView)controles["dgvViajes"]).Rows[0].Cells[0];
                    Check(((DataGridView)controles["dgvCarga"]).Rows.Count==1,"carga conservada al cambiar viaje");
                    Check(((DataGridView)controles["dgvEntregas"]).Rows.Count==1,"entrega conservada al cambiar viaje");
                }
                foreach(var idioma in manager.ListarIdiomas().Where(i => new[]{"es-AR","en-US","pt-BR"}.Contains(i.Codigo)))
                {
                    manager.SetIdiomaActual(idioma);
                    form.ActualizarIdioma(idioma);
                    form.ClientSize = new Size(980, 620);
                    form.PerformLayout();
                    Application.DoEvents();
                    Check(form.Text==manager.Traducir(form.Name+".Title"),form.Name+" título "+idioma.Codigo);
                    foreach(var c in Controles(form))
                    {
                        if(c is Button || c is GroupBox || (c is Label && manager.Traducir(form.Name+"."+c.Name+".Text")!=form.Name+"."+c.Name+".Text"))
                            Check(c.Text==manager.Traducir(form.Name+"."+c.Name+".Text"),form.Name+"."+c.Name+" traducción "+idioma.Codigo);
                        var grid=c as DataGridView;
                        if(grid!=null) foreach(DataGridViewColumn col in grid.Columns)
                            Check(col.HeaderText==manager.Traducir(form.Name+"."+grid.Name+"."+col.DataPropertyName+".HeaderText"),form.Name+" columna "+col.DataPropertyName);
                        if(c is DataGridView) Check(c.Height >= 70, form.Name+" grilla demasiado pequeña: "+c.Name+" "+c.Height);
                        if(c is Button || c is TextBox || c is ComboBox || c is NumericUpDown || c is DateTimePicker)
                            Check(c.Right<=c.Parent.ClientSize.Width && c.Bottom<=c.Parent.ClientSize.Height,form.Name+" control recortado: "+c.Name);
                        if(c is FlowLayoutPanel)
                        {
                            c.PerformLayout();
                            Check(c.Bottom<=c.Parent.ClientSize.Height,form.Name+" panel fuera del grupo: "+c.Name);
                        }
                    }
                    using(var bmp=new Bitmap(form.Width,form.Height))
                    {
                        form.DrawToBitmap(bmp,new Rectangle(Point.Empty,form.Size));
                        bmp.Save(Path.Combine(salida,form.Name+"-"+idioma.Codigo+".png"));
                    }
                    // Reducir y volver a ampliar debe recalcular barras y conservar las grillas.
                    foreach(var size in new[] {new Size(900,580),new Size(1280,720)}) {
                        form.ClientSize=size; form.PerformLayout(); Application.DoEvents();
                        foreach(var c in Controles(form)) {
                            if(c is FlowLayoutPanel) Check(c.Bottom<=c.Parent.ClientSize.Height,"Panel al redimensionar "+form.Name+" "+c.Name);
                            if(c is DataGridView) Check(c.Height>=45,"Grilla al redimensionar "+form.Name+" "+c.Name);
                        }
                    }
                    Console.WriteLine("OK UI: "+form.Name+" "+idioma.Codigo);
                }
            }
            using(var app=new FrmApp())
            {
                SessionManager.SetPermisos(new List<string>{BLL.GestorEntrega.Permiso});
                typeof(FrmApp).GetMethod("ValidarPermisosTD",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(app,null);
                var menu=app.MainMenuStrip.Items.Cast<ToolStripItem>().OfType<ToolStripMenuItem>().Single(m => m.Name=="menuTD");
                Check(menu.DropDownItems.Count==5,"cinco opciones MDI");
                Check(menu.DropDownItems.Cast<ToolStripItem>().Count(m => m.Enabled)==1,"opciones habilitadas por permiso");
                Check(menu.DropDownItems.Cast<ToolStripItem>().Single(m => m.Enabled).Name=="menuTD04","permiso específico de entrega");
                using(var child=new FrmRegistroEntrega()) { child.MdiParent=app; Check(child.MdiParent==app,"formulario MDI hijo"); }
            }
            SessionManager.SetPermisos(new List<string> { BLL.GestorPlanificacion.Permiso,BLL.GestorDespacho.Permiso,BLL.GestorViaje.Permiso,BLL.GestorEntrega.Permiso,BLL.GestorCumplimiento.Permiso });
            using(var app = new HostMDI())
            {
                app.ShowInTaskbar=false; app.Opacity=0;
                app.WindowState=FormWindowState.Normal; app.ClientSize=new Size(1000,680);
                app.Show(); Application.DoEvents();
                typeof(FrmApp).GetMethod("ValidarPermisosTD",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(app,null);
                var menu=app.MainMenuStrip.Items.OfType<ToolStripMenuItem>().Single(m=>m.Name=="menuTD");
                foreach(ToolStripMenuItem opcion in menu.DropDownItems)
                {
                    opcion.PerformClick(); Application.DoEvents();
                    var original=app.ActiveMdiChild;
                    Check(original!=null,"Abre MDI "+opcion.Name);
                    var campo=Controles(original).OfType<TextBox>().FirstOrDefault(t=>!t.ReadOnly);
                    if(campo!=null) campo.Text="BORRADOR";
                    opcion.PerformClick(); Application.DoEvents();
                    Check(ReferenceEquals(original,app.ActiveMdiChild),"Reutiliza "+opcion.Name);
                    Check(app.MdiChildren.Count(f=>f.GetType()==original.GetType())==1,"Una instancia "+opcion.Name);
                    if(campo!=null) Check(campo.Text=="BORRADOR","Conserva edición");
                    original.WindowState=FormWindowState.Minimized;
                    opcion.PerformClick(); Application.DoEvents();
                    Check(original.WindowState!=FormWindowState.Minimized,"Restaura "+opcion.Name);
                    original.Close(); Application.DoEvents();
                    opcion.PerformClick(); Application.DoEvents();
                    Check(!ReferenceEquals(original,app.ActiveMdiChild),"Reabre después de cerrar");
                    app.ActiveMdiChild.Close();
                }
            }
            using(var configForm=new FrmConfiguracionBD())
            {
                configForm.ShowInTaskbar=false; configForm.Opacity=0; configForm.Show();
                foreach(var idioma in manager.ListarIdiomas())
                {
                    manager.SetIdiomaActual(idioma);
                    configForm.ActualizarIdioma(idioma); Application.DoEvents();
                    foreach(var c in Controles(configForm).Where(c=>c is Button || c is TextBox || c is ComboBox))
                        Check(c.Bottom<=c.Parent.ClientSize.Height && c.Right<=c.Parent.ClientSize.Width,"Configuración recortada "+c.Name);
                    using(var bmp=new Bitmap(configForm.Width,configForm.Height)) {
                        configForm.DrawToBitmap(bmp,new Rectangle(Point.Empty,configForm.Size));
                        bmp.Save(Path.Combine(salida,"Conexion-"+idioma.Codigo+".png"));
                    }
                }
            }
            Console.WriteLine("TOTAL UI: "+checks+" comprobaciones.");
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
