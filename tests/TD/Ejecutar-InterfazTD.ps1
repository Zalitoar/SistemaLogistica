param([Parameter(Mandatory=$true)][string]$BasePruebas)
$ErrorActionPreference='Stop'
if($BasePruebas -notmatch '^SistemaLogistica_TD_Test_[a-zA-Z0-9_]+$') { throw 'Se requiere una base aislada de pruebas.' }
$raiz=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Set-Location $raiz
$destino=Join-Path $raiz 'artifacts/td-tests'
Copy-Item 'IngSoft/bin/Debug/*.dll' $destino
Copy-Item 'IngSoft/bin/Debug/IngSoft.exe' $destino
$csc='C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
& $csc /nologo /target:exe "/out:$destino/InterfazTD.exe" "/reference:$destino/BE.dll" "/reference:$destino/BLL.dll" "/reference:$destino/DAL.dll" "/reference:$destino/Servicios.dll" "/reference:$destino/IngSoft.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll tests/TD/InterfazTD.cs
if($LASTEXITCODE -ne 0) { throw 'Falló compilación de pruebas de interfaz.' }
$conexion="Data Source=.\SQLEXPRESS;Initial Catalog=$BasePruebas;Integrated Security=True;TrustServerCertificate=True"
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration><connectionStrings><add name="SQL" connectionString="$conexion" providerName="System.Data.SqlClient"/></connectionStrings></configuration>
"@ | Set-Content "$destino/InterfazTD.exe.config" -Encoding UTF8
& "$destino/InterfazTD.exe" "$destino/capturas"
if($LASTEXITCODE -ne 0) { throw 'Fallaron las pruebas de interfaz.' }
