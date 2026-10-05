param(
    [Parameter(Mandatory=$true)][string]$BasePruebas,
    [ValidateSet('Debug','Release')][string]$Configuration='Debug'
)
$ErrorActionPreference='Stop'
if($BasePruebas -notmatch '^SistemaLogistica_TD_Test_[a-zA-Z0-9_]+$') { throw 'Se requiere una base aislada de pruebas.' }
$raiz=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Set-Location $raiz
$destino=Join-Path $raiz 'artifacts/td-reentrancia'
New-Item -ItemType Directory -Path $destino -Force | Out-Null
Copy-Item "IngSoft/bin/$Configuration/*.dll" $destino
Copy-Item "IngSoft/bin/$Configuration/IngSoft.exe" $destino
$csc='C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
& $csc /nologo /target:exe "/out:$destino/ReentranciaCumplimientoTD.exe" "/reference:$destino/BE.dll" "/reference:$destino/BLL.dll" "/reference:$destino/DAL.dll" "/reference:$destino/Servicios.dll" "/reference:$destino/IngSoft.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Data.dll /reference:System.Configuration.dll tests/TD/ReentranciaCumplimientoTD.cs
if($LASTEXITCODE -ne 0) { throw 'Falló compilación de la prueba.' }
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration><connectionStrings><add name="SQL" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=$BasePruebas;Integrated Security=True;TrustServerCertificate=True" providerName="System.Data.SqlClient"/></connectionStrings></configuration>
"@ | Set-Content "$destino/ReentranciaCumplimientoTD.exe.config" -Encoding UTF8
& "$destino/ReentranciaCumplimientoTD.exe"
if($LASTEXITCODE -ne 0) { throw 'Falló la prueba de reentrancia.' }
