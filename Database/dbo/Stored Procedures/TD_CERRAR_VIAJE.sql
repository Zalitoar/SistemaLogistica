CREATE PROCEDURE dbo.TD_CERRAR_VIAJE @IdViaje INT AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS(SELECT 1 FROM dbo.VIAJE WITH(UPDLOCK,HOLDLOCK) WHERE Id_Viaje=@IdViaje AND Estado='EnCurso')
            THROW 50001,'TD.EstadoInvalido',1;
        IF NOT EXISTS(SELECT 1 FROM dbo.ENTREGA WHERE Id_Viaje=@IdViaje)
            OR EXISTS(SELECT 1 FROM dbo.ENTREGA E LEFT JOIN dbo.COMPROBANTE_ENTREGA C ON C.Id_Entrega=E.Id_Entrega
                WHERE E.Id_Viaje=@IdViaje AND (E.Estado<>'Entregada' OR C.Id_Comprobante IS NULL))
            THROW 50001,'TD.EstadoInvalido',1;
        UPDATE dbo.VIAJE SET Estado='Cerrado',FechaFinalizacion=GETDATE(),PorcentajeCumplimiento=100 WHERE Id_Viaje=@IdViaje;
        COMMIT;
        SELECT @IdViaje AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
