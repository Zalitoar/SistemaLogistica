CREATE PROCEDURE dbo.TD_INICIAR_VIAJE @IdViaje INT AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);
        IF EXISTS(SELECT 1 FROM dbo.VIAJE WHERE Id_Viaje=@IdViaje AND IdUnidadFlota IS NOT NULL)
            AND NOT EXISTS(SELECT 1 FROM dbo.VIAJE V JOIN dbo.UNIDAD_FLOTA U ON U.IdUnidadFlota=V.IdUnidadFlota
                JOIN dbo.INFORME_DISPONIBILIDAD I ON I.IdInformeDisponibilidad=V.IdInformeDisponibilidad AND I.IdUnidadFlota=U.IdUnidadFlota
                WHERE V.Id_Viaje=@IdViaje AND U.Activa=1 AND U.EstadoOperativo='Disponible' AND I.Vigente=1 AND I.Disponible=1)
            THROW 50001,'TD.EstadoInvalido',1;
        IF NOT EXISTS(SELECT 1 FROM dbo.VIAJE WITH(UPDLOCK,HOLDLOCK) WHERE Id_Viaje=@IdViaje AND Estado='Preparado')
            THROW 50001,'TD.EstadoInvalido',1;
        DECLARE @IdDespacho INT=(SELECT Id_Despacho FROM dbo.DOCUMENTO_DESPACHO WHERE Id_Viaje=@IdViaje AND Estado='Confirmado');
        IF @IdDespacho IS NULL OR NOT EXISTS(SELECT 1 FROM dbo.ENTREGA WHERE Id_Viaje=@IdViaje)
            OR NOT EXISTS(SELECT 1 FROM dbo.DETALLE_CARGA_PLANIFICADA WHERE Id_Viaje=@IdViaje)
            OR EXISTS(SELECT Id_Mercaderia,CantidadPrevista FROM dbo.DETALLE_CARGA_PLANIFICADA WHERE Id_Viaje=@IdViaje
                EXCEPT SELECT Id_Mercaderia,Cantidad FROM dbo.DETALLE_DESPACHO WHERE Id_Despacho=@IdDespacho)
            OR EXISTS(SELECT Id_Mercaderia,Cantidad FROM dbo.DETALLE_DESPACHO WHERE Id_Despacho=@IdDespacho
                EXCEPT SELECT Id_Mercaderia,CantidadPrevista FROM dbo.DETALLE_CARGA_PLANIFICADA WHERE Id_Viaje=@IdViaje)
            THROW 50001,'TD.EstadoInvalido',1;
        UPDATE dbo.VIAJE SET Estado='EnCurso',FechaInicio=GETDATE() WHERE Id_Viaje=@IdViaje;
        COMMIT;
        SELECT @IdViaje AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
