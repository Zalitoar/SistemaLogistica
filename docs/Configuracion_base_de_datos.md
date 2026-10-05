# Configuración de conexión e inicialización de la base

La aplicación comprueba su conexión y los objetos requeridos **antes del login**.
Si no existe una configuración, el servidor no responde, la autenticación falla
o la base no es compatible, muestra el asistente con un mensaje y no abre el login.
Cancelar el asistente de arranque cierra la aplicación.

## Primera ejecución

1. Instale SQL Server o SQL Server Express, o utilice un servidor disponible.
2. Ingrese `servidor\instancia`. Para un puerto TCP explícito, ingrese solamente
   el nombre o dirección del host y complete **Puerto TCP**; deje 0 si usa una
   instancia. También se admite el formato `tcp:host,puerto` en Servidor con Puerto 0.
3. **Buscar instancias** ofrece las instancias visibles en la red. La búsqueda
   depende de SQL Browser, UDP 1434 y el firewall; no encontrar un servidor no
   significa que no exista. Siempre puede ingresarlo manualmente.
4. Seleccione **Usar autenticación de Windows**, o desmárquela para ingresar un
   usuario y una contraseña de **SQL Server**. El servidor debe admitir ese modo.
   Son las dos modalidades implementadas para SQL Server/Express; no se agrega
   autenticación Microsoft Entra/Azure ni selección de otra identidad Windows.
5. La conexión está cifrada. **Confiar en el certificado del servidor** omite su
   validación y sólo debe marcarse para un servidor conocido, por ejemplo un
   entorno local con certificado autofirmado. La opción está desmarcada por defecto.
6. Pulse **Conectar y listar bases**. Se muestran las bases de usuario en línea a
   las que la cuenta tiene acceso. Puede escribir el nombre de una base accesible
   que no aparezca por las restricciones de visibilidad del servidor.
7. Seleccione una base existente o escriba un nombre nuevo y pulse **Crear base vacía**.
8. Pulse **Verificar estructura de la base**. Si está vacía se habilita el bloque
   **Administrador inicial de SistemaLogistica**. Si ya está completa, puede guardar.
9. Para una base vacía, ingrese una **contraseña nueva de al menos 8 caracteres** y
   repítala exactamente. Pulse **Inicializar base** y confirme el destino.
10. Tras completar la inicialización, pulse **Guardar conexión**. En el login
    ingrese con el usuario `admin` y la contraseña que acaba de elegir.

## Dos contraseñas diferentes

Para cargar ejemplos después de inicializar el esquema, consulte
[Datos de prueba de Transporte y distribución](Datos_prueba_TD.md).
El botón **Inicializar datos de prueba** se habilita al verificar una base compatible.
No forma parte de la inicialización obligatoria: se ejecuta sólo si lo solicita.

| Campo | Cuenta a la que pertenece | Cuándo se utiliza |
|---|---|---|
| Contraseña SQL Server | Usuario del motor de base de datos | Al conectar con autenticación SQL Server. No se usa con autenticación Windows. |
| Contraseña nueva en Administrador inicial de SistemaLogistica | Usuario `admin` de la aplicación | Sólo al inicializar una base vacía. Luego se usa en el login de SistemaLogistica. |

Crear una base vacía sólo crea su contenedor. No crea todavía tablas, procedimientos,
traducciones ni usuarios. Por eso **Guardar conexión permanece deshabilitado**
hasta inicializarla y verificarla. La contraseña inicial no cambia la contraseña
del usuario de SQL Server y no modifica usuarios de una base ya existente.

## Qué comprueba Verificar estructura de la base

Se conecta a la base seleccionada y compara sus tablas y procedimientos con un
manifiesto generado desde los objetos incluidos en `Database.sqlproj`. Comprueba
las columnas y sus tipos SQL, las restricciones con nombre y los índices con nombre
incluidos en los scripts. Requiere `VIEW DEFINITION` para poder consultar metadatos.
Además exige datos mínimos de arranque: idioma habilitado, usuario activo y DVV inicial.
Los faltantes se enumeran en el área inferior del formulario.

La verificación no ejecuta los procedimientos de negocio, no comprueba todos los
permisos operativos, no compara sus cuerpos ni reemplaza la validación DVH/DVV del
login. Tampoco equivale a una comparación completa de esquema SSDT (longitudes,
precisión, nulabilidad y restricciones anónimas deben revisarse al migrar).

## Inicialización segura y bases existentes

- Sólo se inicializan bases **sin objetos de usuario**. Se bloquean master, model,
  msdb y tempdb. No se borran ni sobrescriben tablas existentes.
- Las tablas se crean según sus dependencias, después los procedimientos y finalmente
  los seeds de `PostDeployment.sql` en su orden original.
- Se utiliza una transacción y un bloqueo de inicialización para evitar ejecuciones
  simultáneas en la misma base. Si falla un script, sus cambios se revierten. Si la
  base fue creada en el paso anterior, queda vacía para volver a intentar.
- El seed de seguridad recibe el hash de la contraseña elegida, en lugar de crear
  el usuario con una contraseña predeterminada. Después se calculan los DVH/DVV.
- Las credenciales SQL necesitan permisos para crear una base cuando corresponda,
  crear tablas/procedimientos, cargar datos y consultar metadatos. La aplicación
  no concede privilegios SQL ni cambia el modo de autenticación del servidor.
- Una base parcialmente creada o de otra aplicación se informa como incompatible.
  Debe actualizarse mediante el proyecto Database y un despliegue revisado por su
  administrador. Los seeds generales pueden modificar roles e integridad: no se
  ejecutan automáticamente sobre una base existente.

## Configuración desde el menú

La opción **Configurar base de datos** requiere `CONFIGURAR_BD`, asignado de forma
idempotente al rol Administrador por `007_permisos_roles.sql`. En una instalación
anterior, publique los cambios de permisos y traducciones con el procedimiento
habitual de actualización, y vuelva a iniciar sesión. No se comprueban nombres
de usuario ni nombres de roles para autorizar el formulario.

Desde una sesión abierta, guardar verifica de nuevo la base y solicita confirmación:
se guarda la configuración, se cierra la sesión y se reinicia la aplicación. Se
pierden borradores no guardados en ventanas abiertas. Hasta el reinicio, las
operaciones continúan utilizando la conexión anterior; no se mezclan bases.

## Persistencia y recuperación de la configuración

El archivo `%LOCALAPPDATA%\SistemaLogistica\conexion.dat` se protege con DPAPI
`CurrentUser`. Cada perfil Windows configura su propia conexión. No se escribe en
Program Files, no se almacena una contraseña SQL en texto plano y no se registra la
cadena en la bitácora. El cambio efectuado con sesión se audita en la base anterior.

El archivo se reemplaza de forma atómica. No se debe copiar a otro equipo o usuario:
se debe volver a configurar allí. Si falta, se admite la cadena `SQL` del `.config`
para compatibilidad con instalaciones anteriores y pruebas; el instalador nuevo la
distribuye vacía. Si el archivo protegido está dañado, se informa el problema y puede
sobrescribirse guardando una conexión válida desde el asistente.

El asistente previo al login debe permitir recuperación cuando la base está caída,
por lo que aún no dispone de permisos de aplicación. Su alcance es el perfil Windows
local; no concede acceso al servidor ni a usuarios de la aplicación. Proteja las
cuentas y perfiles Windows de los puestos, además de los permisos SQL.

## Arquitectura y paquete del instalador

No se agregan proyectos ni referencias entre proyectos. `IngSoft` usa el servicio
transversal `ConfiguracionBDManager`, que llama a DAL y trabaja con modelos BE.
DAL concentra la conexión, la consulta de metadatos y el DDL excepcional de arranque.
Las operaciones normales siguen pasando por `ACCESO` y los procedimientos existentes.

Cada compilación de IngSoft ejecuta `scripts/Generar-PaqueteBD.ps1`. Produce
`DatabaseSetup/manifest.xml` y los scripts SQL en el directorio de salida. No existe
una copia manual independiente del esquema. El generador falla ante un objeto que
no reconoce o una dependencia cíclica, en vez de omitirlo silenciosamente.
El instalador empaqueta esa carpeta junto con las DLL. No necesita SSDT, sqlcmd ni
PowerShell en el equipo destino para inicializar la base: ejecuta los lotes desde DAL.
SQL Server continúa siendo un requisito instalado por separado.

## Pruebas

`scripts/Verificar-TD.ps1 -Hasta 5` compila solución y Database y verifica que se
conserven las dependencias y los controles TD. `tests/ConfiguracionBD/Ejecutar.ps1`
usa un SQL Server local por defecto (o `-Servidor`) y crea bases aisladas con prefijo
`SistemaLogistica_Config_Test_`; no modifica la base operativa ni guarda la conexión
real del perfil. Las bases de prueba se conservan para inspección.

La prueba cubre detección de base vacía, creación, listado, inicialización, contraseña
elegida, DVH/DVV iniciales, permiso, traducciones, persistencia protegida en un archivo
de prueba, rechazo de reinicialización, objeto faltante, bases de sistema y reversión
por error inyectado. También genera una captura del asistente en `artifacts`.

Pruebas manuales adicionales: primera ejecución sin configuración, servidor apagado,
credenciales inválidas, certificado no confiable, usuario SQL válido, servidor remoto
con puerto, búsqueda con SQL Browser apagado, cancelación, cambio desde el menú y
reinicio con ventanas abiertas. Verifique también los tres idiomas y las escalas DPI
utilizadas en los puestos. La autenticación SQL exitosa y un servidor remoto requieren
credenciales y entornos disponibles para esas pruebas.

Validación realizada el 5 de octubre de 2026: solución y Database compilados,
21 comprobaciones automáticas aprobadas sobre SQL Server Express en bases aisladas
y paquete Inno Setup 1.1.0 generado con 74 scripts SQL y su manifiesto. La captura
del formulario fue revisada para comprobar la separación de credenciales del servidor
y del usuario inicial de la aplicación.

## Referencias

- [Microsoft: búsqueda de instancias y entrada manual](https://learn.microsoft.com/en-us/dotnet/api/system.data.sql.sqldatasourceenumerator.getdatasources?view=netframework-4.8.1).
- [Microsoft: protección DPAPI por usuario](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.dataprotectionscope?view=netframework-4.8.1).
