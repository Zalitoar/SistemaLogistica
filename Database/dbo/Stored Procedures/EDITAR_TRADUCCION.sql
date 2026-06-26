CREATE PROCEDURE dbo.EDITAR_TRADUCCION
    @Id_Traduccion INT,
    @Id_Idioma INT,
    @Clave_Traduccion NVARCHAR(200),
    @Valor_Traduccion NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TRADUCCION
    SET Id_Idioma = @Id_Idioma,
        Clave_Traduccion = @Clave_Traduccion,
        Valor_Traduccion = @Valor_Traduccion
    WHERE Id_Traduccion = @Id_Traduccion;
END
