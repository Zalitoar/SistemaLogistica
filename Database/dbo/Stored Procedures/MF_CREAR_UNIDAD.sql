CREATE PROCEDURE dbo.MF_CREAR_UNIDAD @Dominio NVARCHAR(10),@Tipo NVARCHAR(50),@Marca NVARCHAR(50),@Modelo NVARCHAR(50),@Anio INT,@Capacidad DECIMAL(18,3),@KilometrajeActual DECIMAL(18,3) AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);
INSERT dbo.UNIDAD_FLOTA(Dominio,Tipo,Marca,Modelo,Anio,Capacidad,KilometrajeActual,EstadoOperativo,Activa)
VALUES(UPPER(LTRIM(RTRIM(@Dominio))),@Tipo,@Marca,@Modelo,@Anio,@Capacidad,@KilometrajeActual,'FueraServicio',1);
SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
