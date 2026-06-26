CREATE PROCEDURE dbo.OBTENER_IDIOMA_PREFERIDO_USUARIO
    @Id_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Devuelve el idioma preferido del usuario; si no existe, intenta devolver el idioma por defecto del sistema (EsDefault = 1).
    SELECT TOP 1 i.Id_Idioma, i.Codigo_Idioma, i.Nombre_Idioma, i.EsDefault, i.Habilitado_Idioma
    FROM dbo.IDIOMA i
    INNER JOIN dbo.USUARIO_IDIOMA ui ON ui.Id_Idioma = i.Id_Idioma
    WHERE ui.Id_Usuario = @Id_Usuario
    ORDER BY i.EsDefault DESC;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT TOP 1 Id_Idioma, Codigo_Idioma, Nombre_Idioma, EsDefault, Habilitado_Idioma
        FROM dbo.IDIOMA
        WHERE EsDefault = 1;
    END
END
