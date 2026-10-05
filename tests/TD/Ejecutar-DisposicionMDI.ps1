param([string]$BasePruebas)
$ErrorActionPreference='Stop'
if ($BasePruebas -and $BasePruebas -notmatch '^SistemaLogistica_TD_Test_UI_[0-9]+$') { throw 'Se requiere una base aislada de interfaz.' }
$raiz=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Set-Location $raiz
$destino=Join-Path $raiz 'artifacts/ui-tests'
New-Item -ItemType Directory -Force $destino | Out-Null
Copy-Item 'IngSoft/bin/Debug/*.dll' $destino -Force
Copy-Item 'IngSoft/bin/Debug/IngSoft.exe' $destino -Force
Copy-Item 'IngSoft/bin/Debug/DatabaseSetup' $destino -Recurse -Force
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuild=& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
$csc=Join-Path (Split-Path $msbuild -Parent) 'Roslyn/csc.exe'
& $csc /nologo /target:exe "/out:$destino/DisposicionMDI.exe" "/reference:$destino/BE.dll" "/reference:$destino/BLL.dll" "/reference:$destino/DAL.dll" "/reference:$destino/Servicios.dll" "/reference:$destino/IngSoft.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll tests/TD/DisposicionMDI.cs
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de pruebas de interfaz.' }
$argumentos=@("$destino/capturas")
if ($BasePruebas) { $argumentos+=$BasePruebas }
& "$destino/DisposicionMDI.exe" @argumentos
if ($LASTEXITCODE -ne 0) { throw 'Fallaron las pruebas de disposición y MDI.' }
