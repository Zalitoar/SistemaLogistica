CREATE PROCEDURE dbo.MF_FINALIZAR_INTERVENCION @IdIntervencionMantenimiento INT,@Kilometraje DECIMAL(18,3),@TrabajoRealizado NVARCHAR(1000),@Resultado VARCHAR(30),@Observaciones NVARCHAR(1000) AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);

DECLARE @Orden INT=(SELECT IdOrdenMantenimiento FROM dbo.INTERVENCION_MANTENIMIENTO WHERE IdIntervencionMantenimiento=@IdIntervencionMantenimiento AND Resultado='EnCurso');
DECLARE @Unidad INT=(SELECT IdUnidadFlota FROM dbo.ORDEN_MANTENIMIENTO WHERE IdOrdenMantenimiento=@Orden AND EstadoOrden='EnCurso');
IF @Unidad IS NULL OR @Resultado NOT IN ('Satisfactoria','RequiereTareas') THROW 50105,'MF.EstadoInvalido',1;
IF @Kilometraje<(SELECT KilometrajeActual FROM dbo.UNIDAD_FLOTA WHERE IdUnidadFlota=@Unidad) THROW 50102,'MF.Kilometraje',1;
IF NULLIF(LTRIM(RTRIM(@TrabajoRealizado)),'') IS NULL OR (@Resultado='RequiereTareas' AND NULLIF(LTRIM(RTRIM(@Observaciones)),'') IS NULL) THROW 50106,'MF.DatosInvalidos',1;
UPDATE dbo.INTERVENCION_MANTENIMIENTO SET FechaFin=GETDATE(),Kilometraje=@Kilometraje,TrabajoRealizado=@TrabajoRealizado,Resultado=@Resultado,Observaciones=@Observaciones WHERE IdIntervencionMantenimiento=@IdIntervencionMantenimiento;
UPDATE dbo.ORDEN_MANTENIMIENTO SET EstadoOrden=CASE WHEN @Resultado='Satisfactoria' THEN 'PendienteVerificacion' ELSE 'Programada' END WHERE IdOrdenMantenimiento=@Orden;
UPDATE dbo.UNIDAD_FLOTA SET KilometrajeActual=@Kilometraje,EstadoOperativo=CASE WHEN @Resultado='Satisfactoria' THEN 'PendienteVerificacion' ELSE 'EnMantenimiento' END WHERE IdUnidadFlota=@Unidad;
SELECT @IdIntervencionMantenimiento AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
