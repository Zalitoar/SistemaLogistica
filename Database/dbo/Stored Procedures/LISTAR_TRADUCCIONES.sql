CREATE PROCEDURE dbo.LISTAR_TRADUCCIONES
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.Id_Traduccion, t.Id_Idioma, t.Clave_Traduccion, t.Valor_Traduccion
    FROM dbo.TRADUCCION t
    ORDER BY t.Id_Idioma, t.Clave_Traduccion;
END
