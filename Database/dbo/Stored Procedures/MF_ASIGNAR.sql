CREATE PROCEDURE dbo.MF_ASIGNAR @IdUnidadFlota INT,@IdViaje INT AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);

DECLARE @Informe INT=(SELECT I.IdInformeDisponibilidad FROM dbo.INFORME_DISPONIBILIDAD I JOIN dbo.UNIDAD_FLOTA U ON U.IdUnidadFlota=I.IdUnidadFlota WHERE I.IdUnidadFlota=@IdUnidadFlota AND I.Vigente=1 AND I.Disponible=1 AND U.Activa=1 AND U.EstadoOperativo='Disponible');
IF @Informe IS NULL OR EXISTS(SELECT 1 FROM dbo.PARTE_ESTADO_UNIDAD WHERE IdUnidadFlota=@IdUnidadFlota AND EstadoParte<>'Resuelto') THROW 50104,'MF.Pendientes',1;
IF NOT EXISTS(SELECT 1 FROM dbo.VIAJE WHERE Id_Viaje=@IdViaje AND Estado IN ('Planificado','Preparado')) THROW 50105,'MF.EstadoInvalido',1;
IF EXISTS(SELECT 1 FROM dbo.VIAJE WHERE IdUnidadFlota=@IdUnidadFlota AND Id_Viaje<>@IdViaje AND Estado<>'Cerrado') THROW 50107,'MF.Asignada',1;
UPDATE dbo.VIAJE SET IdUnidadFlota=@IdUnidadFlota,IdInformeDisponibilidad=@Informe WHERE Id_Viaje=@IdViaje;
SELECT @IdViaje AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
