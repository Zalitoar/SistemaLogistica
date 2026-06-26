CREATE PROCEDURE dbo.INSERTAR_IDIOMA
    @Nombre_Idioma NVARCHAR(100),
    @Codigo_Idioma NVARCHAR(10),
    @Habilitado_Idioma BIT = 1,
    @EsDefault BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.IDIOMA (Codigo_Idioma, Nombre_Idioma, EsDefault, Habilitado_Idioma)
        VALUES (@Codigo_Idioma, @Nombre_Idioma, @EsDefault, @Habilitado_Idioma);

        DECLARE @NewId INT = SCOPE_IDENTITY();

        IF @EsDefault = 1
        BEGIN
            -- Asegurar unicidad de EsDefault
            UPDATE dbo.IDIOMA
            SET EsDefault = 0
            WHERE Id_Idioma <> @NewId AND EsDefault = 1;
        END

        COMMIT TRANSACTION;

        SELECT @NewId AS Id_Idioma;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
