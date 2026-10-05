# Datos de prueba de Mantenimiento y Flota

En el asistente de conexión, seleccionar una base inicializada y pulsar
**Verificar estructura de la base**. Cuando resulte compatible se habilita
**Inicializar datos de prueba MF**. Confirmar la carga y anotar la contraseña
mostrada al finalizar. No hace falta cargar primero los ejemplos TD.

El botón conserva las mismas reglas de acceso y ejecución en segundo plano que
la carga TD. No se habilita para bases vacías, incompatibles o durante otra operación.
Los scripts no se ejecutan al instalar ni al inicializar el esquema: la carga
de ejemplos requiere pulsar expresamente el botón.

## Usuarios creados

| Usuario | Permisos |
|---|---|
| `demo_flota` | Registrar estados y dar de alta unidades. |
| `demo_mantenimiento` | Programar, iniciar/finalizar intervenciones y habilitar unidades. |
| `demo_trafico_mf` | Consultar disponibilidad y asignar unidades a viajes. |

Los tres comparten una contraseña aleatoria generada para esa carga. Se muestra
una sola vez y únicamente se persiste su hash. El administrador y los usuarios
TD conservan sus contraseñas. Los roles MF no reciben permisos de administración
ni de configuración de base de datos. Para completar también operaciones TD,
usar un usuario que posea los permisos correspondientes, por ejemplo el administrador.

## Escenarios disponibles

| Dominio | Situación inicial | Qué probar |
|---|---|---|
| `MFDEMO01` | Disponible, control sin mantenimiento e informe vigente. | Registrar otra observación o asignarla al viaje libre. |
| `MFDEMO02` | Fuera de servicio, parte pendiente. | Programar mantenimiento en MF-02. |
| `MFDEMO03` | Orden programada, sin intervenciones. | Iniciar intervención en MF-03. |
| `MFDEMO04` | Intervención abierta. | Registrar trabajos y finalizar la intervención. |
| `MFDEMO05` | Intervención finalizada con tareas adicionales pendientes. | Iniciar otra intervención de la misma orden. |
| `MFDEMO06` | Intervención satisfactoria; pendiente de verificación. | Habilitar la unidad desde MF-04. |
| `MFDEMO07` | Mantenimiento cerrado, informe favorable vigente y asignación. | Consultar la reserva y su informe en MF-05; iniciar su viaje desde TD. |

El conjunto contiene 7 unidades, 7 partes, 5 órdenes, 4 intervenciones y 8 informes
(incluye informes históricos no vigentes), además de 22 registros de bitácora.

También crea un producto `DEMO-MF-CARGA`, un requerimiento, un plan y dos viajes
con carga prevista, despacho confirmado y una entrega pendiente cada uno:

- `DEMO-MF-VIAJE-1`: preparado y sin unidad, listo para probar la asignación.
- `DEMO-MF-VIAJE-2`: preparado y asignado a `MFDEMO07` con su informe vigente.

Los viajes y documentos MF son independientes de los ejemplos `DEMO-TD-*`.
Se puede cargar TD antes o después de MF, incluso cuando sus ejemplos ya avanzaron.

## Repetición, integridad e instalación

La marca `DEMO MF v1 cargado` de la bitácora evita duplicaciones. Repetir la carga
no restablece contraseñas, estados ni avances. Si existen nombres reservados para
los ejemplos sin esa marca, se rechaza la operación para no apropiarse de datos
existentes. Una falla revierte todos los cambios de esa carga.

Se comprueban los verificadores DVH/DVV de los usuarios antes de crear las cuentas;
después se incorporan sus verificadores. No se encubre una inconsistencia previa.

El SQL está en `Database/Scripts/DevData/MF_DatosPrueba.sql`, incluido como archivo
de contenido en Database y como `DemoMFScript` en el manifiesto del paquete.
DAL ejecuta el script con el hash parametrizado, Servicios verifica acceso y
compatibilidad, y el formulario sólo coordina la interacción.

El instalador **1.5.1** incluye el SQL y el nuevo botón. Si la base ya tenía MF,
`scripts/Actualizar-MF.ps1` permite incorporar las nuevas traducciones de los seeds;
si todavía tiene únicamente TD, también incorpora el esquema MF. Una base nueva
recibe todo el esquema y las traducciones al inicializarse.

Las pruebas se reproducen con `tests/MF/Ejecutar-DatosPruebaMF.ps1`, después de
compilar en Debug. Crean bases aisladas `SistemaLogistica_MF_Demo_Test_<fecha>`
y una variante `_TDPrimero`; no modifican la conexión guardada ni las bases operativas.
Comprueban rollback, usuarios y permisos, conservación de claves, escenarios,
coexistencia en ambos órdenes, idempotencia, avance mediante BLL y el botón traducido.
Resultado: 35 comprobaciones aprobadas; solución y Database compilados, e
instalador 1.5.1 generado correctamente.
