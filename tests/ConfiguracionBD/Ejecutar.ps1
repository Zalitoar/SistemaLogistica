param([string]$Servidor='.\SQLEXPRESS')
$ErrorActionPreference='Stop'
$raiz=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$salida=Join-Path $raiz 'artifacts/configuracion-bd/pruebas'
New-Item -ItemType Directory -Force $salida | Out-Null
Copy-Item (Join-Path $raiz 'IngSoft/bin/Debug/*.dll') $salida
Copy-Item (Join-Path $raiz 'IngSoft/bin/Debug/IngSoft.exe') $salida
Copy-Item (Join-Path $raiz 'IngSoft/bin/Debug/DatabaseSetup') $salida -Recurse -Force
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuild=& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
$csc=Join-Path (Split-Path $msbuild -Parent) 'Roslyn/csc.exe'
& $csc /nologo /target:exe "/out:$salida/VerificarConexion.exe" "/reference:$salida/BE.dll" "/reference:$salida/DAL.dll" "/reference:$salida/Servicios.dll" "/reference:$salida/IngSoft.exe" /reference:System.Data.dll /reference:System.Xml.Linq.dll /reference:System.Windows.Forms.dll /reference:System.Drawing.dll (Join-Path $PSScriptRoot 'VerificarConexion.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló la prueba de configuración.' }
$nombre='SistemaLogistica_Config_Test_'+[DateTime]::Now.ToString('yyyyMMddHHmmss')
& "$salida/VerificarConexion.exe" $nombre $Servidor $salida
if ($LASTEXITCODE -ne 0) { throw "Falló la prueba. Revisar bases aisladas con prefijo $nombre" }
