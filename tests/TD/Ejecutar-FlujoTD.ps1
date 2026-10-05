param([Parameter(Mandatory=$true)][string]$BasePruebas, [string]$Servidor='.\SQLEXPRESS')
$ErrorActionPreference='Stop'
if($BasePruebas -notmatch '^SistemaLogistica_TD_Test_[a-zA-Z0-9_]+$') { throw 'Se requiere una base aislada de pruebas.' }
$raiz=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Set-Location $raiz
$destino=Join-Path $raiz 'artifacts/td-tests'
New-Item -ItemType Directory -Path $destino -Force | Out-Null
Copy-Item 'IngSoft/bin/Debug/*.dll' $destino
$csc='C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
& $csc /nologo /target:exe "/out:$destino/FlujoTD.exe" "/reference:$destino/BE.dll" "/reference:$destino/BLL.dll" "/reference:$destino/DAL.dll" "/reference:$destino/Servicios.dll" /reference:System.Configuration.dll /reference:System.Data.dll tests/TD/FlujoTD.cs
if($LASTEXITCODE -ne 0) { throw 'Falló compilación de pruebas.' }
$builder=[System.Data.SqlClient.SqlConnectionStringBuilder]::new()
$builder['Data Source']=$Servidor
$builder['Initial Catalog']=$BasePruebas
$builder['Integrated Security']=$true
$builder['TrustServerCertificate']=$true
$conexion=[System.Security.SecurityElement]::Escape($builder.ConnectionString)
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration><connectionStrings><add name="SQL" connectionString="$conexion" providerName="System.Data.SqlClient"/></connectionStrings></configuration>
"@ | Set-Content "$destino/FlujoTD.exe.config" -Encoding UTF8
& "$destino/FlujoTD.exe"
if($LASTEXITCODE -ne 0) { throw 'Fallaron las pruebas de integración.' }
