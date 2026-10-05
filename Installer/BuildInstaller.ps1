[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+(\.\d+)?$')]
    [string]$Version = "1.0.0",

    [ValidateSet("Release")]
    [string]$Configuration = "Release",

    [string]$DotNetInstaller = "",

    [string]$InnoCompiler = ""
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$installerDir = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $installerDir "..")).Path

$projectPath = Join-Path $repoRoot "IngSoft\IngSoft.csproj"
$releaseDir = Join-Path $repoRoot "IngSoft\bin\$Configuration"
$issPath = Join-Path $installerDir "SistemaLogistica.iss"
$outputDir = Join-Path $repoRoot "artifacts\installer"

function Find-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"

    if (Test-Path $vswhere) {
        $result = & $vswhere `
            -latest `
            -products * `
            -requires Microsoft.Component.MSBuild `
            -find "MSBuild\**\Bin\MSBuild.exe" |
            Select-Object -First 1

        if ($result -and (Test-Path $result)) {
            return $result
        }
    }

    $cmd = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    throw "No se encontró MSBuild.exe. Instale Visual Studio/Build Tools con MSBuild."
}

function Find-InnoCompiler {
    param([string]$ExplicitPath)

    if ($ExplicitPath) {
        if (-not (Test-Path $ExplicitPath)) {
            throw "No existe ISCC.exe en: $ExplicitPath"
        }
        return (Resolve-Path $ExplicitPath).Path
    }

    $cmd = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    # Conservar un arreglo incluso cuando el filtro encuentra cero o una ruta.
    $candidates = @(
        @(
            (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"),
            (Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe")
        ) | Where-Object { $_ -and (Test-Path $_) }
    )

    if ($candidates.Count -gt 0) {
        return $candidates[0]
    }

    throw "No se encontró Inno Setup 6 (ISCC.exe). Instálelo o use -InnoCompiler."
}

if (-not (Test-Path $projectPath)) {
    throw "No se encontró el proyecto: $projectPath"
}

$msbuild = Find-MSBuild
$iscc = Find-InnoCompiler -ExplicitPath $InnoCompiler

Write-Host ""
Write-Host "=== SistemaLogistica - Generación de instalador ===" -ForegroundColor Cyan
Write-Host "Repositorio : $repoRoot"
Write-Host "MSBuild     : $msbuild"
Write-Host "Inno Setup  : $iscc"
Write-Host "Versión     : $Version"
Write-Host ""

Write-Host "[1/4] Compilando IngSoft en Release..." -ForegroundColor Yellow

& $msbuild `
    $projectPath `
    /t:Rebuild `
    /p:Configuration=$Configuration `
    /p:Platform=AnyCPU `
    /m `
    /nologo

if ($LASTEXITCODE -ne 0) {
    throw "Falló la compilación de IngSoft."
}

Write-Host "[2/4] Verificando salida Release..." -ForegroundColor Yellow

$requiredFiles = @(
    "IngSoft.exe",
    "IngSoft.exe.config",
    "BE.dll",
    "BLL.dll",
    "DAL.dll",
    "Servicios.dll"
)

foreach ($name in $requiredFiles) {
    $path = Join-Path $releaseDir $name
    if (-not (Test-Path $path)) {
        throw "Falta un archivo requerido en Release: $path"
    }
}

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

Write-Host "[3/4] Compilando Setup.exe..." -ForegroundColor Yellow

$innoArgs = @(
    "/DAppVersion=$Version",
    "/DSourceDir=$releaseDir",
    "/DOutputDir=$outputDir"
)

if ($DotNetInstaller) {
    if (-not (Test-Path $DotNetInstaller)) {
        throw "No existe el instalador de .NET indicado: $DotNetInstaller"
    }

    $dotnetResolved = (Resolve-Path $DotNetInstaller).Path
    $innoArgs += "/DIncludeDotNet472=1"
    $innoArgs += "/DDotNetInstaller=$dotnetResolved"

    Write-Host "Se incluirá .NET Framework 4.7.2 offline." -ForegroundColor Green
}
else {
    Write-Host "No se embebe .NET Framework." -ForegroundColor DarkYellow
    Write-Host "El Setup verificará que exista .NET Framework 4.7.2 o superior." -ForegroundColor DarkYellow
}

$innoArgs += $issPath

& $iscc @innoArgs

if ($LASTEXITCODE -ne 0) {
    throw "Falló la compilación del instalador con Inno Setup."
}

$setup = Join-Path $outputDir "SistemaLogistica-Setup-$Version.exe"

if (-not (Test-Path $setup)) {
    throw "Inno Setup terminó pero no se encontró el instalador esperado: $setup"
}

$hash = (Get-FileHash -Algorithm SHA256 $setup).Hash

Write-Host "[4/4] Instalador generado correctamente." -ForegroundColor Green
Write-Host ""
Write-Host "Archivo : $setup" -ForegroundColor Green
Write-Host "SHA256  : $hash"
Write-Host ""
Write-Host "IMPORTANTE: este instalador NO instala SQL Server ni crea/publica la base de datos." -ForegroundColor Cyan
