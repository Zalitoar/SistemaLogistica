# Transporte y distribución

La integración posterior con Mantenimiento y Flota se describe en
[MF_implementacion.md](MF_implementacion.md). Un viaje puede tener unidad e
informe de disponibilidad asociados; su vigencia se vuelve a verificar al iniciar.

Implementado sobre develop, conservando los seis proyectos y sus referencias.
No se ejecutaron commits ni push. SQL Server se accede mediante DAL.ACCESO y
procedimientos almacenados; BLL valida permisos y reglas, y WinForms presenta los datos.

La configuración inicial de conexión y la creación del esquema desde el asistente
se documentan en [Configuración de base de datos](Configuracion_base_de_datos.md).
El instalador actual incluye los scripts necesarios para inicializar una base vacía.

## Brecha y corrección del modelo

El repositorio tenía seguridad, idiomas y bitácora, pero no implementaba TD.
La secuencia TD-02 consultaba carga prevista sin una relación que la persistiera.
Se agregó DetalleCargaPlanificada, según la aclaración funcional:

- Viaje tiene una o más filas de carga prevista.
- Cada fila referencia una Mercaderia y guarda CantidadPrevista.
- DetalleDespacho guarda la cantidad efectivamente preparada.
- Mercaderia sigue siendo un catálogo sin cantidades.
- Un producto aparece una sola vez por viaje y por despacho.

La confirmación del plan persiste plan, viajes, entregas y cargas en una transacción.
Las cardinalidades mínimas se verifican en BLL y en los procedimientos; las FK,
restricciones UNIQUE y CHECK protegen las relaciones, estados y cantidades.

## Comportamiento de los cinco casos de uso

| Caso | Formulario | Comportamiento |
| --- | --- | --- |
| TD-01 | FrmPlanificacionDistribucion | Recibir requerimiento, seleccionar uno pendiente, definir varios viajes con destinos, fechas y cargas, confirmar todo atómicamente. Incluye alta básica de mercadería porque no existía catálogo operativo. |
| TD-02 | FrmPrepararDespacho | Consultar carga prevista, ingresar cantidades preparadas, comparar y guardar. Una diferencia requiere observaciones, deja el despacho pendiente y bloquea el inicio. Se puede corregir el mismo despacho pendiente. |
| TD-03 | FrmEjecucionViaje | Consultar documento, cantidades y entregas. Iniciar sólo con despacho confirmado y cantidades coincidentes. FechaInicio se registra en SQL Server. |
| TD-04 | FrmRegistroEntrega | Registrar una vez Entregada, Parcial o Incumplida para una entrega de un viaje en curso. Parcial/Incumplida requieren motivo. Entregada emite CE-IdEntrega en la misma transacción. |
| TD-05 | FrmCumplimientoViaje | Mostrar resultados y comprobantes, cantidades de cada estado y porcentaje. Cerrar sólo viajes en curso con todas las entregas completas y sus comprobantes. |

El porcentaje es 100 × entregas Entregada / total de entregas, redondeado a dos
decimales. Parcial e Incumplida no cuentan como entregas completas. El modelo no
contiene cantidades efectivamente recibidas por destino que permitan otro cálculo.
El cierre sigue la condición del diagrama de actividad; la secuencia TD-05 describe
el camino exitoso. Resultados parciales o incumplidos quedan visibles y no habilitan
el cierre. Reintentos, replanificación y gestión completa de incidencias quedan fuera
de los cinco flujos implementados.

El catálogo y los requerimientos se reciben desde la pantalla de planificación;
no se agregaron datos comerciales ficticios a los seeds.

## Archivos creados

- BE: RequerimientoDistribucion.cs, PlanDistribucion.cs, Viaje.cs, Entrega.cs,
  DetalleCargaPlanificada.cs, DocumentoDespacho.cs, DetalleDespacho.cs, Mercaderia.cs
  y ComprobanteEntrega.cs. Proyecciones CargaDespacho.cs y CumplimientoViaje.cs,
  y ReglaTDException.cs para errores traducibles.
- BLL: GestorPlanificacion.cs, GestorDespacho.cs, GestorViaje.cs, GestorEntrega.cs,
  GestorCumplimiento.cs y ReglasTD.cs.
- DAL: MP_PlanDistribucion.cs, MP_DocumentoDespacho.cs, MP_Viaje.cs, MP_Entrega.cs
  y MP_TD.cs.
- IngSoft: los cinco formularios de la tabla anterior, cada uno con .cs,
  .Designer.cs y .resx; FrmApp.TD.cs y TDPresentacion.cs.
- Database: nueve archivos de tablas y trece de procedimientos, enumerados abajo.
- scripts/Verificar-TD.ps1.
- tests/TD: FlujoTD.cs, Ejecutar-FlujoTD.ps1, InterfazTD.cs,
  Ejecutar-InterfazTD.ps1 y SeedsTD.sql.
- Este documento.

## Archivos existentes modificados

- BE/BE.csproj, BLL/BLL.csproj, DAL/DAL.csproj, IngSoft/IngSoft.csproj y
  Database/Database.sqlproj: inclusión explícita de archivos; referencias conservadas.
- IngSoft/FrmApp.cs: inicialización y permisos del menú TD.
- Database/Scripts/Seed/007_permisos_roles.sql: permisos y asignación idempotente
  al rol Administrador. La autorización de ejecución consulta TienePermiso,
  sin comparar nombres de usuarios o roles.
- Database/Scripts/Seed/004_traducciones_es_ar.sql,
  005_traducciones_en_us.sql y 006_traducciones_pt_br.sql.
- docs/uml/Actividad_TD.puml, Clases_TD_Dominio.puml,
  DER_TD_CrowsFoot.puml, Secuencia_TD01_PlanificarDistribucion.puml y
  Secuencia_TD02_PrepararDespacho.puml.

## Tablas agregadas

En Database/dbo/Tables:

1. REQUERIMIENTO_DISTRIBUCION
2. PLAN_DISTRIBUCION
3. VIAJE
4. ENTREGA
5. MERCADERIA
6. DETALLE_CARGA_PLANIFICADA
7. DOCUMENTO_DESPACHO
8. DETALLE_DESPACHO
9. COMPROBANTE_ENTREGA

CantidadPrevista es decimal(18,3) positiva. Cantidad preparada admite cero para
registrar faltantes. Una cantidad diferente, por defecto o por exceso, impide
confirmar el despacho. DOCUMENTO_DESPACHO.Observaciones conserva el motivo
de la diferencia junto con las cantidades previstas y preparadas.

## Procedimientos agregados

En Database/dbo/Stored Procedures:

- TD_LISTAR_REQUERIMIENTOS, TD_RECIBIR_REQUERIMIENTO.
- TD_LISTAR_MERCADERIA, TD_CREAR_MERCADERIA.
- TD_CONFIRMAR_PLAN.
- TD_LISTAR_VIAJES, TD_OBTENER_ENTREGAS.
- TD_OBTENER_CARGA, TD_OBTENER_DESPACHO, TD_GUARDAR_DESPACHO.
- TD_INICIAR_VIAJE, TD_REGISTRAR_ENTREGA, TD_CERRAR_VIAJE.

Las transiciones usan transacciones, XACT_ABORT y bloqueos UPDLOCK/HOLDLOCK.
SQL vuelve a verificar el estado después de las validaciones de BLL, incluyendo
confirmaciones concurrentes, doble inicio, doble resultado y doble cierre.

## Permisos, idiomas y bitácora

Permisos agregados:

- TD_PLANIFICAR_DISTRIBUCION
- TD_PREPARAR_DESPACHO
- TD_EJECUTAR_VIAJE
- TD_REGISTRAR_ENTREGA
- TD_CONTROLAR_CUMPLIMIENTO

Se agregaron 136 claves por idioma: 408 traducciones entre es-AR, en-US y pt-BR.
Incluyen títulos, controles, columnas, menú, estados, confirmaciones y errores.
Las claves siguen NombreFormulario.Title, NombreFormulario.NombreControl.Text,
la convención existente de grillas .HeaderText y claves TD.* para mensajes comunes.
Se reutilizan FormularioTraducible, su IIdiomaObserver, IdiomaManager y ObtenerTexto.
Las grillas y fechas adoptan el formato del idioma seleccionado.

Se registran mediante BitacoraManager.Registrar la creación/confirmación del plan,
preparación pendiente o confirmada, inicio, resultado de entrega y cierre.
Los eventos tienen identificadores y caben en los 50 caracteres de la tabla
BITACORA existente. No se modificó su esquema.

## Verificaciones realizadas

- Se compilaron la solución y Database después de cada caso de uso, corrigiendo
  errores antes de continuar. Se volvieron a compilar tras los ajustes finales.
- Referencias, herencia traducible, inclusión de formularios, permisos y llamadas
  a bitácora verificadas por scripts/Verificar-TD.ps1.
- Publicación real del DACPAC en una base SQL Server Express aislada.
- La base temporal SistemaLogistica_TD_Test_20261005_01a10bcf se eliminó al finalizar
  las pruebas; las capturas y los scripts de reproducción se conservaron.
- 46 comprobaciones de integración BLL→DAL→SQL: permisos, validación de carga,
  planificación múltiple, rollback, diferencias y corrección de despacho, inicio,
  entregas, comprobantes, porcentajes, cierre, bitácora y concurrencia.
- 362 comprobaciones WinForms: cinco formularios en tres idiomas, textos y columnas,
  separación de carga/entregas por viaje, navegación, permisos del menú y relación MDI.
  Capturas locales en artifacts/td-tests/capturas, excluidas de Git.
- Seeds ejecutados dos veces sin duplicar permisos, relaciones ni traducciones,
  ni modificar el número de planes.
- git diff --check sin errores.

Se corrigió durante la prueba de navegación un evento que podía mostrar los detalles
del viaje anterior. Los formularios ahora actualizan el detalle en CurrentCellChanged.

### Corrección de foco en cumplimiento (instalador 1.0.1)

Se reprodujo la excepción SetCurrentCellAddressCore reportada en la aplicación
instalada: al quitar la celda actual mientras Cerrar viaje tenía foco, la actualización
síncrona deshabilitaba el botón y WinForms intentaba enfocar la grilla dentro de su
propio cambio de celda.

FrmCumplimientoViaje ahora agrupa las notificaciones de selección y difiere la
actualización mediante BeginInvoke. Omite los estados transitorios durante una
recarga, utiliza la selección vigente y descarta callbacks cuando el formulario
está cerrado. Las reglas de cierre y el esquema SQL no cambiaron.

La prueba tests/TD/ReentranciaCumplimientoTD.cs falló con la misma excepción antes
de la corrección y pasó después: 13 comprobaciones de foco MDI, selección rápida,
recarga, idiomas, persistencia del cierre y descarte de actualizaciones pendientes.
La regresión pasó tanto con Debug como con los binarios Release empaquetados en
1.0.1. La base temporal de esta regresión fue eliminada al terminar.
Se ejecuta en una base aislada con:

```powershell
./tests/TD/Ejecutar-ReentranciaCumplimientoTD.ps1 -BasePruebas SistemaLogistica_TD_Test_Local
```

Para repetir las comprobaciones, compilar con:

```powershell
./scripts/Verificar-TD.ps1 -Hasta 5
```

Publicar Database/bin/Debug/Database.dacpac en una base nueva cuyo nombre comience
por SistemaLogistica_TD_Test_. Los scripts de pruebas rechazan otros nombres.
No usar una base de la aplicación para estas pruebas.

```powershell
./tests/TD/Ejecutar-FlujoTD.ps1 -BasePruebas SistemaLogistica_TD_Test_Local
./tests/TD/Ejecutar-InterfazTD.ps1 -BasePruebas SistemaLogistica_TD_Test_Local
# Desde Database:
sqlcmd -S '.\SQLEXPRESS' -E -C -b -I -d SistemaLogistica_TD_Test_Local -i ../tests/TD/SeedsTD.sql
```

El entorno utilizado tiene Visual Studio 18 Community, MSBuild/SSDT y .NET Framework
4.7.2. Los scripts usan las rutas de ese entorno; Verificar-TD acepta -MSBuild.
La compilación normaliza las variables duplicadas Path/PATH del host únicamente
en el proceso hijo. No instala herramientas ni modifica la configuración del sistema.

## Pruebas manuales recomendadas

1. Publicar Database en el entorno de desarrollo elegido y volver a iniciar sesión
   para recargar permisos. Comprobar las cinco opciones TD con Administrador.
2. Crear un producto y requerimiento; planificar dos viajes con destinos y cargas
   diferentes. Cambiar de viaje y verificar que cada uno conserva sus datos.
3. Intentar confirmar sin carga, sin entregas, con cantidades inválidas o números
   repetidos. Verificar mensajes traducidos y ausencia de registros parciales.
4. Preparar una cantidad menor o mayor a la prevista, agregar motivo y guardar.
   Reabrir, comprobar las diferencias e intentar iniciar. Corregir cantidades y confirmar.
5. Iniciar una sola vez. En una entrega, registrar Entregada y consultar su comprobante.
   En otro viaje registrar Parcial/Incumplida con motivo.
6. Verificar porcentaje, pendientes y comprobantes; cerrar el viaje completamente
   entregado y comprobar que los viajes con incumplimientos no se cierran.
7. Cambiar entre español, inglés y portugués con ventanas abiertas. Revisar también
   las confirmaciones, errores, columnas y estados. Probar a la resolución y escala
   de Windows utilizadas habitualmente.
8. Probar un rol con un solo permiso TD, comprobar las opciones habilitadas y revisar
   la bitácora de las cinco operaciones.

## Límites y pendientes operativos

- La base de la aplicación no fue publicada ni modificada; la publicación verificada
  se realizó sobre una base aislada. Falta publicar el DACPAC en el entorno elegido.
- Los resultados de entrega son definitivos en este alcance; no se incluyen anulación
  de comprobantes, reintentos, replanificación ni gestión de incidencias.
- El servicio BitacoraManager existente ignora fallas de auditoría y no participa en
  la transacción de negocio. Se reutiliza tal como está, sin refactorizarlo.
- Los despachos pendientes conservan su preparación actual y pueden corregirse;
  no se añadió un historial versionado de cantidades.
- No se realizaron pruebas interactivas completas con un usuario final ni a todas
  las escalas DPI. Las pantallas se renderizaron y verificaron programáticamente.
