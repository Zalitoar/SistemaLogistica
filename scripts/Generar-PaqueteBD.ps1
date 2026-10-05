param([Parameter(Mandatory=$true)][string]$Destino)
$ErrorActionPreference = 'Stop'
$base = Join-Path (Split-Path $PSScriptRoot -Parent) 'Database'
[xml]$proyecto = Get-Content (Join-Path $base 'Database.sqlproj')
$archivos = @($proyecto.Project.ItemGroup.Build | Where-Object { $_ } | ForEach-Object { $_.Include })
$tablas = @{}
$procedimientos = @()
foreach ($archivo in $archivos) {
    $texto = Get-Content (Join-Path $base $archivo) -Raw -Encoding UTF8
    $match = [regex]::Match($texto, '(?i)CREATE\s+(TABLE|PROCEDURE|PROC)\s+\[?dbo\]?\.\[?(\w+)\]?')
    if (!$match.Success) { throw "Objeto no soportado en el paquete: $archivo" }
    $nombre = $match.Groups[2].Value
    $objeto = [pscustomobject]@{ Nombre=$nombre; Texto=$texto; Dependencias=@([regex]::Matches($texto, '(?i)REFERENCES\s+\[?dbo\]?\.\[?(\w+)\]?') | ForEach-Object { $_.Groups[1].Value }) }
    if ($match.Groups[1].Value -eq 'TABLE') { $tablas[$nombre]=$objeto }
    else { $procedimientos += $objeto }
}
$ordenadas = @()
while ($tablas.Count -gt 0) {
    $listas = @($tablas.Values | Where-Object { $n=$_.Nombre; @($_.Dependencias | Where-Object { $_ -ne $n -and $tablas.ContainsKey($_) }).Count -eq 0 } | Sort-Object Nombre)
    if ($listas.Count -eq 0) { throw 'Dependencias cíclicas entre tablas; revisar el generador.' }
    foreach ($t in $listas) { $ordenadas += $t; $tablas.Remove($t.Nombre) }
}
New-Item -ItemType Directory -Force $Destino | Out-Null
$manifest = New-Object System.Xml.XmlDocument
$raiz = $manifest.CreateElement('DatabaseSetup'); $manifest.AppendChild($raiz) | Out-Null
$raiz.SetAttribute('Version','1')
$indice = 0
foreach ($obj in @($ordenadas) + @($procedimientos | Sort-Object Nombre)) {
    $tipo = if ($ordenadas -contains $obj) { 'U' } else { 'P' }
    $elemento = $manifest.CreateElement('Object'); $elemento.SetAttribute('Name',$obj.Nombre); $elemento.SetAttribute('Type',$tipo)
    if ($tipo -eq 'U') {
        foreach ($col in [regex]::Matches($obj.Texto, '(?im)^\s*\[?(\w+)\]?\s+(INT|BIGINT|SMALLINT|BIT|N?VARCHAR|N?CHAR|DATETIME2?|DATE|DECIMAL|NUMERIC|VARBINARY)\b')) {
            $c=$manifest.CreateElement('Column'); $c.SetAttribute('Name',$col.Groups[1].Value); $c.SetAttribute('SqlType',$col.Groups[2].Value.ToLowerInvariant()); $elemento.AppendChild($c) | Out-Null
        }
        foreach ($matchConstraint in [regex]::Matches($obj.Texto, '(?i)CONSTRAINT\s+\[?(\w+)\]?')) {
            $c=$manifest.CreateElement('Constraint'); $c.SetAttribute('Name',$matchConstraint.Groups[1].Value); $elemento.AppendChild($c) | Out-Null
        }
        foreach ($matchIndex in [regex]::Matches($obj.Texto, '(?i)CREATE\s+(?:UNIQUE\s+)?(?:NONCLUSTERED\s+|CLUSTERED\s+)?INDEX\s+\[?(\w+)\]?')) {
            $c=$manifest.CreateElement('Index'); $c.SetAttribute('Name',$matchIndex.Groups[1].Value); $elemento.AppendChild($c) | Out-Null
        }
    }
    $raiz.AppendChild($elemento) | Out-Null
    $indice++; $nombreArchivo=('{0:D3}_{1}.sql' -f $indice,$obj.Nombre)
    [IO.File]::WriteAllText((Join-Path $Destino $nombreArchivo),$obj.Texto,[Text.Encoding]::UTF8)
    $script=$manifest.CreateElement('Script'); $script.SetAttribute('File',$nombreArchivo); $raiz.AppendChild($script) | Out-Null
}
$post = Get-Content (Join-Path $base 'PostDeployment.sql') -Raw
foreach ($inc in [regex]::Matches($post, '(?m)^:r\s+(.+?)\s*$')) {
    $ruta=Join-Path $base $inc.Groups[1].Value.Trim()
    $texto=Get-Content $ruta -Raw -Encoding UTF8
    if ([IO.Path]::GetFileName($ruta) -eq '002_seguridad_basica.sql') {
        $patron="(?i)DECLARE @ClaveAdmin VARCHAR\(64\) = '[0-9a-f]+';"
        if (![regex]::IsMatch($texto,$patron)) { throw 'No se encontró el parámetro de clave inicial.' }
        $texto=[regex]::Replace($texto,$patron,'DECLARE @ClaveAdmin VARCHAR(64) = @ClaveInicial;')
    }
    $indice++; $nombreArchivo=('{0:D3}_seed_{1}' -f $indice,[IO.Path]::GetFileName($ruta))
    [IO.File]::WriteAllText((Join-Path $Destino $nombreArchivo),$texto,[Text.Encoding]::UTF8)
    $script=$manifest.CreateElement('Script'); $script.SetAttribute('File',$nombreArchivo); $raiz.AppendChild($script) | Out-Null
}
$demo='TD_DatosPrueba.sql'
Copy-Item (Join-Path $base "Scripts/DevData/$demo") (Join-Path $Destino $demo) -Force
$opcional=$manifest.CreateElement('DemoScript'); $opcional.SetAttribute('File',$demo); $raiz.AppendChild($opcional) | Out-Null
$manifest.Save((Join-Path $Destino 'manifest.xml'))
Write-Output "Paquete BD generado: $indice scripts en $Destino"
