CREATE PROCEDURE [dbo].[ACTUALIZAR_DVVUSUARIO]
      @Valor_DVV VARCHAR(64)
  AS
      IF EXISTS (SELECT 1 FROM DVV WHERE Tabla_DVV = 'Usuario')
          UPDATE DVV SET Valor_DVV = @Valor_DVV WHERE Tabla_DVV = 'Usuario'
      ELSE
          INSERT INTO DVV (Tabla_DVV, Valor_DVV) VALUES ('Usuario', @Valor_DVV)
