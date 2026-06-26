CREATE PROCEDURE dbo.EDITAR_IDIOMA
    @Id_Idioma INT,
    @Nombre_Idioma NVARCHAR(100),
    @Codigo_Idioma NVARCHAR(10),
    @Habilitado_Idioma BIT = 1,
    @EsDefault BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.IDIOMA
        SET Nombre_Idioma = @Nombre_Idioma,
            Codigo_Idioma = @Codigo_Idioma,
            Habilitado_Idioma = @Habilitado_Idioma,
            EsDefault = @EsDefault
        WHERE Id_Idioma = @Id_Idioma;

        IF @EsDefault = 1
        BEGIN
            UPDATE dbo.IDIOMA
            SET EsDefault = 0
            WHERE Id_Idioma <> @Id_Idioma AND EsDefault = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
