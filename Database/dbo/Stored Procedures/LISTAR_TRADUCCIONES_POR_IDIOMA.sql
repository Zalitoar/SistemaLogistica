CREATE PROCEDURE dbo.LISTAR_TRADUCCIONES_POR_IDIOMA
    @Id_Idioma INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id_Traduccion, Id_Idioma, Clave_Traduccion, Valor_Traduccion
    FROM dbo.TRADUCCION
    WHERE Id_Idioma = @Id_Idioma
    ORDER BY Clave_Traduccion;
END
