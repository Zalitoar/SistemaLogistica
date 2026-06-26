CREATE PROCEDURE dbo.INSERTAR_USUARIO_IDIOMA
    @Id_Usuario INT,
    @Id_Idioma INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS(SELECT 1 FROM dbo.USUARIO_IDIOMA WHERE Id_Usuario = @Id_Usuario AND Id_Idioma = @Id_Idioma)
    BEGIN
        INSERT INTO dbo.USUARIO_IDIOMA (Id_Usuario, Id_Idioma)
        VALUES (@Id_Usuario, @Id_Idioma);
    END
END
