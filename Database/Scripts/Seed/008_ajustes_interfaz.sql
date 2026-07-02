/*
Ajustes puntuales de textos de interfaz ya sembrados.

Sólo reemplaza los valores originales conocidos para no pisar traducciones
personalizadas por un administrador.
*/

UPDATE traduccion
SET Valor_Traduccion = CASE idioma.Codigo_Idioma
    WHEN N'es-AR' THEN N'Restaurar base de datos'
    WHEN N'en-US' THEN N'Restore database'
    WHEN N'pt-BR' THEN N'Restaurar banco de dados'
END
FROM dbo.TRADUCCION AS traduccion
INNER JOIN dbo.IDIOMA AS idioma
    ON idioma.Id_Idioma = traduccion.Id_Idioma
WHERE traduccion.Clave_Traduccion = N'frmRestore.btnRestore.Text'
  AND (
      (idioma.Codigo_Idioma = N'es-AR' AND traduccion.Valor_Traduccion = N'Backup base de datos')
      OR (idioma.Codigo_Idioma = N'en-US' AND traduccion.Valor_Traduccion = N'Database backup')
      OR (idioma.Codigo_Idioma = N'pt-BR' AND traduccion.Valor_Traduccion = N'Backup do banco de dados')
  );
