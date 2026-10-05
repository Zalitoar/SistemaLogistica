# Instalador de SistemaLogistica con Inno Setup 6

Esta carpeta permite generar de forma repetible un único instalador `.exe` para la aplicación WinForms.

## Qué incluye

El instalador empaqueta la salida de `IngSoft\bin\Release\`, incluyendo el ejecutable,
su `.config` y las DLL que MSBuild copie como dependencias.

No incluye archivos `.pdb`.

## Qué NO incluye

- SQL Server / SQL Server Express.
- El proyecto `Database`.
- Instalación o publicación del esquema.
- Creación de la base `SistemaLogistica`.

La base se prepara por separado.

## Conexión actual

La aplicación usa actualmente en `IngSoft/App.config`:

`Data Source=.\SQLEXPRESS;Initial Catalog=SistemaLogistica;Integrated Security=True`

Ese valor termina en `IngSoft.exe.config`, que sí se distribuye.

Por lo tanto, el equipo destino debe tener una instancia/base compatible con esa cadena
o debe ajustarse la configuración para apuntar al servidor SQL correcto.

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

Probar el instalador en una máquina Windows limpia o snapshot limpio:

1. preparar SQL Server y la base por el mecanismo separado;
2. ejecutar el Setup;
3. comprobar inicio y login;
4. probar cambio de idioma;
5. probar permisos con distintos perfiles;
6. probar los cinco casos de uso TD;
7. desinstalar;
8. verificar que la aplicación fue eliminada correctamente.
