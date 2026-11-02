# Revisión de implementación TD y MF

Fecha: 5 de octubre de 2026. Rama: `develop`.

## Alcance y conclusión

Revisión estática del código actual, incluidos los cambios locales, contra los diagramas de actividad, casos de uso, clases, secuencias y DER de `docs/uml`. Se revisaron BE, BLL, DAL, formularios, referencias de proyecto, esquema SSDT, permisos y scripts de ejemplos. No se modificó la implementación ni se ejecutaron nuevas pruebas de integración o compilaciones para este informe. Los hallazgos funcionales siguientes se deducen del código; no se presentan como fallas reproducidas en una base operativa.

Los cinco casos de TD y los cinco de MF tienen implementación. La arquitectura conserva los seis proyectos existentes y sus dependencias. La correspondencia con UML no es exacta: hay diferencias de documentación, una regla de estado que necesita revisión y pasos de verificación que no se muestran anticipadamente al usuario.

## Hallazgos funcionales

| Prioridad | Hallazgo y evidencia | Consecuencia y acción sugerida |
|---|---|---|
| Media | `Database/dbo/Stored Procedures/MF_FINALIZAR_INTERVENCION.sql`, actualización de `UNIDAD_FLOTA`: calcula el estado de la unidad únicamente a partir de la intervención que acaba de finalizar. El modelo admite varias órdenes de una unidad. | Si una intervención termina satisfactoriamente mientras otra orden sigue en curso, la unidad queda `PendienteVerificacion`, aunque todavía tenga mantenimiento en ejecución. Calcular el estado considerando todas sus órdenes. `MF_HABILITAR` sí impide habilitarla con órdenes pendientes; el problema identificado es la representación del estado global. |
| Media | `Secuencia_MF04_HabilitarUnidad.puml` muestra `verificarCondiciones` y la respuesta «apta para habilitar» antes de confirmar. `FrmHabilitarUnidad.Mostrar()` sólo carga intervenciones e informes; la comprobación efectiva ocurre al ejecutar `MF_HABILITAR`. | Falta la verificación visible previa prevista por la secuencia. Mostrar aptitud y motivos pendientes antes de habilitar, manteniendo la validación transaccional al guardar. |
| Media | `BLL/GestorViaje.PuedeIniciarse()` comprueba condiciones TD; la disponibilidad de la unidad y vigencia de su informe se verifican recién en `TD_INICIAR_VIAJE.sql`. El rechazo se convierte en el mensaje genérico `TD.EstadoInvalido` mediante `DAL/MP_TD.cs`. | Una asignación que dejó de ser válida se rechaza correctamente, pero el usuario no recibe la causa concreta de MF. Incorporar un diagnóstico de disponibilidad y un mensaje específico, sin quitar la comprobación SQL. |
| Por definir | `Clases_TD_Dominio.puml` incluye `PlanDistribucion.Replanificar()`, pero no se encontró una operación equivalente en BLL, DAL o formularios. | Se puede armar el plan antes de confirmarlo, pero no replanificar uno confirmado. Las cinco secuencias no desarrollan esa operación: definir su alcance o retirar el método del diagrama si no integra el alcance actual. |

Prueba recomendada para el primer hallazgo: registrar dos partes para una unidad, programar ambas órdenes, iniciar sus intervenciones y finalizar sólo una satisfactoriamente. Revisar el estado global y comprobar que la habilitación continúe bloqueada por la otra orden.

## Diferencias entre diagramas y código

| Tema | Diferencia encontrada | Criterio para alinear |
|---|---|---|
| Orden e intervenciones MF | El diagrama de clases indica `1..*`; el DER y el modelo conceptual indican `0..*`. El código permite una orden recién programada sin intervención. | Corregir el diagrama de clases a `0..*`, coherente con MF-02 y MF-03. |
| Servicio de transporte | DER y modelo conceptual MF utilizan `ServicioTransporte`; clases y secuencia MF-05 utilizan `Viaje`. El código integra MF en `VIAJE`. | Unificar el nombre o documentar explícitamente la equivalencia. No se creó una tabla paralela de servicios. |
| Integración TD–MF | `VIAJE` incorpora unidad e informe de disponibilidad; el DER y las clases TD no reflejan completamente estas relaciones. | Actualizar los diagramas TD con las referencias opcionales y su integridad conjunta. |
| Informe sin intervención | El DER MF vincula cada informe con una intervención obligatoria. SQL permite `IdIntervencionMantenimiento` nulo y crea informes de control sin mantenimiento. | Hacer opcional esa relación para cubrir el flujo sin mantenimiento del diagrama de actividad. |
| Informe de habilitación y varias órdenes | `MF_HABILITAR` cierra todas las órdenes aptas de la unidad, pero vincula el informe sólo con una intervención satisfactoria, elegida mediante `MAX(IdIntervencionMantenimiento)`. El modelo conceptual admite varias intervenciones asociadas con un informe. | Aclarar si se necesita trazabilidad explícita de todas las intervenciones verificadas; en ese caso, la relación actual es insuficiente. |
| Numeración MF | Las clases `ParteEstadoUnidad` y `OrdenMantenimiento` muestran un atributo `numero`. BE y tablas usan identificadores y no incluyen numeración documental independiente; el DER tampoco la exige. | Definir si el ID sirve como referencia visible o agregar numeración de negocio. |
| Actor MF-03 | `CasosUso_MF.puml` no conecta al técnico con MF-03, aunque su secuencia y los permisos implementados le permiten registrar intervenciones. | Agregar la asociación del actor con MF-03. |
| Registro de condición MF-01 | La secuencia no representa todos los registros creados: el código genera parte e informe tanto si requiere mantenimiento como si no. | Ampliar la secuencia para reflejar la trazabilidad de ambos caminos. |
| Métodos de entidades | Los diagramas asignan comportamiento a entidades; BE contiene principalmente propiedades y la ejecución se distribuye entre BLL y procedimientos. | Documentar la adaptación a la arquitectura existente. No implica que todos esos comportamientos estén ausentes. |
| Unidad obligatoria para viajar | El modelo conceptual MF dice que el servicio requiere una unidad; otros diagramas permiten asignación opcional. El código permite iniciar viajes sin unidad y valida MF cuando existe asignación. | Resolver la contradicción documental y definir si la asignación debe ser obligatoria para todos los viajes. |
| Reserva de unidades | SQL impide asignar una unidad a otro viaje no cerrado, incluso si sus fechas previstas no coinciden. | Documentar esta regla conservadora; los diagramas no detallan reservas por horario. |

`Actividad_IC.puml` describe otro proceso. Registrar una entrega parcial/incumplida con observaciones no implementa la clasificación, tratamiento y cierre completo de incidencias IC. Se considera alcance separado de TD y MF, no un caso de uso implementado por estos módulos.

## Funcionalidades y ventanas

| Caso | Formulario | Implementación |
|---|---|---|
| TD-01 | `FrmPlanificacionDistribucion` | Registra/selecciona requerimientos y confirma planes con viajes, destinos, fechas, productos y cantidades previstas. |
| TD-02 | `FrmPrepararDespacho` | Consulta la carga prevista, registra cantidades preparadas y las compara. Las diferencias dejan pendiente el despacho; la coincidencia permite confirmarlo. |
| TD-03 | `FrmEjecucionViaje` | Consulta viaje, despacho, carga y entregas; registra el inicio si cumple las condiciones. |
| TD-04 | `FrmRegistroEntrega` | Registra entrega completa, parcial o incumplida. Exige observaciones cuando corresponde y genera comprobante para la entrega completa. |
| TD-05 | `FrmCumplimientoViaje` | Muestra cumplimiento y resultados; cierra el viaje cuando todas las entregas están completas y tienen comprobante. |
| MF-01 | `FrmEstadoFlota` | Da de alta unidades y registra condición, kilometraje y necesidad de mantenimiento. |
| MF-02 | `FrmProgramarMantenimiento` | Convierte un parte pendiente en orden con tipo, prioridad, fecha y trabajo previsto. |
| MF-03 | `FrmIntervencionMantenimiento` | Inicia y finaliza intervenciones; registra trabajos, kilometraje y resultado satisfactorio o tareas adicionales. |
| MF-04 | `FrmHabilitarUnidad` | Consulta antecedentes y habilita unidades aptas; cierra órdenes/partes y emite informe favorable vigente. |
| MF-05 | `FrmDisponibilidadFlota` | Consulta disponibilidad y asigna unidad e informe a un viaje TD planificado o preparado. |

## Tablas agregadas

Son **14 tablas de negocio**: nueve TD y cinco MF. No incluye tablas técnicas preexistentes reutilizadas por estos módulos.

| Proceso | Tabla | Propósito |
|---|---|---|
| TD | `REQUERIMIENTO_DISTRIBUCION` | Solicitud a planificar. |
| TD | `PLAN_DISTRIBUCION` | Cabecera del plan confirmado. |
| TD | `VIAJE` | Recorrido, fechas, estado y cumplimiento. |
| TD | `ENTREGA` | Destino, fechas y resultado. |
| TD | `MERCADERIA` | Catálogo de productos, sin cantidades. |
| TD | `DETALLE_CARGA_PLANIFICADA` | Producto y cantidad prevista por viaje. |
| TD | `DOCUMENTO_DESPACHO` | Cabecera y estado de preparación. |
| TD | `DETALLE_DESPACHO` | Producto y cantidad efectivamente preparada. |
| TD | `COMPROBANTE_ENTREGA` | Evidencia de entrega completa. |
| MF | `UNIDAD_FLOTA` | Vehículo, kilometraje y estado operativo. |
| MF | `PARTE_ESTADO_UNIDAD` | Novedad y necesidad de mantenimiento. |
| MF | `ORDEN_MANTENIMIENTO` | Programación y seguimiento del trabajo. |
| MF | `INTERVENCION_MANTENIMIENTO` | Ejecución, trabajos y resultados. |
| MF | `INFORME_DISPONIBILIDAD` | Disponibilidad histórica y vigente. |

MF también modifica `VIAJE`: agrega `IdUnidadFlota` e `IdInformeDisponibilidad`, claves foráneas y validación para que ambos estén informados conjuntamente o ambos sean nulos. La relación compuesta impide asociar un informe de otra unidad. Las tablas y procedimientos están incluidos en `Database.sqlproj`.

## Procedimientos almacenados agregados

Son **25 procedimientos de negocio**: 13 TD y 12 MF.

| Grupo | Procedimientos |
|---|---|
| TD: requerimientos y catálogo | `TD_LISTAR_REQUERIMIENTOS`, `TD_RECIBIR_REQUERIMIENTO`, `TD_LISTAR_MERCADERIA`, `TD_CREAR_MERCADERIA` |
| TD: planificación y consultas | `TD_CONFIRMAR_PLAN`, `TD_LISTAR_VIAJES`, `TD_OBTENER_ENTREGAS`, `TD_OBTENER_CARGA`, `TD_OBTENER_DESPACHO` |
| TD: operaciones | `TD_GUARDAR_DESPACHO`, `TD_INICIAR_VIAJE`, `TD_REGISTRAR_ENTREGA`, `TD_CERRAR_VIAJE` |
| MF: consultas | `MF_LISTAR_UNIDADES`, `MF_LISTAR_PARTES`, `MF_LISTAR_ORDENES`, `MF_LISTAR_INTERVENCIONES`, `MF_LISTAR_INFORMES` |
| MF: condición y programación | `MF_CREAR_UNIDAD`, `MF_REGISTRAR_ESTADO`, `MF_PROGRAMAR` |
| MF: ejecución y disponibilidad | `MF_INICIAR_INTERVENCION`, `MF_FINALIZAR_INTERVENCION`, `MF_HABILITAR`, `MF_ASIGNAR` |

La integración MF modifica además `TD_LISTAR_VIAJES` y `TD_INICIAR_VIAJE`; ya están contados entre los 13 de TD. Los scripts de datos de prueba son archivos SQL, no procedimientos adicionales.

## Permisos agregados

| Grupo | Permisos |
|---|---|
| TD | `TD_PLANIFICAR_DISTRIBUCION`, `TD_PREPARAR_DESPACHO`, `TD_EJECUTAR_VIAJE`, `TD_REGISTRAR_ENTREGA`, `TD_CONTROLAR_CUMPLIMIENTO` |
| MF | `MF_REGISTRAR_ESTADO`, `MF_PROGRAMAR_MANTENIMIENTO`, `MF_REGISTRAR_INTERVENCION`, `MF_HABILITAR_UNIDAD`, `MF_ASIGNAR_UNIDAD` |
| Configuración | `CONFIGURAR_BD` |

Son diez permisos funcionales y uno de configuración. `007_permisos_roles.sql` los incorpora de forma idempotente y los asigna al rol Administrador. La autorización en ejecución utiliza permisos, no nombres de usuarios o roles. Los roles de demostración se contabilizan aparte.

## Otros cambios

- **Capas:** entidades en BE; gestores y reglas en BLL; mapeadores `MP_*` en DAL utilizando `ACCESO`; formularios en IngSoft. No se agregó ORM ni un proyecto paralelo. Las referencias conservan IngSoft → BLL/Servicios/BE, BLL → DAL/Servicios/BE, Servicios → DAL/BE y DAL → BE.
- **Carga planificada:** cantidades previstas en `DetalleCargaPlanificada` y cantidades preparadas en `DetalleDespacho`, respetando la corrección del proceso TD.
- **Interfaz:** diez ventanas de negocio traducibles, menú TD y MF, apertura como hijos MDI y reutilización de una única instancia por formulario; controles y distribución del espacio ajustados.
- **Traducciones:** textos de controles, títulos, mensajes y estados en español, inglés y portugués mediante los seeds `004`, `005` y `006` y el mecanismo existente.
- **Auditoría:** reutilización de `BitacoraManager.Registrar` para operaciones TD y MF. No se incorporó otra bitácora.
- **Configuración inicial:** asistente antes del login ante conexión ausente/fallida, selección y comprobación de base, inicialización y acceso posterior mediante permiso. La conexión se guarda localmente protegida por DPAPI.
- **Distribución:** paquete `DatabaseSetup` generado desde el proyecto SQL e incluido en Inno Setup; contiene esquema, procedimientos, seeds y scripts opcionales de demostración. `Actualizar-MF.ps1` permite incorporar MF a una base existente.
- **Ejemplos:** botones de carga TD y MF, transacciones, detección de cargas previas, preservación de contraseñas existentes y actualización de verificadores de integridad de usuarios.
- **Documentación y pruebas:** guías en `docs`, scripts de verificación y pruebas de integración para TD, MF y configuración. Sus resultados históricos no equivalen a una ejecución nueva en esta revisión.

## Datos de prueba

Cantidades calculadas a partir de los `INSERT`, condiciones y bucles de `TD_DatosPrueba.sql` y `MF_DatosPrueba.sql`. Representan una **primera carga exitosa**, sin colisiones con nombres reservados. No son una consulta de cantidades actuales de una base. No incluyen los seeds obligatorios del esquema, el administrador inicial ni operaciones realizadas después por el usuario. Repetir una carga ya completada agrega **cero** registros.

### Conjunto TD

| Entidad | Cantidad agregada |
|---|---:|
| USUARIO | 4 |
| PERMISO — filas de tipo ROL | 4 |
| ROL_COMPONENTE | 6 |
| USUARIO_IDIOMA | 4 |
| MERCADERIA | 3 |
| REQUERIMIENTO_DISTRIBUCION | 8 |
| PLAN_DISTRIBUCION | 7 |
| VIAJE | 7 |
| DETALLE_CARGA_PLANIFICADA | 21 |
| ENTREGA | 21 |
| DOCUMENTO_DESPACHO | 6 |
| DETALLE_DESPACHO | 18 |
| COMPROBANTE_ENTREGA | 7 |
| BITACORA | 22 |

Usuarios: `demo_planificador`, `demo_expedicion`, `demo_transporte` y `demo_consulta`. Escenarios: viaje sin despacho, despacho con diferencias, preparado, en curso sin resultados, cumplimiento parcial, cumplimiento completo pendiente de cierre y viaje cerrado. Hay un requerimiento adicional pendiente de planificación.

### Conjunto MF

| Entidad | Cantidad agregada |
|---|---:|
| USUARIO | 3 |
| PERMISO — filas de tipo ROL | 3 |
| ROL_COMPONENTE | 5 |
| USUARIO_IDIOMA | 3 |
| UNIDAD_FLOTA | 7 |
| PARTE_ESTADO_UNIDAD | 7 |
| ORDEN_MANTENIMIENTO | 5 |
| INTERVENCION_MANTENIMIENTO | 4 |
| INFORME_DISPONIBILIDAD | 8 |
| MERCADERIA | 1 |
| REQUERIMIENTO_DISTRIBUCION | 1 |
| PLAN_DISTRIBUCION | 1 |
| VIAJE | 2 |
| DETALLE_CARGA_PLANIFICADA | 2 |
| ENTREGA | 2 |
| DOCUMENTO_DESPACHO | 2 |
| DETALLE_DESPACHO | 2 |
| COMPROBANTE_ENTREGA | 0 |
| BITACORA | 22 |

Usuarios: `demo_flota`, `demo_mantenimiento` y `demo_trafico_mf`. Las siete unidades representan: disponible sin mantenimiento, parte pendiente, orden programada, intervención en curso, tareas adicionales, pendiente de habilitación y habilitada/asignada. Los ocho informes incluyen históricos. Se crean dos viajes TD propios, uno libre y otro asignado; MF no necesita cargar previamente el conjunto TD.

### Total al cargar ambos conjuntos

| Entidad | Cantidad agregada |
|---|---:|
| USUARIO | 7 |
| PERMISO — filas de tipo ROL | 7 |
| ROL_COMPONENTE | 11 |
| USUARIO_IDIOMA | 7 |
| MERCADERIA | 4 |
| REQUERIMIENTO_DISTRIBUCION | 9 |
| PLAN_DISTRIBUCION | 8 |
| VIAJE | 9 |
| DETALLE_CARGA_PLANIFICADA | 23 |
| ENTREGA | 23 |
| DOCUMENTO_DESPACHO | 8 |
| DETALLE_DESPACHO | 20 |
| COMPROBANTE_ENTREGA | 7 |
| UNIDAD_FLOTA | 7 |
| PARTE_ESTADO_UNIDAD | 7 |
| ORDEN_MANTENIMIENTO | 5 |
| INTERVENCION_MANTENIMIENTO | 4 |
| INFORME_DISPONIBILIDAD | 8 |
| BITACORA | 44 |

No se agregan permisos funcionales desde los scripts demo: se vinculan los ya existentes con los roles nuevos. Se actualiza el DVV existente y se calculan los DVH de los usuarios nuevos. No existen tablas independientes `DESTINO` o `ENVIO` en este modelo: los destinos son datos de `ENTREGA` y la distribución se representa mediante planes, viajes, cargas y entregas. Cada conjunto genera una contraseña aleatoria para sus usuarios y la muestra una sola vez; repetir la carga no restablece credenciales ni estados.
