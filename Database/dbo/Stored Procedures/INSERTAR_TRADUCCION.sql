CREATE PROCEDURE dbo.INSERTAR_TRADUCCION
    @Id_Idioma INT,
    @Clave_Traduccion NVARCHAR(200),
    @Valor_Traduccion NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.TRADUCCION (Id_Idioma, Clave_Traduccion, Valor_Traduccion)
    VALUES (@Id_Idioma, @Clave_Traduccion, @Valor_Traduccion);

    SELECT SCOPE_IDENTITY() AS Id_Traduccion;
END
