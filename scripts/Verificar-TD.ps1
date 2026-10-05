param([int]$Hasta = 5, [string]$MSBuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe')
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
function Assert-TD($ok, $message) { if (!$ok) { throw $message } }
function Build-TD($target, $platform) {
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $MSBuild
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    # Normaliza claves duplicadas del entorno del host, sin modificar el sistema.
    $start.EnvironmentVariables.Clear()
    foreach ($entry in [Environment]::GetEnvironmentVariables().GetEnumerator()) {
        $start.EnvironmentVariables[$entry.Key] = $entry.Value
    }
    $start.Arguments = "$target /t:Build /p:Configuration=Debug /p:Platform=`"$platform`" /p:UseSharedCompilation=false /v:minimal /nologo"
    $process = [Diagnostics.Process]::Start($start)
    $output = $process.StandardOutput.ReadToEndAsync()
    $errors = $process.StandardError.ReadToEndAsync()
    $process.WaitForExit()
    Write-Output $output.Result
    Write-Output $errors.Result
    Assert-TD ($process.ExitCode -eq 0) "Falló compilación de $target"
}
Build-TD 'SistemaLogistica.sln' 'Any CPU'
Build-TD 'Database/Database.sqlproj' 'AnyCPU'
$expected = @{ BE=@(); DAL=@('BE'); Servicios=@('BE','DAL'); BLL=@('BE','DAL','Servicios'); IngSoft=@('BE','BLL','Servicios') }
foreach ($p in $expected.Keys) {
    [xml]$xml = Get-Content "$p/$p.csproj"
    $actual = @($xml.Project.ItemGroup.ProjectReference | Where-Object { $_ } | ForEach-Object { $_.Name } | Sort-Object)
    Assert-TD (($actual -join ',') -eq (($expected[$p] | Sort-Object) -join ',')) "Referencias incorrectas en $p"
}
$forms = @('FrmPlanificacionDistribucion','FrmPrepararDespacho','FrmEjecucionViaje','FrmRegistroEntrega','FrmCumplimientoViaje')
$gestores = @('GestorPlanificacion','GestorDespacho','GestorViaje','GestorEntrega','GestorCumplimiento')
$permissions = @('TD_PLANIFICAR_DISTRIBUCION','TD_PREPARAR_DESPACHO','TD_EJECUTAR_VIAJE','TD_REGISTRAR_ENTREGA','TD_CONTROLAR_CUMPLIMIENTO')
$seed = Get-Content Database/Scripts/Seed/007_permisos_roles.sql -Raw
$menu = Get-Content IngSoft/FrmApp.TD.cs -Raw
$project = Get-Content IngSoft/IngSoft.csproj -Raw
for ($i=0; $i -lt $Hasta; $i++) {
    $form = $forms[$i]
    $code = Get-Content "IngSoft/$form.cs" -Raw
    $bll = Get-Content ("BLL/" + $gestores[$i] + ".cs") -Raw
    Assert-TD ($code.Contains(': FormularioTraducible')) "Formulario no traducible: $form"
    Assert-TD ($project.Contains("$form.cs") -and $project.Contains("$form.Designer.cs")) "Formulario no incorporado: $form"
    Assert-TD ($menu.Contains("new $form()") -and $menu.Contains('TienePermiso')) "Falta menú o permiso de $form"
    Assert-TD ($bll.Contains('ReglasTD.Permiso') -and $bll.Contains($permissions[$i]) -and $seed.Contains($permissions[$i])) "Falta permiso en $form"
    Assert-TD ($bll.Contains('BitacoraManager.Registrar')) "Falta bitácora en $form"
    foreach($language in @('004_traducciones_es_ar','005_traducciones_en_us','006_traducciones_pt_br')) {
        $translations = Get-Content "Database/Scripts/Seed/$language.sql" -Raw
        Assert-TD ($translations.Contains("$form.Title")) "Falta título $form en $language"
    }
}
Write-Output "TD-01 a TD-0${Hasta}: compilaciones, referencias, formularios traducibles, permisos y bitácora verificados."
