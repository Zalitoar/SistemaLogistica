# Instalador de SistemaLogistica con Inno Setup 6

Esta carpeta permite generar de forma repetible un único instalador `.exe` para la aplicación WinForms.

## Qué incluye

El instalador empaqueta la salida de `IngSoft\bin\Release\`, incluyendo el ejecutable,
su `.config`, las DLL que MSBuild copie como dependencias y `DatabaseSetup` con
los scripts SQL y su manifiesto, generados desde el proyecto Database en cada compilación.
También incluye el script opcional `TD_DatosPrueba.sql`, ejecutable desde el botón
**Inicializar datos de prueba** del asistente; no se carga automáticamente.

No incluye archivos `.pdb`.

## Qué NO incluye

- SQL Server / SQL Server Express.
- Las herramientas SSDT o el código fuente del proyecto `Database`.
- Migraciones automáticas sobre bases que ya contienen objetos.

SQL Server debe estar disponible. La aplicación permite crear e inicializar una
base vacía desde su asistente previo al login; el instalador no se conecta al servidor.

## Conexión actual

`IngSoft/App.config` se distribuye sin servidor ni base configurados. El asistente
guarda la conexión protegida con DPAPI para el usuario actual de Windows en
`%LOCALAPPDATA%\SistemaLogistica\conexion.dat`. El instalador no sobrescribe ese archivo.
Consulte [la guía de configuración](../docs/Configuracion_base_de_datos.md).

## Requisitos para generar el instalador

1. Visual Studio / Build Tools con MSBuild.
2. Inno Setup 6.

## Comando normal

Desde la raíz del repositorio:

```powershell
.\Installer\BuildInstaller.ps1 -Version 1.0.0
```

El resultado será:

`artifacts\installer\SistemaLogistica-Setup-1.0.0.exe`

## Incluir .NET Framework 4.7.2 offline

La aplicación usa .NET Framework 4.7.2.

Sin parámetro, el Setup simplemente verifica que exista 4.7.2 o una versión 4.x posterior.

Para generar un instalador autónomo respecto de .NET:

```powershell
.\Installer\BuildInstaller.ps1 `
    -Version 1.0.0 `
    -DotNetInstaller "C:\Instaladores\NDP472-KB4054530-x86-x64-AllOS-ENU.exe"
```

SQL Server continúa excluido.

## Qué hace el script

1. recompila `IngSoft` en Release;
2. compila automáticamente BE/BLL/DAL/Servicios por sus referencias;
3. verifica los archivos mínimos;
4. ejecuta Inno Setup;
5. genera un único `.exe`;
6. calcula su SHA-256.

## Prueba recomendada

### Error MSB3577: `IngSoft.FrmApp.resources` duplicado

El diseñador principal se abre desde `FrmApp.cs`. Los archivos
`FrmApp.TD.cs` y `FrmApp.ConfiguracionBD.cs` son partes de esa misma clase;
deben figurar como `SubType=Code`, dependientes de `FrmApp.cs`, y editarse
como código. No deben tener otro `InitializeComponent` ni un `.resx` propio:
los recursos del formulario, incluido el ícono, pertenecen a `FrmApp.resx`.
Un recurso asociado a cualquiera de esas partes puede producir el mismo nombre
de salida y provocar MSB3577 durante la compilación previa a Inno Setup.

### Instalación

Probar el instalador en una máquina Windows limpia o snapshot limpio:

1. preparar SQL Server y una cuenta con permisos de inicialización;
2. ejecutar el Setup;
3. crear e inicializar una base vacía desde el asistente y comprobar el login con la clave elegida;
4. probar cambio de idioma;
5. probar permisos con distintos perfiles;
6. probar los cinco casos de uso TD;
7. desinstalar;
8. verificar que la aplicación fue eliminada correctamente.
