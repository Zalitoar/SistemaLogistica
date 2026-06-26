CREATE PROCEDURE dbo.LISTAR_IDIOMAS_POR_USUARIO
    @Id_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT i.Id_Idioma, i.Codigo_Idioma, i.Nombre_Idioma, i.EsDefault, i.Habilitado_Idioma
    FROM dbo.IDIOMA i
    INNER JOIN dbo.USUARIO_IDIOMA ui ON ui.Id_Idioma = i.Id_Idioma
    WHERE ui.Id_Usuario = @Id_Usuario
    ORDER BY i.EsDefault DESC, i.Nombre_Idioma;
END
