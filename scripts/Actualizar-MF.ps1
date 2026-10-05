param(
    [Parameter(Mandatory=$true)][string]$Servidor,
    [Parameter(Mandatory=$true)][string]$BaseDatos,
    [string]$Paquete = ''
)
$ErrorActionPreference='Stop'
# Actualización aditiva de una base TD existente. No crea usuarios ni cambia claves.
if(!$Paquete) {
    $Paquete=if(Test-Path (Join-Path $PSScriptRoot 'manifest.xml')){$PSScriptRoot}else{Join-Path $PSScriptRoot '../IngSoft/bin/Release/DatabaseSetup'}
}
Add-Type -AssemblyName System.Data
[xml]$manifest=Get-Content (Join-Path $Paquete 'manifest.xml')
$builder=New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source']=$Servidor
$builder['Initial Catalog']=$BaseDatos
$builder['Integrated Security']=$true
$builder['Encrypt']=$true
$builder['TrustServerCertificate']=$true
$cn=New-Object System.Data.SqlClient.SqlConnection $builder.ConnectionString
$cn.Open()
$tx=$cn.BeginTransaction()
function Ejecutar([string]$sql) {
    foreach($lote in [regex]::Split($sql,'(?im)^\s*GO\s*(?:--[^\r\n]*)?$')) {
        if([string]::IsNullOrWhiteSpace($lote)){continue}
        $cmd=$cn.CreateCommand(); $cmd.Transaction=$tx; $cmd.CommandText=$lote; $cmd.CommandTimeout=120
        try { [void]$cmd.ExecuteNonQuery() } finally { $cmd.Dispose() }
    }
}
function Existe([string]$nombre) {
    $cmd=$cn.CreateCommand();$cmd.Transaction=$tx;$cmd.CommandText='SELECT COUNT(*) FROM sys.objects WHERE object_id=OBJECT_ID(@nombre)'
    [void]$cmd.Parameters.AddWithValue('@nombre','dbo.'+$nombre)
    try { return [int]$cmd.ExecuteScalar() -gt 0 } finally { $cmd.Dispose() }
}
try {
    if(!(Existe 'VIAJE') -or !(Existe 'TRADUCCION')){throw 'La base seleccionada no contiene el esquema TD existente.'}
    $tablas=@('UNIDAD_FLOTA','PARTE_ESTADO_UNIDAD','ORDEN_MANTENIMIENTO','INTERVENCION_MANTENIMIENTO','INFORME_DISPONIBILIDAD')
    $scripts=@($manifest.DatabaseSetup.Script | ForEach-Object { Join-Path $Paquete $_.File })
    foreach($tabla in $tablas) {
        if(!(Existe $tabla)) {
            $archivo=@($scripts | Where-Object { [IO.Path]::GetFileName($_) -match ('^\d+_'+$tabla+'\.sql$') })
            if($archivo.Count -ne 1){throw "Falta la definición de $tabla en el paquete."}
            Ejecutar (Get-Content $archivo[0] -Raw -Encoding UTF8)
        }
    }
    Ejecutar @'
IF COL_LENGTH('dbo.VIAJE','IdUnidadFlota') IS NULL ALTER TABLE dbo.VIAJE ADD IdUnidadFlota INT NULL;
IF COL_LENGTH('dbo.VIAJE','IdInformeDisponibilidad') IS NULL ALTER TABLE dbo.VIAJE ADD IdInformeDisponibilidad INT NULL;
GO
IF OBJECT_ID('dbo.FK_VIAJE_UNIDAD') IS NULL ALTER TABLE dbo.VIAJE ADD CONSTRAINT FK_VIAJE_UNIDAD FOREIGN KEY(IdUnidadFlota) REFERENCES dbo.UNIDAD_FLOTA(IdUnidadFlota);
IF OBJECT_ID('dbo.FK_VIAJE_INFORME_UNIDAD') IS NULL ALTER TABLE dbo.VIAJE ADD CONSTRAINT FK_VIAJE_INFORME_UNIDAD FOREIGN KEY(IdInformeDisponibilidad,IdUnidadFlota) REFERENCES dbo.INFORME_DISPONIBILIDAD(IdInformeDisponibilidad,IdUnidadFlota);
IF OBJECT_ID('dbo.CK_VIAJE_ASIGNACION') IS NULL ALTER TABLE dbo.VIAJE ADD CONSTRAINT CK_VIAJE_ASIGNACION CHECK((IdUnidadFlota IS NULL AND IdInformeDisponibilidad IS NULL) OR (IdUnidadFlota IS NOT NULL AND IdInformeDisponibilidad IS NOT NULL));
'@
    foreach($archivo in $scripts) {
        $nombre=[IO.Path]::GetFileName($archivo)
        if($nombre -match '^\d+_((MF_\w+)|(TD_INICIAR_VIAJE)|(TD_LISTAR_VIAJES))\.sql$') {
            $objeto=$Matches[1]
            $sql=Get-Content $archivo -Raw -Encoding UTF8
            if(Existe $objeto){$sql=[regex]::Replace($sql,'(?i)CREATE\s+PROCEDURE','ALTER PROCEDURE')}
            Ejecutar $sql
        }
        elseif($nombre -match '^\d+_seed_00[4567]_') { Ejecutar (Get-Content $archivo -Raw -Encoding UTF8) }
    }
    $tx.Commit()
    Write-Output 'MF actualizado: tablas, integración TD, procedimientos, permisos y traducciones. Reinicie la aplicación.'
}
catch { $tx.Rollback();throw }
finally { $tx.Dispose();$cn.Dispose() }
