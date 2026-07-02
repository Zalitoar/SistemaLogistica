CREATE PROC [dbo].[BACKUP_BD]
    @Ruta varchar(260),
    @BaseDatos sysname
AS
BEGIN
    SET NOCOUNT ON;

    IF DB_ID(@BaseDatos) IS NULL
        THROW 50001, 'La base de datos indicada no existe.', 1;

    DECLARE @BaseDatosEscapada nvarchar(258) = QUOTENAME(@BaseDatos);
    DECLARE @RutaEscapada nvarchar(520) = REPLACE(CONVERT(nvarchar(260), @Ruta), N'''', N'''''' );
    DECLARE @Sql nvarchar(max);

    BEGIN TRY
        SET @Sql = N'ALTER DATABASE ' + @BaseDatosEscapada
                 + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE;';
        EXEC (@Sql);

        SET @Sql = N'RESTORE DATABASE ' + @BaseDatosEscapada
                 + N' FROM DISK = N''' + @RutaEscapada + N''' WITH REPLACE;';
        EXEC (@Sql);

        SET @Sql = N'ALTER DATABASE ' + @BaseDatosEscapada
                 + N' SET MULTI_USER WITH ROLLBACK IMMEDIATE;';
        EXEC (@Sql);
    END TRY
    BEGIN CATCH
        BEGIN TRY
            SET @Sql = N'ALTER DATABASE ' + @BaseDatosEscapada
                     + N' SET MULTI_USER WITH ROLLBACK IMMEDIATE;';
            EXEC (@Sql);
        END TRY
        BEGIN CATCH
            -- Conservar la excepción original de la restauración.
        END CATCH;

        THROW;
    END CATCH;
END
