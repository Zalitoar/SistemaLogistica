CREATE PROCEDURE dbo.MF_HABILITAR @IdUnidadFlota INT,@Observaciones NVARCHAR(1000) AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);

IF NOT EXISTS(SELECT 1 FROM dbo.UNIDAD_FLOTA WHERE IdUnidadFlota=@IdUnidadFlota AND Activa=1) THROW 50105,'MF.EstadoInvalido',1;
IF EXISTS(SELECT 1 FROM dbo.PARTE_ESTADO_UNIDAD WHERE IdUnidadFlota=@IdUnidadFlota AND EstadoParte='Pendiente') OR EXISTS(SELECT 1 FROM dbo.ORDEN_MANTENIMIENTO WHERE IdUnidadFlota=@IdUnidadFlota AND EstadoOrden NOT IN ('PendienteVerificacion','Cerrada')) THROW 50104,'MF.Pendientes',1;
DECLARE @Intervencion INT=(SELECT MAX(I.IdIntervencionMantenimiento) FROM dbo.INTERVENCION_MANTENIMIENTO I JOIN dbo.ORDEN_MANTENIMIENTO O ON O.IdOrdenMantenimiento=I.IdOrdenMantenimiento WHERE O.IdUnidadFlota=@IdUnidadFlota AND O.EstadoOrden='PendienteVerificacion' AND I.Resultado='Satisfactoria');
IF @Intervencion IS NULL THROW 50105,'MF.EstadoInvalido',1;
UPDATE dbo.INFORME_DISPONIBILIDAD SET Vigente=0 WHERE IdUnidadFlota=@IdUnidadFlota;
INSERT dbo.INFORME_DISPONIBILIDAD(IdUnidadFlota,IdIntervencionMantenimiento,FechaHora,Disponible,EstadoUnidad,Observaciones,Vigente)
VALUES(@IdUnidadFlota,@Intervencion,GETDATE(),1,'Disponible',@Observaciones,1);
DECLARE @Id INT=CAST(SCOPE_IDENTITY() AS INT);
UPDATE P SET EstadoParte='Resuelto' FROM dbo.PARTE_ESTADO_UNIDAD P JOIN dbo.ORDEN_MANTENIMIENTO O ON O.IdParteEstadoUnidad=P.IdParteEstadoUnidad WHERE O.IdUnidadFlota=@IdUnidadFlota AND O.EstadoOrden='PendienteVerificacion';
UPDATE dbo.ORDEN_MANTENIMIENTO SET EstadoOrden='Cerrada' WHERE IdUnidadFlota=@IdUnidadFlota AND EstadoOrden='PendienteVerificacion';
UPDATE dbo.UNIDAD_FLOTA SET EstadoOperativo='Disponible' WHERE IdUnidadFlota=@IdUnidadFlota;
SELECT @Id AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
