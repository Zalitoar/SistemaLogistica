CREATE PROCEDURE dbo.BORRAR_USUARIO_IDIOMA
    @Id_Usuario INT,
    @Id_Idioma INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.USUARIO_IDIOMA
    WHERE Id_Usuario = @Id_Usuario AND Id_Idioma = @Id_Idioma;
END
