using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BE;
using BLL;
using DAL;
using Servicios;

// Ejecutable de integración sin un nuevo proyecto. Usar sólo con la base aislada de pruebas.
internal static class FlujoTD
{
    private static int comprobaciones;
    private static readonly string[] permisos = {
        GestorPlanificacion.Permiso, GestorDespacho.Permiso, GestorViaje.Permiso,
        GestorEntrega.Permiso, GestorCumplimiento.Permiso };
    private static void Check(bool ok, string nombre)
    {
        if (!ok) throw new Exception("FALLO: " + nombre);
        comprobaciones++;
        Console.WriteLine("OK: " + nombre);
    }
    private static void Rechaza(Action accion, string clave, string nombre)
    {
        try { accion(); }
        catch (ReglaTDException ex) { Check(ex.Message == clave, nombre + " (" + ex.Message + ")"); return; }
        throw new Exception("Debía rechazarse: " + nombre);
    }
    private static int Count(string sql)
    {
        using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQL"].ConnectionString))
        using (var cmd = new SqlCommand(sql, cn)) { cn.Open(); return Convert.ToInt32(cmd.ExecuteScalar()); }
    }
    private static Viaje NuevoViaje(string numero, int producto, int entregas)
    {
        var viaje = new Viaje { Numero=numero, FechaPrevista=DateTime.Now.AddHours(1) };
        viaje.CargaPlanificada.Add(new DetalleCargaPlanificada { IdMercaderia=producto, CantidadPrevista=5.125m });
        for (int i=0;i<entregas;i++) viaje.Entregas.Add(new Entrega { Destino="Destino "+i, FechaPrevista=viaje.FechaPrevista.AddHours(i+1) });
        return viaje;
    }
    private static List<CargaDespacho> Carga(int producto, decimal cantidad)
    {
        // CantidadPrevista adulterada a propósito: BLL y SQL deben utilizar la persistida.
        return new List<CargaDespacho> { new CargaDespacho { IdMercaderia=producto, CantidadPrevista=cantidad, CantidadPreparada=cantidad } };
    }
    private static void Flujo()
    {
        string prefijo="TEST-"+Guid.NewGuid().ToString("N").Substring(0,10);
        var planificador=new GestorPlanificacion();
        var expedicion=new GestorDespacho();
        var transporte=new GestorViaje();
        var entregas=new GestorEntrega();
        var control=new GestorCumplimiento();
        Rechaza(() => planificador.ObtenerPendientes(),"TD.SinPermiso","sesión ausente");
        SessionManager.Login(new BE.Usuario { Nombre="TD_TEST" });
        SessionManager.SetPermisos(new List<string>());
        Rechaza(() => planificador.Confirmar(null),"TD.SinPermiso","permiso planificación");
        Rechaza(() => expedicion.Guardar(0,null,null,null),"TD.SinPermiso","permiso despacho");
        Rechaza(() => transporte.Iniciar(0),"TD.SinPermiso","permiso ejecución");
        Rechaza(() => entregas.Registrar(0,0,null,DateTime.Now,null),"TD.SinPermiso","permiso entrega");
        Rechaza(() => control.Cerrar(0),"TD.SinPermiso","permiso cumplimiento");
        SessionManager.SetPermisos(permisos.ToList());
        int producto=planificador.CrearMercaderia(prefijo,"Producto de prueba");
        int req=planificador.RecibirRequerimiento(prefijo,"Depósito de prueba");
        var plan=new PlanDistribucion { IdRequerimiento=req, Numero=prefijo };
        Rechaza(() => planificador.Confirmar(plan),"TD.PlanIncompleto","plan sin viajes");
        var primero=NuevoViaje(prefijo+"-1",producto,2);
        var segundo=NuevoViaje(prefijo+"-2",producto,3);
        plan.Viajes.Add(primero); plan.Viajes.Add(segundo);
        primero.CargaPlanificada[0].CantidadPrevista=0;
        Rechaza(() => planificador.Confirmar(plan),"TD.CantidadInvalida","carga prevista cero");
        primero.CargaPlanificada[0].CantidadPrevista=5.1251m;
        Rechaza(() => planificador.Confirmar(plan),"TD.CantidadInvalida","precisión de cantidades");
        primero.CargaPlanificada[0].CantidadPrevista=5.125m;
        int planId=planificador.Confirmar(plan);
        Check(!planificador.ObtenerPendientes().Any(r => r.IdRequerimiento==req),"requerimiento planificado");
        var viajes=new MP_Viaje().Listar().Where(v => v.IdPlan==planId).OrderBy(v => v.Numero).ToList();
        Check(viajes.Count==2,"plan con dos viajes");
        int v1=viajes[0].IdViaje, v2=viajes[1].IdViaje;
        Check(new MP_Entrega().Obtener(v1).Count==2 && new MP_Entrega().Obtener(v2).Count==3,"entregas de cada viaje");
        Check(expedicion.ObtenerCarga(v1).Single().CantidadPrevista==5.125m,"carga prevista persistida");
        Rechaza(() => planificador.Confirmar(plan),"TD.EstadoInvalido","doble confirmación de requerimiento");
        int req2=planificador.RecibirRequerimiento(prefijo+"-ROLLBACK","Origen");
        var invalido=new PlanDistribucion { IdRequerimiento=req2, Numero=prefijo+"-ROLLBACK", Viajes=new List<Viaje> { NuevoViaje(primero.Numero,producto,1) } };
        Rechaza(() => planificador.Confirmar(invalido),"TD.Duplicado","número de viaje duplicado");
        Check(planificador.ObtenerPendientes().Any(r => r.IdRequerimiento==req2),"rollback conserva requerimiento pendiente");
        Check(Count("SELECT COUNT(*) FROM dbo.PLAN_DISTRIBUCION WHERE Id_Requerimiento="+req2)==0,"rollback no deja plan huérfano");
        Rechaza(() => expedicion.Guardar(v1,prefijo+"-D1","",Carga(producto,4)),"TD.ExpliqueDiferencia","diferencia exige observación");
        var doc=expedicion.Guardar(v1,prefijo+"-D1","Falta una unidad",Carga(producto,4.125m));
        Check(doc.Estado=="Pendiente","diferencia deja despacho pendiente");
        Check(expedicion.ObtenerCarga(v1).Single().Diferencia==-1m,"diferencia persistida");
        Rechaza(() => transporte.Iniciar(v1),"TD.InicioNoHabilitado","inicio bloqueado en BLL");
        Rechaza(() => new MP_Viaje().Iniciar(v1),"TD.EstadoInvalido","inicio bloqueado en SQL");
        var confirmado=expedicion.Guardar(v1,prefijo+"-D1","Carga corregida",Carga(producto,5.125m));
        Check(confirmado.Estado=="Confirmado" && confirmado.IdDespacho==doc.IdDespacho,"corrección reutiliza despacho");
        Rechaza(() => expedicion.Guardar(v1,prefijo+"-D1","",Carga(producto,5.125m)),"TD.EstadoInvalido","despacho confirmado inmutable");
        transporte.Iniciar(v1);
        Check(new MP_Viaje().Listar().Single(v => v.IdViaje==v1).FechaInicio.HasValue,"inicio registrado");
        Rechaza(() => transporte.Iniciar(v1),"TD.InicioNoHabilitado","doble inicio rechazado");
        var lista=entregas.ObtenerEntregas(v1);
        Rechaza(() => entregas.Registrar(v1,lista[0].IdEntrega,"Entregada",DateTime.Now.AddDays(1),""),"TD.FechaResultado","fecha futura rechazada");
        entregas.Registrar(v1,lista[0].IdEntrega,"Entregada",DateTime.Now,"Recibido");
        Check(control.Analizar(v1).Porcentaje==50m,"cumplimiento intermedio 50%");
        Rechaza(() => control.Cerrar(v1),"TD.CierreNoHabilitado","pendientes bloquean cierre");
        Rechaza(() => new MP_Viaje().Cerrar(v1),"TD.EstadoInvalido","SQL bloquea cierre prematuro");
        entregas.Registrar(v1,lista[1].IdEntrega,"Entregada",DateTime.Now,"Recibido");
        var resumen=control.Analizar(v1);
        Check(resumen.Comprobantes==2 && resumen.Porcentaje==100m && resumen.PuedeCerrar,"comprobantes y cumplimiento total");
        Rechaza(() => entregas.Registrar(v1,lista[0].IdEntrega,"Entregada",DateTime.Now,""),"TD.EstadoInvalido","resultado no duplicable");
        control.Cerrar(v1);
        Check(control.Analizar(v1).Viaje.Estado=="Cerrado" && control.Analizar(v1).Viaje.FechaFinalizacion.HasValue,"cierre registrado");
        Rechaza(() => control.Cerrar(v1),"TD.CierreNoHabilitado","doble cierre rechazado");
        expedicion.Guardar(v2,prefijo+"-D2","",Carga(producto,5.125m));
        transporte.Iniciar(v2);
        lista=entregas.ObtenerEntregas(v2);
        Rechaza(() => entregas.Registrar(v2,lista[0].IdEntrega,"Parcial",DateTime.Now,""),"TD.ExpliqueResultado","resultado parcial exige motivo");
        entregas.Registrar(v2,lista[0].IdEntrega,"Parcial",DateTime.Now,"Recepción parcial");
        entregas.Registrar(v2,lista[1].IdEntrega,"Incumplida",DateTime.Now,"Destinatario ausente");
        entregas.Registrar(v2,lista[2].IdEntrega,"Entregada",DateTime.Now,"");
        resumen=control.Analizar(v2);
        Check(resumen.Porcentaje==33.33m && resumen.Comprobantes==1 && resumen.Parciales==1 && resumen.Incumplidas==1,"resultados mixtos y redondeo");
        Rechaza(() => control.Cerrar(v2),"TD.CierreNoHabilitado","incumplimientos bloquean cierre");
        Check(Count("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora='TD plan creado/confirmado #"+planId+"'")==1,"bitácora plan");
        Check(Count("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora LIKE 'TD despacho "+doc.IdDespacho+" %'")==2,"bitácora despacho pendiente y confirmado");
        Check(Count("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora='TD: inicio de viaje "+v1+"'")==1,"bitácora inicio");
        Check(Count("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora='TD cierre de viaje "+v1+"'")==1,"bitácora cierre");
        Check(Count("SELECT COUNT(*) FROM dbo.BITACORA WHERE Actividad_Bitacora='TD entrega "+lista[2].IdEntrega+" Entregada'")==1,"bitácora entrega");
        Check(Count("SELECT COUNT(*) FROM dbo.PERMISO P JOIN dbo.ROL_COMPONENTE R ON R.Id_Componente=P.Id_Permiso JOIN dbo.PERMISO A ON A.Id_Permiso=R.Id_Rol WHERE A.Nombre_Permiso='Administrador' AND P.Nombre_Permiso LIKE 'TD[_]%'")==5,"administrador posee cinco permisos TD");
        Check(Count("SELECT COUNT(*) FROM dbo.TRADUCCION T JOIN dbo.IDIOMA I ON I.Id_Idioma=T.Id_Idioma WHERE T.Clave_Traduccion='FrmPlanificacionDistribucion.Title' AND I.Codigo_Idioma IN ('es-AR','en-US','pt-BR')")==3,"traducciones publicadas");
        int reqConcurrente=planificador.RecibirRequerimiento(prefijo+"-RACE","Origen");
        var resultados=new bool[2];
        Parallel.For(0,2,i => {
            var p=new PlanDistribucion { IdRequerimiento=reqConcurrente,Numero=prefijo+"-RACE-"+i,Viajes=new List<Viaje>{NuevoViaje(prefijo+"-RACE-"+i,producto,1)} };
            try { new MP_PlanDistribucion().Confirmar(p); resultados[i]=true; }
            catch(ReglaTDException ex) { if(ex.Message!="TD.EstadoInvalido") throw; }
        });
        Check(resultados.Count(r => r)==1,"confirmaciones concurrentes: sólo una tiene éxito");
        Check(Count("SELECT COUNT(*) FROM dbo.PLAN_DISTRIBUCION WHERE Id_Requerimiento="+reqConcurrente)==1,"cardinalidad requerimiento-plan bajo concurrencia");
        SessionManager.Logout();
    }
    private static int Main()
    {
        try
        {
            var builder=new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SQL"].ConnectionString);
            if(!builder.InitialCatalog.StartsWith("SistemaLogistica_TD_Test_",StringComparison.Ordinal))
                throw new Exception("La prueba requiere una base aislada SistemaLogistica_TD_Test_*.");
            Flujo();
            Console.WriteLine("TOTAL: "+comprobaciones+" comprobaciones correctas.");
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
