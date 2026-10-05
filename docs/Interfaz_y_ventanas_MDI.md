# Interfaz y ventanas MDI

Las opciones de Transporte y distribución, Usuarios, Roles, Bitácora e Idiomas
mantienen una sola ventana abierta por tipo dentro de la ventana principal.
Volver a seleccionar una opción activa la ventana existente, conservando la
selección y las ediciones pendientes. Si estaba minimizada, se restaura maximizada.
Después de cerrarla, la opción abre una instancia nueva. Las ventanas nuevas se
abren maximizadas para aprovechar el área MDI; pueden restaurarse y redimensionarse.
La configuración de conexión sigue siendo un diálogo modal.

MDI permite múltiples instancias; no es un error del framework. Para estas
pantallas de gestión se eligió una sola instancia para evitar borradores duplicados.
La activación no actualiza automáticamente los datos: se conserva el botón
Actualizar y su comportamiento existente.

## Distribución de controles

- Las cinco pantallas TD ajustan la altura de las secciones de entrada y acciones
  al contenido. El espacio restante se asigna a las grillas.
- Los botones ajustan su ancho al texto traducido, con un tamaño mínimo legible.
  Se redujeron márgenes y algunos campos cortos sin reducir la fuente.
- Las barras distribuyen los controles en más filas cuando falta ancho.
- La planificación amplía el desplegable de productos para facilitar su lectura.
- El resumen de cumplimiento adapta el texto al ancho disponible y no reserva
  una fila vacía para un aviso inexistente.
- El asistente de conexión ocupa menos altura y conserva todas sus operaciones.

No se cambiaron reglas de negocio, persistencia, permisos, auditoría ni traducciones.

## Verificación

Se compilaron la solución y Database, y se verificaron referencias, traducción,
permisos y llamadas de bitácora mediante `scripts/Verificar-TD.ps1 -Hasta 5`.

`tests/TD/Ejecutar-DisposicionMDI.ps1` ejecuta las pruebas WinForms contra SQL Express.
Sin parámetros crea una base aislada `SistemaLogistica_TD_Test_UI_<fecha>` con los
datos demo; no modifica la conexión guardada del usuario. El parámetro
`-BasePruebas` permite reutilizar una base de ese prefijo. Las bases se conservan
para inspección. Las capturas quedan en `artifacts/ui-tests/capturas`.

Resultado: 764 comprobaciones aprobadas, incluyendo los tres idiomas, dimensiones
de cliente TD de 980×620, 900×580 y 1280×720, conservación del borrador al reabrir,
restauración de ventanas minimizadas y nueva instancia tras cerrar. También se
revisaron capturas de las pantallas. El instalador actualizado es la versión 1.2.1.

Se recomienda comprobar manualmente el escalado de Windows a 125 % y 150 % en los
equipos de destino, así como recorrer los campos con Tab y usar las grillas con
datos de longitud habitual. Las pruebas automáticas no sustituyen esa revisión
en monitores con distintas configuraciones de DPI.
