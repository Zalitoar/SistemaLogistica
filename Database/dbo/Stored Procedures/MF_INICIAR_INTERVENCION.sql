CREATE PROCEDURE dbo.MF_INICIAR_INTERVENCION @IdOrdenMantenimiento INT AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);

DECLARE @Unidad INT=(SELECT IdUnidadFlota FROM dbo.ORDEN_MANTENIMIENTO WHERE IdOrdenMantenimiento=@IdOrdenMantenimiento AND EstadoOrden='Programada');
IF @Unidad IS NULL THROW 50105,'MF.EstadoInvalido',1;
INSERT dbo.INTERVENCION_MANTENIMIENTO(IdOrdenMantenimiento,FechaInicio,Kilometraje,TrabajoRealizado,Resultado)
SELECT @IdOrdenMantenimiento,GETDATE(),KilometrajeActual,N'','EnCurso' FROM dbo.UNIDAD_FLOTA WHERE IdUnidadFlota=@Unidad;
DECLARE @Id INT=CAST(SCOPE_IDENTITY() AS INT);
UPDATE dbo.ORDEN_MANTENIMIENTO SET EstadoOrden='EnCurso' WHERE IdOrdenMantenimiento=@IdOrdenMantenimiento;
UPDATE dbo.UNIDAD_FLOTA SET EstadoOperativo='EnMantenimiento' WHERE IdUnidadFlota=@Unidad;
SELECT @Id AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
