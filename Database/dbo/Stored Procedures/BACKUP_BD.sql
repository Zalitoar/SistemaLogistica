CREATE PROC [dbo].[BACKUP_BD]
    @Ruta varchar(260)
AS
BEGIN
    BACKUP DATABASE IngSoftDB TO DISK = @Ruta WITH FORMAT, INIT, NAME = 'Backup IngSoftDB.bak'
END
