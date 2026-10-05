# Datos de prueba de Transporte y distribución

El asistente **Configurar base de datos** incluye el botón **Inicializar datos de
prueba**. Sirve para preparar demostraciones y pruebas manuales de TD con usuarios,
cargas y operaciones ficticias. Los registros usan el prefijo `DEMO-TD-` y no son
datos reales ni importaciones de producción.

## Cómo cargarlos

1. Conecte al servidor y seleccione una base de pruebas.
2. Si está vacía, primero utilice **Inicializar base** con la contraseña del usuario
   `admin` de la aplicación. Este paso crea el esquema y los datos obligatorios.
3. Pulse **Verificar estructura de la base**. Cuando la base es compatible se
   habilita **Inicializar datos de prueba**.
4. Pulse ese botón y confirme la carga en la base seleccionada.
5. El asistente muestra una contraseña aleatoria común a los cuatro usuarios demo.
   Anótela: sólo se muestra al realizar la primera carga y nunca se registra en la
   bitácora. La contraseña de `admin` no cambia.
6. Guarde la conexión e ingrese con `admin` para probar todo el proceso, o con un
   usuario demo para comprobar sus permisos específicos.

Desde una sesión abierta, acceder al asistente y ejecutar la carga requiere el
permiso `CONFIGURAR_BD`. Antes del login se conserva el flujo de configuración local
del asistente. La cuenta utilizada para SQL Server debe poder insertar los datos.

## Usuarios y roles

| Usuario | Rol nuevo | Funcionalidades |
|---|---|---|
| `demo_planificador` | DEMO TD Planificacion | Planificar distribución y controlar/cerrar viajes. |
| `demo_expedicion` | DEMO TD Expedicion | Preparar despacho. |
| `demo_transporte` | DEMO TD Transporte | Iniciar viajes y registrar entregas. |
| `demo_consulta` | DEMO TD Consulta | Consultar bitácora. No tiene permisos de modificación TD. |

Estos usuarios no reciben permisos de configuración ni administración de seguridad.
El usuario administrador existente mantiene su contraseña y sus roles. Los cuatro
usuarios demo tienen asociado el idioma español. Si pierde la contraseña generada,
utilice la administración de usuarios; repetir la carga no restablece credenciales.

## Datos incluidos

| Entidad | Cantidad agregada |
|---|---:|
| Usuarios / roles demo | 4 / 4 |
| Productos de catálogo | 3 |
| Requerimientos | 8, incluido uno pendiente de planificación |
| Planes / viajes | 7 / 7 |
| Detalles de carga planificada | 21 |
| Documentos / detalles de despacho | 6 / 18 |
| Entregas | 21 |
| Comprobantes de entrega | 7 |
| Entradas de bitácora | 22 |

Los productos son cajas de alimentos, bultos de limpieza y pallets de bebidas.
Cada viaje prevé 10 cajas, 5 bultos y 2 pallets. Las cantidades están en los detalles
de carga y de despacho, nunca en el catálogo de mercadería.

Los destinos se registran en las entregas, según el modelo actual: Sucursal Centro,
Cliente Norte y Depósito Sur. No se crea una tabla paralela de destinos o envíos;
los envíos se representan mediante viajes, cargas y entregas.

## Recorrido de los ejemplos

| Viaje | Situación inicial | Prueba recomendada |
|---|---|---|
| DEMO-TD-VIAJE-1 | Planificado, sin despacho | Preparar por primera vez las tres cantidades previstas. |
| DEMO-TD-VIAJE-2 | Planificado, despacho Pendiente | Consultar faltante de 2 cajas; corregir de 8 a 10, manteniendo 5 bultos y 2 pallets. |
| DEMO-TD-VIAJE-3 | Preparado, despacho Confirmado | Confirmar el inicio y pasar al registro de entregas. |
| DEMO-TD-VIAJE-4 | En curso, tres entregas pendientes | Registrar los resultados y comprobar cómo cambia el porcentaje. |
| DEMO-TD-VIAJE-5 | En curso, una Entregada, una Parcial y una Incumplida | Consultar motivos, un comprobante y cumplimiento de 33,33 %. No permite cerrar. |
| DEMO-TD-VIAJE-6 | En curso, tres Entregada | Revisar tres comprobantes y 100 %; confirmar el cierre. |
| DEMO-TD-VIAJE-7 | Cerrado, tres Entregada | Consultar historial, inicio, cierre, comprobantes y 100 %. |

`DEMO-TD-REQ-PENDIENTE` queda disponible para crear un plan nuevo desde TD-01.
Los planes de ejemplo ya están confirmados: no aparecen como borradores editables.
Las fechas se calculan respecto del momento de carga, con inicio anterior a las
entregas y cierre posterior. Al registrar nuevas entregas, utilice la fecha real
actual y posterior al inicio efectivo, no copie automáticamente la fecha prevista.

## Repetición y protección de datos

El script ejecuta la carga en una transacción y utiliza un bloqueo para serializar
las cargas simultáneas. Si un paso falla se revierten los usuarios, roles y datos TD
insertados durante ese intento. Los datos previos se conservan.

Una entrada de bitácora `DEMO TD v1 cargado` marca la carga completada. Al repetir,
no se duplican registros, no cambian contraseñas y no se deshacen los avances hechos
por el usuario. Para empezar de cero, utilice otra base de pruebas. No borre el
marcador: si hay nombres DEMO existentes sin ese marcador, la carga se rechaza para
no apropiarse de registros ni mezclar conjuntos parciales.

Antes de agregar usuarios se verifica su integridad existente. Los nuevos usuarios
reciben DVH y se actualiza el DVV, de modo que el login mantenga sus validaciones.
Si se detecta una inconsistencia previa se cancela la carga, sin repararla ni ocultarla.
La conversión de texto a UTF-8 coincide con el hash de la aplicación, incluidos los
nombres con acentos. El script opcional requiere SQL Server 2019 o posterior para
esa intercalación; el proyecto Database utiliza el proveedor de SQL Server 2022.

## Implementación y distribución

- Fuente SQL: `Database/Scripts/DevData/TD_DatosPrueba.sql`, incluida como `None` en
  `Database.sqlproj` para revisión y distribución.
- No se incluye en `PostDeployment.sql`: publicar Database o inicializar una base
  normal no agrega automáticamente usuarios ni operaciones de demostración.
- `Generar-PaqueteBD.ps1` copia el archivo a `DatabaseSetup` y lo registra como
  `DemoScript`, separado de los scripts obligatorios de arranque.
- Servicios valida permisos y compatibilidad y genera la contraseña; DAL ejecuta
  el script con el hash como parámetro, sin SQL en el formulario.
- El instalador incluye el script opcional y verifica su presencia al empaquetar.
- Las fechas y registros de bitácora con prefijo DEMO representan hechos simulados.

## Verificación

Ejecute `scripts/Verificar-TD.ps1 -Hasta 5` para compilar la solución y Database.
`tests/ConfiguracionBD/Ejecutar.ps1` crea bases aisladas y comprueba los conteos,
contraseñas preservadas, permisos sin elevación, DVH/DVV, idempotencia y reversión
completa mediante un error inyectado durante la creación de comprobantes.

Validación realizada: solución y Database compilados; 42 comprobaciones automáticas
aprobadas en bases aisladas. Instalador 1.2.0 generado con el script opcional incluido.
