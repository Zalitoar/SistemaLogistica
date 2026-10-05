-- Exclusivamente bajo petición del asistente. No incluir en PostDeployment.sql.
-- Parámetro @ClaveDemo: hash SHA256 de la contraseña aleatoria presentada al usuario.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @Lock INT, @N INT;
 EXEC @Lock=sys.sp_getapplock @Resource=N'SistemaLogistica.DemoTD',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000;
 IF @Lock<0 THROW 50010,'BD.DemoError',1;
 SELECT @N=COUNT(*) FROM dbo.USUARIO WITH(TABLOCKX,HOLDLOCK);
 SELECT @N=COUNT(*) FROM dbo.BITACORA WITH(TABLOCKX,HOLDLOCK);
 IF EXISTS(SELECT 1 FROM dbo.BITACORA WHERE Actividad_Bitacora='DEMO TD v1 cargado')
 BEGIN
  COMMIT; SELECT CAST(0 AS INT); RETURN;
 END;
 -- No apropiarse de registros existentes ni restablecer sus credenciales.
 IF EXISTS(SELECT 1 FROM dbo.USUARIO WHERE Nombre_Usuario IN ('demo_planificador','demo_expedicion','demo_transporte','demo_consulta'))
 OR EXISTS(SELECT 1 FROM dbo.PERMISO WHERE Nombre_Permiso LIKE 'DEMO TD %')
 OR EXISTS(SELECT 1 FROM dbo.REQUERIMIENTO_DISTRIBUCION WHERE Numero LIKE 'DEMO-TD-%')
 OR EXISTS(SELECT 1 FROM dbo.PLAN_DISTRIBUCION WHERE Numero LIKE 'DEMO-TD-%')
 OR EXISTS(SELECT 1 FROM dbo.VIAJE WHERE Numero LIKE 'DEMO-TD-%')
 OR EXISTS(SELECT 1 FROM dbo.DOCUMENTO_DESPACHO WHERE Numero LIKE 'DEMO-TD-%')
 OR EXISTS(SELECT 1 FROM dbo.MERCADERIA WHERE Codigo LIKE 'DEMO-TD-%')
 THROW 50010,'BD.DemoColision',1;
 -- Verificar antes de recalcular DVV para no encubrir corrupción previa.
 DECLARE @Texto VARCHAR(MAX), @Dvv VARCHAR(64);
 SELECT @Texto=(SELECT CONVERT(VARCHAR(20),Id_Usuario)+Nombre_Usuario+Clave_Usuario+CONVERT(VARCHAR(20),Id_Rol) FROM dbo.USUARIO ORDER BY Id_Usuario FOR XML PATH(''),TYPE).value('.','VARCHAR(MAX)');
 SET @Dvv=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),CONVERT(VARCHAR(MAX),CONVERT(NVARCHAR(MAX),ISNULL(@Texto,'')) COLLATE Latin1_General_100_BIN2_UTF8))),2));
 IF NOT EXISTS(SELECT 1 FROM dbo.DVV WHERE Tabla_DVV='Usuario' AND Valor_DVV=@Dvv)
 OR (SELECT COUNT(*) FROM dbo.DVV WHERE Tabla_DVV='Usuario')<>1
 OR EXISTS(SELECT 1 FROM dbo.USUARIO WHERE Borrado_Usuario IS NULL OR dvh_Usuario IS NULL OR dvh_Usuario<>LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),CONVERT(VARCHAR(MAX),CONVERT(NVARCHAR(MAX),Nombre_Usuario+'|'+Clave_Usuario+'|'+CONVERT(VARCHAR(20),Id_Rol)+'|'+CONVERT(VARCHAR(20),Borrado_Usuario)) COLLATE Latin1_General_100_BIN2_UTF8))),2)))
 THROW 50010,'BD.DemoIntegridad',1;
 IF @ClaveDemo IS NULL OR LEN(@ClaveDemo)<>64 THROW 50010,'BD.DemoError',1;

 IF (SELECT COUNT(DISTINCT Nombre_Permiso) FROM dbo.PERMISO WHERE Tipo_Permiso='PERMISO' AND Nombre_Permiso IN ('TD_PLANIFICAR_DISTRIBUCION','TD_PREPARAR_DESPACHO','TD_EJECUTAR_VIAJE','TD_REGISTRAR_ENTREGA','TD_CONTROLAR_CUMPLIMIENTO','VER_BITACORA'))<>6 THROW 50010,'BD.DemoError',1;
 DECLARE @Roles TABLE(Numero INT,Nombre VARCHAR(100),Usuario VARCHAR(50),IdRol INT);
 INSERT @Roles(Numero,Nombre,Usuario) VALUES
 (1,'DEMO TD Planificacion','demo_planificador'),(2,'DEMO TD Expedicion','demo_expedicion'),
 (3,'DEMO TD Transporte','demo_transporte'),(4,'DEMO TD Consulta','demo_consulta');
 INSERT dbo.PERMISO(Nombre_Permiso,Tipo_Permiso) SELECT Nombre,'ROL' FROM @Roles;
 UPDATE R SET IdRol=P.Id_Permiso FROM @Roles R JOIN dbo.PERMISO P ON P.Nombre_Permiso=R.Nombre;
 INSERT dbo.ROL_COMPONENTE(Id_Rol,Id_Componente)
 SELECT R.IdRol,P.Id_Permiso FROM @Roles R JOIN dbo.PERMISO P ON
 (R.Numero=1 AND P.Nombre_Permiso IN ('TD_PLANIFICAR_DISTRIBUCION','TD_CONTROLAR_CUMPLIMIENTO')) OR
 (R.Numero=2 AND P.Nombre_Permiso='TD_PREPARAR_DESPACHO') OR
 (R.Numero=3 AND P.Nombre_Permiso IN ('TD_EJECUTAR_VIAJE','TD_REGISTRAR_ENTREGA')) OR
 (R.Numero=4 AND P.Nombre_Permiso='VER_BITACORA');
 DECLARE @IdUsuario INT=(SELECT ISNULL(MAX(Id_Usuario),0) FROM dbo.USUARIO);
 INSERT dbo.USUARIO(Id_Usuario,Nombre_Usuario,Clave_Usuario,Borrado_Usuario,dvh_Usuario,Id_Rol)
 SELECT @IdUsuario+Numero,Usuario,@ClaveDemo,0,
 LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),Usuario+'|'+@ClaveDemo+'|'+CONVERT(VARCHAR(20),IdRol)+'|0')),2)),IdRol FROM @Roles;
 INSERT dbo.USUARIO_IDIOMA(Id_Usuario,Id_Idioma)
 SELECT @IdUsuario+Numero,(SELECT TOP(1) Id_Idioma FROM dbo.IDIOMA WHERE Codigo_Idioma='es-AR') FROM @Roles;
 SELECT @Texto=(SELECT CONVERT(VARCHAR(20),Id_Usuario)+Nombre_Usuario+Clave_Usuario+CONVERT(VARCHAR(20),Id_Rol) FROM dbo.USUARIO ORDER BY Id_Usuario FOR XML PATH(''),TYPE).value('.','VARCHAR(MAX)');
 UPDATE dbo.DVV SET Valor_DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),CONVERT(VARCHAR(MAX),CONVERT(NVARCHAR(MAX),@Texto) COLLATE Latin1_General_100_BIN2_UTF8))),2)) WHERE Tabla_DVV='Usuario';

 INSERT dbo.MERCADERIA(Codigo,Descripcion) VALUES
 ('DEMO-TD-CAJA',N'Cajas de alimentos de prueba'),('DEMO-TD-BULTO',N'Bultos de limpieza de prueba'),('DEMO-TD-PALLET',N'Pallets de bebidas de prueba');
 DECLARE @Ahora DATETIME=GETDATE(),@i INT=1,@j INT,@Req INT,@Plan INT,@Viaje INT,@Despacho INT,@Entrega INT,@Estado VARCHAR(20),@EstadoEntrega VARCHAR(20),@Inicio DATETIME;
 INSERT dbo.REQUERIMIENTO_DISTRIBUCION(Numero,Fecha,Origen,Estado) VALUES('DEMO-TD-REQ-PENDIENTE',@Ahora,N'Centro de distribución de prueba','Pendiente');
 WHILE @i<=7
 BEGIN
  SET @Estado=CASE WHEN @i<=2 THEN 'Planificado' WHEN @i=3 THEN 'Preparado' WHEN @i<=6 THEN 'EnCurso' ELSE 'Cerrado' END;
  SET @Inicio=CASE WHEN @i>=4 THEN DATEADD(HOUR,-4,@Ahora) END;
  INSERT dbo.REQUERIMIENTO_DISTRIBUCION(Numero,Fecha,Origen,Estado) VALUES(CONCAT('DEMO-TD-REQ-',@i),DATEADD(DAY,-1,@Ahora),N'Centro de distribución de prueba','Planificado');
  SET @Req=SCOPE_IDENTITY();
  INSERT dbo.PLAN_DISTRIBUCION(Id_Requerimiento,Numero,Fecha,Estado) VALUES(@Req,CONCAT('DEMO-TD-PLAN-',@i),DATEADD(DAY,-1,@Ahora),'Confirmado'); SET @Plan=SCOPE_IDENTITY();
  INSERT dbo.VIAJE(Id_Plan,Numero,FechaPrevista,FechaInicio,FechaFinalizacion,Estado,PorcentajeCumplimiento)
  VALUES(@Plan,CONCAT('DEMO-TD-VIAJE-',@i),DATEADD(HOUR,-5,@Ahora),@Inicio,CASE WHEN @i=7 THEN DATEADD(MINUTE,-30,@Ahora) END,@Estado,CASE WHEN @i=5 THEN 33.33 WHEN @i>=6 THEN 100 ELSE 0 END);
  SET @Viaje=SCOPE_IDENTITY();
  INSERT dbo.DETALLE_CARGA_PLANIFICADA(Id_Viaje,Id_Mercaderia,CantidadPrevista)
  SELECT @Viaje,Id_Mercaderia,CASE Codigo WHEN 'DEMO-TD-CAJA' THEN 10 WHEN 'DEMO-TD-BULTO' THEN 5 ELSE 2 END FROM dbo.MERCADERIA WHERE Codigo LIKE 'DEMO-TD-%';
  IF @i>=2
  BEGIN
   INSERT dbo.DOCUMENTO_DESPACHO(Id_Viaje,Numero,Fecha,Estado,Observaciones)
   VALUES(@Viaje,CONCAT('DEMO-TD-DESPACHO-',@i),DATEADD(HOUR,-5,@Ahora),CASE WHEN @i=2 THEN 'Pendiente' ELSE 'Confirmado' END,CASE WHEN @i=2 THEN N'DEMO: faltan 2 cajas; corregir antes de iniciar.' ELSE N'DEMO: carga verificada.' END);
   SET @Despacho=SCOPE_IDENTITY();
   INSERT dbo.DETALLE_DESPACHO(Id_Despacho,Id_Mercaderia,Cantidad)
   SELECT @Despacho,C.Id_Mercaderia,C.CantidadPrevista-CASE WHEN @i=2 AND M.Codigo='DEMO-TD-CAJA' THEN 2 ELSE 0 END FROM dbo.DETALLE_CARGA_PLANIFICADA C JOIN dbo.MERCADERIA M ON M.Id_Mercaderia=C.Id_Mercaderia WHERE C.Id_Viaje=@Viaje;
  END;
  SET @j=1;
  WHILE @j<=3
  BEGIN
   SET @EstadoEntrega=CASE WHEN @i<5 THEN 'Pendiente' WHEN @i>=6 OR @j=1 THEN 'Entregada' WHEN @j=2 THEN 'Parcial' ELSE 'Incumplida' END;
   INSERT dbo.ENTREGA(Id_Viaje,Destino,FechaPrevista,FechaRealizada,Estado,Observaciones)
   VALUES(@Viaje,CASE @j WHEN 1 THEN N'DEMO Sucursal Centro' WHEN 2 THEN N'DEMO Cliente Norte' ELSE N'DEMO Depósito Sur' END,
    DATEADD(HOUR,-4+@j,@Ahora),CASE WHEN @EstadoEntrega<>'Pendiente' THEN DATEADD(HOUR,-4+@j,@Ahora) END,@EstadoEntrega,
    CASE @EstadoEntrega WHEN 'Parcial' THEN N'DEMO: recepción parcial por falta de espacio.' WHEN 'Incumplida' THEN N'DEMO: destinatario ausente.' WHEN 'Entregada' THEN N'DEMO: entrega completa.' END);
   SET @Entrega=SCOPE_IDENTITY();
   IF @EstadoEntrega='Entregada'
    INSERT dbo.COMPROBANTE_ENTREGA(Id_Entrega,Numero,Fecha,Resultado,Observaciones) VALUES(@Entrega,CONCAT('CE-',@Entrega),DATEADD(HOUR,-4+@j,@Ahora),'Entregada',N'DEMO: comprobante de entrega completa.');
   SET @j+=1;
  END;
  DECLARE @IdBit INT=(SELECT ISNULL(MAX(Id_Bitacora),0) FROM dbo.BITACORA);
  INSERT dbo.BITACORA VALUES(@IdBit+1,'demo_planificador',DATEADD(DAY,-1,@Ahora),CONCAT('DEMO plan confirmado V',@i));
  IF @i>=2 INSERT dbo.BITACORA VALUES(@IdBit+2,'demo_expedicion',DATEADD(HOUR,-5,@Ahora),CONCAT('DEMO despacho preparado V',@i));
  IF @i>=4 INSERT dbo.BITACORA VALUES(@IdBit+3,'demo_transporte',@Inicio,CONCAT('DEMO inicio de viaje V',@i));
  IF @i>=5 INSERT dbo.BITACORA VALUES(@IdBit+4,'demo_transporte',DATEADD(HOUR,-1,@Ahora),CONCAT('DEMO resultados de entrega V',@i));
  IF @i=7 INSERT dbo.BITACORA VALUES(@IdBit+5,'demo_planificador',DATEADD(MINUTE,-30,@Ahora),'DEMO cierre de viaje V7');
  SET @i+=1;
 END;
 INSERT dbo.BITACORA SELECT ISNULL(MAX(Id_Bitacora),0)+1,'Sistema',@Ahora,'DEMO TD v1 cargado' FROM dbo.BITACORA;
 COMMIT;
 SELECT CAST(1 AS INT);
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
