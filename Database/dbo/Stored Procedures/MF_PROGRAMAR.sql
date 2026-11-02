CREATE PROCEDURE dbo.MF_PROGRAMAR @IdParteEstadoUnidad INT,@FechaProgramada DATETIME,@TipoMantenimiento VARCHAR(30),@Prioridad VARCHAR(20),@DescripcionTrabajo NVARCHAR(500) AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);

DECLARE @Unidad INT=(SELECT IdUnidadFlota FROM dbo.PARTE_ESTADO_UNIDAD WHERE IdParteEstadoUnidad=@IdParteEstadoUnidad AND RequiereMantenimiento=1 AND EstadoParte='Pendiente');
IF @Unidad IS NULL OR @FechaProgramada<CONVERT(DATE,GETDATE()) THROW 50105,'MF.EstadoInvalido',1;
IF EXISTS(SELECT 1 FROM dbo.VIAJE WHERE IdUnidadFlota=@Unidad AND Estado='EnCurso') THROW 50103,'MF.EnViaje',1;
INSERT dbo.ORDEN_MANTENIMIENTO(IdUnidadFlota,IdParteEstadoUnidad,FechaEmision,FechaProgramada,TipoMantenimiento,Prioridad,DescripcionTrabajo,EstadoOrden)
VALUES(@Unidad,@IdParteEstadoUnidad,GETDATE(),@FechaProgramada,@TipoMantenimiento,@Prioridad,@DescripcionTrabajo,'Programada');
DECLARE @Id INT=CAST(SCOPE_IDENTITY() AS INT);
UPDATE dbo.PARTE_ESTADO_UNIDAD SET EstadoParte='Programado' WHERE IdParteEstadoUnidad=@IdParteEstadoUnidad;
UPDATE dbo.UNIDAD_FLOTA SET EstadoOperativo='EnMantenimiento' WHERE IdUnidadFlota=@Unidad;
UPDATE dbo.INFORME_DISPONIBILIDAD SET Vigente=0 WHERE IdUnidadFlota=@Unidad;
SELECT @Id AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
