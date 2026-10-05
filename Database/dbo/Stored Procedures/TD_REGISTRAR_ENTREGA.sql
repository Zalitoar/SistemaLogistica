CREATE PROCEDURE dbo.TD_REGISTRAR_ENTREGA
    @IdViaje INT, @IdEntrega INT, @Resultado VARCHAR(20), @Fecha DATETIME, @Observaciones NVARCHAR(1000)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @Inicio DATETIME;
        SELECT @Inicio=FechaInicio FROM dbo.VIAJE WITH(UPDLOCK,HOLDLOCK) WHERE Id_Viaje=@IdViaje AND Estado='EnCurso';
        IF @Inicio IS NULL OR NOT EXISTS(SELECT 1 FROM dbo.ENTREGA WITH(UPDLOCK,HOLDLOCK) WHERE Id_Entrega=@IdEntrega AND Id_Viaje=@IdViaje AND Estado='Pendiente')
            THROW 50001,'TD.EstadoInvalido',1;
        IF @Resultado IS NULL OR @Resultado NOT IN ('Entregada','Parcial','Incumplida') OR @Fecha IS NULL OR @Fecha<@Inicio OR @Fecha>GETDATE()
            OR (@Resultado<>'Entregada' AND NULLIF(LTRIM(RTRIM(@Observaciones)),'') IS NULL)
            THROW 50002,'TD.DatosInvalidos',1;
        UPDATE dbo.ENTREGA SET Estado=@Resultado,FechaRealizada=@Fecha,Observaciones=@Observaciones WHERE Id_Entrega=@IdEntrega;
        IF @Resultado='Entregada'
            INSERT dbo.COMPROBANTE_ENTREGA(Id_Entrega,Numero,Fecha,Resultado,Observaciones)
            VALUES (@IdEntrega,CONCAT('CE-',@IdEntrega),@Fecha,@Resultado,@Observaciones);
        UPDATE dbo.VIAJE SET PorcentajeCumplimiento=(SELECT CAST(100.0*SUM(CASE WHEN Estado='Entregada' THEN 1 ELSE 0 END)/COUNT(*) AS DECIMAL(5,2))
            FROM dbo.ENTREGA WHERE Id_Viaje=@IdViaje) WHERE Id_Viaje=@IdViaje;
        COMMIT;
        SELECT @IdEntrega AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
