CREATE PROCEDURE dbo.TD_GUARDAR_DESPACHO
    @IdViaje INT, @Numero NVARCHAR(50), @Observaciones NVARCHAR(1000), @Carga XML
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM dbo.VIAJE WITH (UPDLOCK,HOLDLOCK) WHERE Id_Viaje=@IdViaje AND Estado='Planificado')
            THROW 50001, 'TD.EstadoInvalido', 1;
        IF NULLIF(LTRIM(RTRIM(@Numero)),'') IS NULL OR @Carga IS NULL OR @Carga.exist('/carga/item')=0
            THROW 50002, 'TD.DatosInvalidos', 1;
        DECLARE @Real TABLE(IdMercaderia INT PRIMARY KEY, Cantidad DECIMAL(18,3) CHECK(Cantidad>=0));
        INSERT @Real SELECT C.value('@mercaderia','int'),C.value('@cantidad','decimal(18,3)') FROM @Carga.nodes('/carga/item') N(C);
        IF EXISTS (SELECT Id_Mercaderia FROM dbo.DETALLE_CARGA_PLANIFICADA WHERE Id_Viaje=@IdViaje EXCEPT SELECT IdMercaderia FROM @Real)
           OR EXISTS (SELECT IdMercaderia FROM @Real EXCEPT SELECT Id_Mercaderia FROM dbo.DETALLE_CARGA_PLANIFICADA WHERE Id_Viaje=@IdViaje)
            THROW 50002, 'TD.DatosInvalidos', 1;
        DECLARE @Estado VARCHAR(20) = CASE WHEN EXISTS (
            SELECT 1 FROM dbo.DETALLE_CARGA_PLANIFICADA P JOIN @Real R ON R.IdMercaderia=P.Id_Mercaderia
            WHERE P.Id_Viaje=@IdViaje AND P.CantidadPrevista<>R.Cantidad) THEN 'Pendiente' ELSE 'Confirmado' END;
        IF @Estado='Pendiente' AND NULLIF(LTRIM(RTRIM(@Observaciones)),'') IS NULL
            THROW 50002, 'TD.DatosInvalidos', 1;
        DECLARE @Id INT;
        SELECT @Id=Id_Despacho FROM dbo.DOCUMENTO_DESPACHO WITH (UPDLOCK,HOLDLOCK) WHERE Id_Viaje=@IdViaje;
        IF @Id IS NULL
        BEGIN
            INSERT dbo.DOCUMENTO_DESPACHO(Id_Viaje,Numero,Fecha,Estado,Observaciones) VALUES (@IdViaje,@Numero,GETDATE(),@Estado,@Observaciones);
            SET @Id=CONVERT(INT,SCOPE_IDENTITY());
        END
        ELSE
        BEGIN
            IF NOT EXISTS(SELECT 1 FROM dbo.DOCUMENTO_DESPACHO WHERE Id_Despacho=@Id AND Estado='Pendiente' AND Numero=@Numero)
                THROW 50001, 'TD.EstadoInvalido', 1;
            UPDATE dbo.DOCUMENTO_DESPACHO SET Estado=@Estado,Fecha=GETDATE(),Observaciones=@Observaciones WHERE Id_Despacho=@Id;
            DELETE dbo.DETALLE_DESPACHO WHERE Id_Despacho=@Id;
        END;
        INSERT dbo.DETALLE_DESPACHO(Id_Despacho,Id_Mercaderia,Cantidad) SELECT @Id,IdMercaderia,Cantidad FROM @Real;
        IF @Estado='Confirmado' UPDATE dbo.VIAJE SET Estado='Preparado' WHERE Id_Viaje=@IdViaje;
        COMMIT;
        SELECT @Id AS Id, @Estado AS Estado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
