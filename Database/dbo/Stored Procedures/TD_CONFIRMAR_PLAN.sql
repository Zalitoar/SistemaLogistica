CREATE PROCEDURE dbo.TD_CONFIRMAR_PLAN
    @IdRequerimiento INT, @Numero NVARCHAR(50), @Viajes XML
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM dbo.REQUERIMIENTO_DISTRIBUCION WITH (UPDLOCK,HOLDLOCK) WHERE Id_Requerimiento=@IdRequerimiento AND Estado='Pendiente')
            THROW 50001, 'TD.EstadoInvalido', 1;
        IF NULLIF(LTRIM(RTRIM(@Numero)), '') IS NULL OR @Viajes IS NULL OR @Viajes.exist('/viajes/viaje')=0
            THROW 50002, 'TD.DatosInvalidos', 1;

        DECLARE @Datos TABLE (Numero NVARCHAR(50) PRIMARY KEY, Fecha DATETIME, Datos XML);
        INSERT @Datos SELECT V.value('@numero','nvarchar(50)'), V.value('@fecha','datetime'), V.query('.')
        FROM @Viajes.nodes('/viajes/viaje') AS N(V);
        IF EXISTS (SELECT 1 FROM @Datos WHERE NULLIF(LTRIM(RTRIM(Numero)),'') IS NULL OR Fecha IS NULL
            OR Datos.exist('/viaje/entregas/entrega')=0 OR Datos.exist('/viaje/carga/item')=0)
            THROW 50002, 'TD.DatosInvalidos', 1;

        IF EXISTS (SELECT 1 FROM @Datos D CROSS APPLY D.Datos.nodes('/viaje/entregas/entrega') N(E)
            WHERE NULLIF(LTRIM(RTRIM(E.value('@destino','nvarchar(200)'))),'') IS NULL OR E.value('@fecha','datetime') < D.Fecha)
            THROW 50002, 'TD.DatosInvalidos', 1;
        IF EXISTS (SELECT 1 FROM @Datos D CROSS APPLY D.Datos.nodes('/viaje/carga/item') N(C)
            WHERE C.value('@cantidad','decimal(18,3)') <= 0 OR NOT EXISTS
            (SELECT 1 FROM dbo.MERCADERIA M WHERE M.Id_Mercaderia=C.value('@mercaderia','int')))
            THROW 50002, 'TD.DatosInvalidos', 1;

        INSERT dbo.PLAN_DISTRIBUCION(Id_Requerimiento,Numero,Fecha,Estado) VALUES (@IdRequerimiento,@Numero,GETDATE(),'Confirmado');
        DECLARE @IdPlan INT=CONVERT(INT,SCOPE_IDENTITY());
        DECLARE @Nuevos TABLE (Id INT, Numero NVARCHAR(50));
        INSERT dbo.VIAJE(Id_Plan,Numero,FechaPrevista,Estado)
        OUTPUT inserted.Id_Viaje, inserted.Numero INTO @Nuevos
        SELECT @IdPlan,Numero,Fecha,'Planificado' FROM @Datos;

        INSERT dbo.ENTREGA(Id_Viaje,Destino,FechaPrevista,Estado)
        SELECT V.Id,E.value('@destino','nvarchar(200)'),E.value('@fecha','datetime'),'Pendiente'
        FROM @Datos D JOIN @Nuevos V ON V.Numero=D.Numero CROSS APPLY D.Datos.nodes('/viaje/entregas/entrega') N(E);
        INSERT dbo.DETALLE_CARGA_PLANIFICADA(Id_Viaje,Id_Mercaderia,CantidadPrevista)
        SELECT V.Id,C.value('@mercaderia','int'),C.value('@cantidad','decimal(18,3)')
        FROM @Datos D JOIN @Nuevos V ON V.Numero=D.Numero CROSS APPLY D.Datos.nodes('/viaje/carga/item') N(C);
        UPDATE dbo.REQUERIMIENTO_DISTRIBUCION SET Estado='Planificado' WHERE Id_Requerimiento=@IdRequerimiento;
        COMMIT;
        SELECT @IdPlan AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END;
