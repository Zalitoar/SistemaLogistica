CREATE PROCEDURE dbo.MF_REGISTRAR_ESTADO @IdUnidadFlota INT,@Kilometraje DECIMAL(18,3),@TipoNovedad NVARCHAR(30),@Descripcion NVARCHAR(500),@RequiereMantenimiento BIT AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @BloqueoFlota INT;
        SELECT @BloqueoFlota=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);

IF NOT EXISTS(SELECT 1 FROM dbo.UNIDAD_FLOTA WHERE IdUnidadFlota=@IdUnidadFlota AND Activa=1 AND KilometrajeActual<=@Kilometraje) THROW 50102,'MF.Kilometraje',1;
IF EXISTS(SELECT 1 FROM dbo.VIAJE WHERE IdUnidadFlota=@IdUnidadFlota AND Estado='EnCurso') THROW 50103,'MF.EnViaje',1;
IF @RequiereMantenimiento=0 AND EXISTS(SELECT 1 FROM dbo.PARTE_ESTADO_UNIDAD WHERE IdUnidadFlota=@IdUnidadFlota AND EstadoParte<>'Resuelto') THROW 50104,'MF.Pendientes',1;
INSERT dbo.PARTE_ESTADO_UNIDAD(IdUnidadFlota,FechaHora,Kilometraje,TipoNovedad,Descripcion,RequiereMantenimiento,EstadoParte)
VALUES(@IdUnidadFlota,GETDATE(),@Kilometraje,@TipoNovedad,@Descripcion,@RequiereMantenimiento,CASE WHEN @RequiereMantenimiento=1 THEN 'Pendiente' ELSE 'Resuelto' END);
DECLARE @Id INT=CAST(SCOPE_IDENTITY() AS INT);
UPDATE dbo.INFORME_DISPONIBILIDAD SET Vigente=0 WHERE IdUnidadFlota=@IdUnidadFlota AND Vigente=1;
UPDATE dbo.UNIDAD_FLOTA SET KilometrajeActual=@Kilometraje,EstadoOperativo=CASE WHEN @RequiereMantenimiento=1 THEN 'FueraServicio' ELSE 'Disponible' END WHERE IdUnidadFlota=@IdUnidadFlota;
INSERT dbo.INFORME_DISPONIBILIDAD(IdUnidadFlota,FechaHora,Disponible,EstadoUnidad,Observaciones,Vigente)
VALUES(@IdUnidadFlota,GETDATE(),1-@RequiereMantenimiento,CASE WHEN @RequiereMantenimiento=1 THEN 'FueraServicio' ELSE 'Disponible' END,@Descripcion,1);
SELECT @Id AS Id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH;
END;
