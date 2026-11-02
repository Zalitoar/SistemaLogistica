
$ErrorActionPreference='Stop'
$raiz=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Set-Location $raiz
$salida=Join-Path $raiz 'artifacts/mf/test'
New-Item -ItemType Directory -Force $salida | Out-Null
Copy-Item IngSoft/bin/Debug/*.dll $salida -Force
Copy-Item IngSoft/bin/Debug/IngSoft.exe $salida -Force
Copy-Item IngSoft/bin/Debug/DatabaseSetup $salida -Recurse -Force
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuild=& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
$csc=Join-Path (Split-Path $msbuild -Parent) 'Roslyn/csc.exe'
& $csc /nologo /target:exe "/out:$salida/DatosPruebaMF.exe" "/reference:$salida/BE.dll" "/reference:$salida/BLL.dll" "/reference:$salida/DAL.dll" "/reference:$salida/Servicios.dll" "/reference:$salida/IngSoft.exe" /reference:System.Data.dll /reference:System.Windows.Forms.dll /reference:System.Drawing.dll tests/MF/DatosPruebaMF.cs
if($LASTEXITCODE -ne 0){throw 'No compiló la prueba MF.'}
$argumentos=@((Join-Path $raiz 'artifacts/mf/capturas'))
& "$salida/DatosPruebaMF.exe" @argumentos
if($LASTEXITCODE -ne 0){throw 'Falló la prueba MF.'}

