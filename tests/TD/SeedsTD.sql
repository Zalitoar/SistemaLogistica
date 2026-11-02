-- Ejecutar con sqlcmd -b desde el directorio Database, en una base de pruebas.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
IF DB_NAME() NOT LIKE 'SistemaLogistica[_]TD[_]Test[_]%'
    THROW 50003, 'La prueba sólo admite una base aislada TD.', 1;
CREATE TABLE #Antes (Permisos INT, Relaciones INT, Traducciones INT, Planes INT);
INSERT #Antes SELECT (SELECT COUNT(*) FROM dbo.PERMISO), (SELECT COUNT(*) FROM dbo.ROL_COMPONENTE),
    (SELECT COUNT(*) FROM dbo.TRADUCCION), (SELECT COUNT(*) FROM dbo.PLAN_DISTRIBUCION);
GO
:r .\PostDeployment.sql
GO
:r .\PostDeployment.sql
GO
SELECT 'Antes' AS Momento, * FROM #Antes;
SELECT 'Despues' AS Momento, (SELECT COUNT(*) FROM dbo.PERMISO) AS Permisos,
    (SELECT COUNT(*) FROM dbo.ROL_COMPONENTE) AS Relaciones, (SELECT COUNT(*) FROM dbo.TRADUCCION) AS Traducciones,
    (SELECT COUNT(*) FROM dbo.PLAN_DISTRIBUCION) AS Planes;
IF EXISTS(SELECT 1 FROM #Antes WHERE Permisos<>(SELECT COUNT(*) FROM dbo.PERMISO)
    OR Relaciones<>(SELECT COUNT(*) FROM dbo.ROL_COMPONENTE)
    OR Traducciones<>(SELECT COUNT(*) FROM dbo.TRADUCCION)
    OR Planes<>(SELECT COUNT(*) FROM dbo.PLAN_DISTRIBUCION))
    THROW 50003, 'Los seeds modificaron los conteos al repetirse.', 1;
SELECT 'OK: seeds repetidos dos veces sin duplicados ni cambios en planes' AS Resultado;
SELECT COUNT(*) AS ClavesTD, I.Codigo_Idioma FROM dbo.TRADUCCION T
JOIN dbo.IDIOMA I ON I.Id_Idioma=T.Id_Idioma
WHERE T.Clave_Traduccion LIKE 'TD.%' OR T.Clave_Traduccion LIKE 'FrmApp.menuTD%'
   OR T.Clave_Traduccion LIKE 'FrmPlanificacionDistribucion.%' OR T.Clave_Traduccion LIKE 'FrmPrepararDespacho.%'
   OR T.Clave_Traduccion LIKE 'FrmEjecucionViaje.%' OR T.Clave_Traduccion LIKE 'FrmRegistroEntrega.%'
   OR T.Clave_Traduccion LIKE 'FrmCumplimientoViaje.%'
GROUP BY I.Codigo_Idioma;
