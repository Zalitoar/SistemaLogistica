# MF — Mantenimiento y disponibilidad de flota

## Alcance y criterios de integración

Se implementaron los cinco casos de uso de los PlantUML suministrados en
`docs/uml`, conservando los proyectos y sus referencias. El recorrido es:
registrar estado → programar mantenimiento → iniciar/finalizar intervenciones
→ verificar y habilitar → asignar la unidad a un viaje TD.

Se resolvieron las diferencias entre los diagramas de esta manera:

- `ServicioTransporte` del DER y del modelo conceptual corresponde al `Viaje`
  existente, como indica la secuencia MF-05. No se creó una entidad paralela.
- Una orden recién programada tiene cero intervenciones; posteriormente puede
  tener varias. Esto sigue el DER, el modelo conceptual y la secuencia MF-03,
  aunque el diagrama de clases indica un mínimo de una.
- Un informe puede no tener intervención: es necesario para el camino del
  diagrama de actividad donde el control no detecta mantenimiento pendiente.
- Se utilizan dominio, marca, modelo, año, tipo, capacidad y kilometraje para
  reunir los atributos de los modelos suministrados. La unidad intervenida se
  obtiene mediante su orden, sin duplicar esa relación en la intervención.
- Los identificadores de partes y órdenes se muestran como números de referencia;
  no se inventó una numeración documental adicional.

## Ubicación del código

| Capa | Archivos y responsabilidad |
|---|---|
| BE | `UnidadFlota`, `ParteEstadoUnidad`, `OrdenMantenimiento`, `IntervencionMantenimiento`, `InformeDisponibilidad`, `ReglaMFException`; referencias opcionales a unidad e informe en `Viaje`. |
| BLL | `GestorFlota`, `GestorMantenimiento`, `GestorDisponibilidad` y `ReglasMF`: permisos, validación de entradas, consultas, coordinación y bitácora. |
| DAL | `MP_MF`: mapeos tipados y procedimientos mediante `ACCESO`. `MP_Viaje` recupera la asignación MF. |
| IngSoft | Cinco formularios traducibles, cada uno con `.cs`, `.Designer.cs` y `.resx`; `FrmApp.MF.cs` incorpora el menú y `MFPresentacion` comparte únicamente presentación. |
| Database | Cinco tablas, doce procedimientos, extensión de `VIAJE`, permisos y traducciones. |

No hay SQL en formularios ni BLL. Las operaciones SQL vuelven a validar las
precondiciones dentro de transacciones con rollback. Las mutaciones de flota y
el inicio TD toman un bloqueo transaccional de la tabla de unidades, para evitar
asignaciones o habilitaciones concurrentes incompatibles.

## Ventanas y prueba manual

1. **Registrar estado de flota — MF-01 (`FrmEstadoFlota`).** Crear una unidad con
   dominio único, tipo, marca, modelo, año, capacidad y kilometraje. Seleccionarla
   e ingresar kilometraje, novedad y descripción. Una unidad nueva está fuera de
   servicio hasta registrar su control. Marcar «Requiere mantenimiento» genera
   un parte pendiente y la deja no disponible. Sin esa marca se genera un parte
   resuelto y un informe vigente de disponibilidad, siempre que no haya pendientes.
   La grilla inferior conserva los partes anteriores.
2. **Programar mantenimiento — MF-02 (`FrmProgramarMantenimiento`).** Seleccionar
   un parte pendiente; elegir fecha, mantenimiento preventivo o correctivo,
   prioridad y descripción del trabajo. Programar crea una única orden para ese
   parte e invalida la disponibilidad anterior. La fecha no puede ser anterior
   al día actual. La segunda grilla muestra las órdenes y sus estados.
3. **Registrar intervención — MF-03 (`FrmIntervencionMantenimiento`).** Seleccionar
   una orden programada y pulsar «Iniciar intervención». Se registra el inicio
   con la hora del servidor. Seleccionar la intervención abierta, completar
   kilometraje, trabajo realizado, resultado y observaciones, y finalizarla.
   «Requiere más tareas» exige observaciones y permite iniciar otra intervención
   de la misma orden. «Satisfactoria» deja la orden pendiente de verificación;
   todavía no habilita la unidad. No se pueden finalizar dos veces los mismos trabajos.
4. **Habilitar unidad para operar — MF-04 (`FrmHabilitarUnidad`).** Seleccionar
   la unidad y revisar sus intervenciones e informes. Confirmar la habilitación
   sólo es posible si no hay partes sin programar ni órdenes con tareas pendientes,
   y existe mantenimiento satisfactorio por verificar. Se cierran las órdenes
   verificadas, se resuelven sus partes y se emite un informe vigente. Los informes
   anteriores se conservan como historial.
5. **Consultar disponibilidad y asignar unidad — MF-05 (`FrmDisponibilidadFlota`).**
   Seleccionar un viaje planificado o preparado, consultar las unidades disponibles
   y sus informes vigentes, elegir una y asignarla. El viaje guarda tanto la unidad
   como el informe que respaldó la asignación. Antes del inicio puede reasignarse
   a otra unidad disponible. La opción incluye la unidad ya asignada al viaje actual.

Las cinco ventanas reutilizan la apertura MDI de instancia única. Los botones
de actualización recargan los datos; volver a elegir una opción del menú conserva
la ventana y sus ediciones. Los campos obligatorios, errores y confirmaciones se
presentan en el idioma activo.

## Reglas de disponibilidad

- El kilometraje no puede disminuir. Capacidad positiva; kilometrajes no negativos.
- Un parte sin necesidad de mantenimiento no puede saltarse partes u órdenes
  pendientes. No se modifica el estado de una unidad durante un viaje en curso.
- Una unidad requiere estado disponible, actividad e informe vigente favorable
  para asignarse. La asignación vuelve a comprobarse en SQL para resolver carreras.
- Sin un intervalo de ocupación definido en los diagramas, se reserva una unidad
  para un solo viaje sin cerrar. El cierre TD libera esa reserva; no se eliminan
  la asignación histórica ni su informe.
- Registrar una nueva condición invalida el informe anterior. Si un viaje ya
  estaba asignado, deberá confirmarse nuevamente su asignación con el informe
  vigente antes del inicio. TD impide iniciar con una habilitación invalidada.
- Los viajes TD sin asignación siguen admitidos para conservar el comportamiento
  existente; no se inventó una obligación de asignación que los modelos dejan opcional.
- La capacidad se registra como dato de catálogo. No se compara con la carga TD:
  los diagramas no definen una unidad de medida ni pesos/volúmenes por producto.

## Persistencia, permisos e idiomas

Tablas: `UNIDAD_FLOTA`, `PARTE_ESTADO_UNIDAD`, `ORDEN_MANTENIMIENTO`,
`INTERVENCION_MANTENIMIENTO`, `INFORME_DISPONIBILIDAD`.
`VIAJE` incorpora `IdUnidadFlota` e `IdInformeDisponibilidad` opcionales, con
restricción que exige ambos o ninguno y FK compuesta que valida la misma unidad.
Hay índices únicos para un informe vigente por unidad y una intervención abierta
por orden; una intervención puede respaldar como máximo un informe.

Procedimientos: `MF_LISTAR_UNIDADES`, `MF_LISTAR_PARTES`, `MF_LISTAR_ORDENES`,
`MF_LISTAR_INTERVENCIONES`, `MF_LISTAR_INFORMES`, `MF_CREAR_UNIDAD`,
`MF_REGISTRAR_ESTADO`, `MF_PROGRAMAR`, `MF_INICIAR_INTERVENCION`,
`MF_FINALIZAR_INTERVENCION`, `MF_HABILITAR`, `MF_ASIGNAR`.
Se ampliaron `TD_LISTAR_VIAJES` y `TD_INICIAR_VIAJE` para la integración.

Permisos idempotentes, asignados inicialmente al Administrador:
`MF_REGISTRAR_ESTADO`, `MF_PROGRAMAR_MANTENIMIENTO`, `MF_REGISTRAR_INTERVENCION`,
`MF_HABILITAR_UNIDAD`, `MF_ASIGNAR_UNIDAD`. La autorización usa `TienePermiso`;
no se comparan usuarios o roles en el código funcional.

Se agregaron 171 claves por idioma (513 traducciones) en los seeds existentes de
español, inglés y portugués. Se reutilizan `FormularioTraducible`, `IdiomaManager`
y `ObtenerTexto`. `BitacoraManager.Registrar` registra altas de unidades, partes,
programación, inicio y finalización de intervención, habilitación y asignación.

## Instalación y actualización

El instalador 1.5.0 incluye el paquete SQL actualizado. En una base nueva,
«Inicializar base» del asistente crea TD y MF conjuntamente. El botón de datos
de prueba existente conserva su conjunto TD; las unidades se crean desde MF-01.

Una base TD anterior requiere actualización del esquema antes de usar esta versión.
El asistente detectará que faltan objetos MF y no intentará recrear una base con datos.
Con la aplicación cerrada y un respaldo disponible, un administrador SQL puede ejecutar:

```powershell
.\scripts\Actualizar-MF.ps1 -Servidor '.\SQLEXPRESS' -BaseDatos 'NombreDeLaBase'
```

El script usa autenticación de Windows y, por defecto, el paquete de Release
generado por la compilación. Admite `-Paquete` para indicar otra carpeta.
También se distribuye dentro de `DatabaseSetup`: desde esa carpeta instalada se
ejecuta `Actualizar-MF.ps1` con los mismos
parámetros, y toma el paquete de su propio directorio.

La actualización es aditiva y transaccional: crea objetos MF ausentes, añade
columnas opcionales, actualiza procedimientos y ejecuta seeds de permisos e idiomas.
Puede repetirse sin duplicar datos. No reinicializa usuarios, contraseñas, planes,
viajes ni bitácora. Requiere permisos para modificar el esquema; no se ejecuta
automáticamente contra la base del usuario. Tras completarla, reiniciar la aplicación.

## Verificación y límites

- Solución y Database compilados; referencias entre proyectos conservadas.
- `tests/MF/Ejecutar-MF.ps1`: 551 comprobaciones de integración y WinForms,
  cubriendo los cinco casos, falta de permisos, kilometraje, duplicados, tareas
  adicionales, concurrencia, bitácora, integración TD, idiomas y ventana MDI única.
- Actualización aplicada dos veces sobre una base aislada creada con el esquema
  TD anterior. Las 46 comprobaciones de `tests/TD/Ejecutar-FlujoTD.ps1` pasan
  sobre esa base actualizada.
- Capturas de las cinco pantallas en los tres idiomas en `artifacts/mf/capturas`.
  Después del ajuste final de combos se repitieron 518 comprobaciones de interfaz.
  Para repetir sólo esa parte puede pasarse `-BasePruebas` al script MF con el
  nombre de una base aislada `SistemaLogistica_MF_Test_<fecha>` ya generada.
  Las pruebas crean bases temporales con prefijos `SistemaLogistica_MF_Test_` y
  `SistemaLogistica_TD_Test_MigracionMF_`, conservadas para inspección. No alteran
  la conexión del perfil ni las bases operativas.
- Probar manualmente el escalado de Windows y los tamaños de texto de destino.
  El bloqueo de flota es conservador: una instalación con mucha concurrencia
  puede requerir posteriormente bloqueos por unidad y medición de carga.
- La auditoría conserva la política existente de `BitacoraManager`: un fallo
  de bitácora no revierte una operación de negocio ya confirmada.

No se implementaron reglas no especificadas para cancelación de órdenes,
desasignación sin reemplazo, desactivación de unidades, costos, repuestos o
reservas por intervalos horarios.
