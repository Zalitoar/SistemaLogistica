CREATE PROCEDURE dbo.BORRAR_IDIOMA
    @Id_Idioma INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Eliminar traducciones y relaciones de usuario primero
        DELETE FROM dbo.TRADUCCION WHERE Id_Idioma = @Id_Idioma;
        DELETE FROM dbo.USUARIO_IDIOMA WHERE Id_Idioma = @Id_Idioma;
        DELETE FROM dbo.IDIOMA WHERE Id_Idioma = @Id_Idioma;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
