CREATE PROCEDURE dbo.LISTAR_IDIOMAS
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id_Idioma, Codigo_Idioma, Nombre_Idioma, EsDefault, Habilitado_Idioma
    FROM dbo.IDIOMA
    ORDER BY EsDefault DESC, Nombre_Idioma;
END
