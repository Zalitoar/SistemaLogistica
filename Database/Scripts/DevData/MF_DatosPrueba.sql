-- Datos ficticios MF, sólo a pedido del asistente. Nunca ejecutar desde PostDeployment.
-- @ClaveDemo contiene exclusivamente el hash de la contraseña generada por Servicios.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @Lock INT,@N INT;
 EXEC @Lock=sys.sp_getapplock @Resource=N'SistemaLogistica.DemoMF',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000;
 IF @Lock<0 THROW 50010,'BD.DemoError',1;
 SELECT @N=COUNT(*) FROM dbo.USUARIO WITH(TABLOCKX,HOLDLOCK);
 SELECT @N=COUNT(*) FROM dbo.BITACORA WITH(TABLOCKX,HOLDLOCK);
 SELECT @N=COUNT(*) FROM dbo.UNIDAD_FLOTA WITH(TABLOCKX,HOLDLOCK);
 IF EXISTS(SELECT 1 FROM dbo.BITACORA WHERE Actividad_Bitacora='DEMO MF v1 cargado')
 BEGIN
  COMMIT; SELECT CAST(0 AS INT); RETURN;
 END;
 IF EXISTS(SELECT 1 FROM dbo.USUARIO WHERE Nombre_Usuario IN ('demo_flota','demo_mantenimiento','demo_trafico_mf'))
 OR EXISTS(SELECT 1 FROM dbo.PERMISO WHERE Nombre_Permiso LIKE 'DEMO MF %')
 OR EXISTS(SELECT 1 FROM dbo.UNIDAD_FLOTA WHERE Dominio LIKE 'MFDEMO%')
 OR EXISTS(SELECT 1 FROM dbo.REQUERIMIENTO_DISTRIBUCION WHERE Numero LIKE 'DEMO-MF-%')
 OR EXISTS(SELECT 1 FROM dbo.PLAN_DISTRIBUCION WHERE Numero LIKE 'DEMO-MF-%')
 OR EXISTS(SELECT 1 FROM dbo.VIAJE WHERE Numero LIKE 'DEMO-MF-%')
 OR EXISTS(SELECT 1 FROM dbo.DOCUMENTO_DESPACHO WHERE Numero LIKE 'DEMO-MF-%')
 OR EXISTS(SELECT 1 FROM dbo.MERCADERIA WHERE Codigo LIKE 'DEMO-MF-%')
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


 IF (SELECT COUNT(DISTINCT Nombre_Permiso) FROM dbo.PERMISO WHERE Tipo_Permiso='PERMISO' AND Nombre_Permiso IN ('MF_REGISTRAR_ESTADO','MF_PROGRAMAR_MANTENIMIENTO','MF_REGISTRAR_INTERVENCION','MF_HABILITAR_UNIDAD','MF_ASIGNAR_UNIDAD'))<>5 THROW 50010,'BD.DemoError',1;
 DECLARE @Roles TABLE(Numero INT,Nombre VARCHAR(100),Usuario VARCHAR(50),IdRol INT);
 INSERT @Roles(Numero,Nombre,Usuario) VALUES
 (1,'DEMO MF Flota','demo_flota'),(2,'DEMO MF Mantenimiento','demo_mantenimiento'),(3,'DEMO MF Trafico','demo_trafico_mf');
 INSERT dbo.PERMISO(Nombre_Permiso,Tipo_Permiso) SELECT Nombre,'ROL' FROM @Roles;
 UPDATE R SET IdRol=P.Id_Permiso FROM @Roles R JOIN dbo.PERMISO P ON P.Nombre_Permiso=R.Nombre;
 INSERT dbo.ROL_COMPONENTE(Id_Rol,Id_Componente)
 SELECT R.IdRol,P.Id_Permiso FROM @Roles R JOIN dbo.PERMISO P ON
 (R.Numero=1 AND P.Nombre_Permiso='MF_REGISTRAR_ESTADO') OR
 (R.Numero=2 AND P.Nombre_Permiso IN ('MF_PROGRAMAR_MANTENIMIENTO','MF_REGISTRAR_INTERVENCION','MF_HABILITAR_UNIDAD')) OR
 (R.Numero=3 AND P.Nombre_Permiso='MF_ASIGNAR_UNIDAD');
 DECLARE @IdUsuario INT=(SELECT ISNULL(MAX(Id_Usuario),0) FROM dbo.USUARIO);
 INSERT dbo.USUARIO(Id_Usuario,Nombre_Usuario,Clave_Usuario,Borrado_Usuario,dvh_Usuario,Id_Rol)
 SELECT @IdUsuario+Numero,Usuario,@ClaveDemo,0,
 LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),Usuario+'|'+@ClaveDemo+'|'+CONVERT(VARCHAR(20),IdRol)+'|0')),2)),IdRol FROM @Roles;
 INSERT dbo.USUARIO_IDIOMA(Id_Usuario,Id_Idioma)
 SELECT @IdUsuario+Numero,(SELECT TOP(1) Id_Idioma FROM dbo.IDIOMA WHERE Codigo_Idioma='es-AR') FROM @Roles;
 SELECT @Texto=(SELECT CONVERT(VARCHAR(20),Id_Usuario)+Nombre_Usuario+Clave_Usuario+CONVERT(VARCHAR(20),Id_Rol) FROM dbo.USUARIO ORDER BY Id_Usuario FOR XML PATH(''),TYPE).value('.','VARCHAR(MAX)');
 UPDATE dbo.DVV SET Valor_DVV=LOWER(CONVERT(VARCHAR(64),HASHBYTES('SHA2_256',CONVERT(VARBINARY(MAX),CONVERT(VARCHAR(MAX),CONVERT(NVARCHAR(MAX),@Texto) COLLATE Latin1_General_100_BIN2_UTF8))),2)) WHERE Tabla_DVV='Usuario';

 DECLARE @Ahora DATETIME=GETDATE(),@i INT=1,@Unidad INT,@Parte INT,@Orden INT,@Intervencion INT,@Informe INT,@UnidadAsignada INT,@InformeAsignado INT,@Estado VARCHAR(30),@EstadoOrden VARCHAR(30);
 WHILE @i<=7
 BEGIN
  SET @Estado=CASE WHEN @i IN (1,7) THEN 'Disponible' WHEN @i=2 THEN 'FueraServicio' WHEN @i=6 THEN 'PendienteVerificacion' ELSE 'EnMantenimiento' END;
  INSERT dbo.UNIDAD_FLOTA(Dominio,Tipo,Marca,Modelo,Anio,Capacidad,KilometrajeActual,EstadoOperativo,Activa)
  VALUES(CONCAT('MFDEMO0',@i),N'Camión',N'Marca de prueba',CONCAT('Modelo demo ',@i),2024,12000,10000+@i*100,@Estado,1);
  SET @Unidad=SCOPE_IDENTITY();
  INSERT dbo.PARTE_ESTADO_UNIDAD(IdUnidadFlota,FechaHora,Kilometraje,TipoNovedad,Descripcion,RequiereMantenimiento,EstadoParte)
  VALUES(@Unidad,DATEADD(DAY,-3,@Ahora),10000+@i*100,CASE WHEN @i=1 THEN N'Control rutinario' ELSE N'Revisión de frenos' END,
   CONCAT(N'DEMO MF escenario ',@i, N': control de la unidad.'),CASE WHEN @i=1 THEN 0 ELSE 1 END,
   CASE WHEN @i IN (1,7) THEN 'Resuelto' WHEN @i=2 THEN 'Pendiente' ELSE 'Programado' END);
  SET @Parte=SCOPE_IDENTITY();
  SET @Intervencion=NULL;
  IF @i>=3
  BEGIN
   SET @EstadoOrden=CASE WHEN @i IN (3,5) THEN 'Programada' WHEN @i=4 THEN 'EnCurso' WHEN @i=6 THEN 'PendienteVerificacion' ELSE 'Cerrada' END;
   INSERT dbo.ORDEN_MANTENIMIENTO(IdUnidadFlota,IdParteEstadoUnidad,FechaEmision,FechaProgramada,TipoMantenimiento,Prioridad,DescripcionTrabajo,EstadoOrden)
   VALUES(@Unidad,@Parte,DATEADD(DAY,-2,@Ahora),CASE WHEN @i=3 THEN DATEADD(DAY,1,@Ahora) ELSE DATEADD(DAY,-1,@Ahora) END,'Correctivo','Alta',N'DEMO: revisar frenos y verificar funcionamiento.',@EstadoOrden);
   SET @Orden=SCOPE_IDENTITY();
   IF @i>=4
   BEGIN
    INSERT dbo.INTERVENCION_MANTENIMIENTO(IdOrdenMantenimiento,FechaInicio,FechaFin,Kilometraje,TrabajoRealizado,Resultado,Observaciones)
    VALUES(@Orden,DATEADD(HOUR,-8,@Ahora),CASE WHEN @i<>4 THEN DATEADD(HOUR,-6,@Ahora) END,10000+@i*100,
     CASE WHEN @i=4 THEN N'' ELSE N'DEMO: inspección y ajuste de frenos.' END,
     CASE WHEN @i=4 THEN 'EnCurso' WHEN @i=5 THEN 'RequiereTareas' ELSE 'Satisfactoria' END,
     CASE WHEN @i=5 THEN N'DEMO: reemplazar componente y repetir verificación.' ELSE N'DEMO: registro de mantenimiento.' END);
    SET @Intervencion=SCOPE_IDENTITY();
   END;
  END;
  -- Se conserva el informe no favorable anterior a la programación.
  INSERT dbo.INFORME_DISPONIBILIDAD(IdUnidadFlota,FechaHora,Disponible,EstadoUnidad,Observaciones,Vigente)
  VALUES(@Unidad,DATEADD(DAY,-3,@Ahora),CASE WHEN @i=1 THEN 1 ELSE 0 END,CASE WHEN @i=1 THEN 'Disponible' ELSE 'FueraServicio' END,
   N'DEMO: resultado del control inicial.',CASE WHEN @i IN (1,2) THEN 1 ELSE 0 END);
  IF @i=7
  BEGIN
   INSERT dbo.INFORME_DISPONIBILIDAD(IdUnidadFlota,IdIntervencionMantenimiento,FechaHora,Disponible,EstadoUnidad,Observaciones,Vigente)
   VALUES(@Unidad,@Intervencion,DATEADD(HOUR,-5,@Ahora),1,'Disponible',N'DEMO: mantenimiento verificado; unidad habilitada.',1);
   SET @InformeAsignado=SCOPE_IDENTITY(); SET @UnidadAsignada=@Unidad;
  END;
  DECLARE @Bit INT=(SELECT ISNULL(MAX(Id_Bitacora),0) FROM dbo.BITACORA);
  INSERT dbo.BITACORA VALUES(@Bit+1,'demo_flota',DATEADD(DAY,-3,@Ahora),CONCAT('DEMO MF parte unidad ',@i));
  IF @i>=3 INSERT dbo.BITACORA VALUES(@Bit+2,'demo_mantenimiento',DATEADD(DAY,-2,@Ahora),CONCAT('DEMO MF orden unidad ',@i));
  IF @i>=4 INSERT dbo.BITACORA VALUES(@Bit+3,'demo_mantenimiento',DATEADD(HOUR,-8,@Ahora),CONCAT('DEMO MF inicio intervencion ',@i));
  IF @i>=5 INSERT dbo.BITACORA VALUES(@Bit+4,'demo_mantenimiento',DATEADD(HOUR,-6,@Ahora),CONCAT('DEMO MF fin intervencion ',@i));
  IF @i=7 INSERT dbo.BITACORA VALUES(@Bit+5,'demo_mantenimiento',DATEADD(HOUR,-5,@Ahora),'DEMO MF habilitacion unidad 7');
  SET @i+=1;
 END;
 -- Viajes propios: no depende de que se haya cargado ni conserva supuestos sobre el avance TD.
 INSERT dbo.MERCADERIA(Codigo,Descripcion) VALUES('DEMO-MF-CARGA',N'Mercadería de prueba para flota');
 DECLARE @Producto INT=SCOPE_IDENTITY(),@Req INT,@Plan INT,@Viaje INT,@Despacho INT;
 INSERT dbo.REQUERIMIENTO_DISTRIBUCION(Numero,Fecha,Origen,Estado) VALUES('DEMO-MF-REQ',@Ahora,N'Depósito de prueba MF','Planificado'); SET @Req=SCOPE_IDENTITY();
 INSERT dbo.PLAN_DISTRIBUCION(Id_Requerimiento,Numero,Fecha,Estado) VALUES(@Req,'DEMO-MF-PLAN',@Ahora,'Confirmado'); SET @Plan=SCOPE_IDENTITY();
 SET @i=1;
 WHILE @i<=2
 BEGIN
  INSERT dbo.VIAJE(Id_Plan,Numero,FechaPrevista,Estado,IdUnidadFlota,IdInformeDisponibilidad)
  VALUES(@Plan,CONCAT('DEMO-MF-VIAJE-',@i),DATEADD(DAY,1,@Ahora),'Preparado',CASE WHEN @i=2 THEN @UnidadAsignada END,CASE WHEN @i=2 THEN @InformeAsignado END);
  SET @Viaje=SCOPE_IDENTITY();
  INSERT dbo.DETALLE_CARGA_PLANIFICADA(Id_Viaje,Id_Mercaderia,CantidadPrevista) VALUES(@Viaje,@Producto,10);
  INSERT dbo.ENTREGA(Id_Viaje,Destino,FechaPrevista,Estado) VALUES(@Viaje,CONCAT(N'Destino de prueba MF ',@i),DATEADD(HOUR,26,@Ahora),'Pendiente');
  INSERT dbo.DOCUMENTO_DESPACHO(Id_Viaje,Numero,Fecha,Estado,Observaciones) VALUES(@Viaje,CONCAT('DEMO-MF-DESPACHO-',@i),@Ahora,'Confirmado',N'DEMO: carga preparada para probar flota.'); SET @Despacho=SCOPE_IDENTITY();
  INSERT dbo.DETALLE_DESPACHO(Id_Despacho,Id_Mercaderia,Cantidad) VALUES(@Despacho,@Producto,10);
  SET @i+=1;
 END;
 INSERT dbo.BITACORA SELECT ISNULL(MAX(Id_Bitacora),0)+1,'demo_trafico_mf',@Ahora,'DEMO MF asignacion unidad 7' FROM dbo.BITACORA;
 INSERT dbo.BITACORA SELECT ISNULL(MAX(Id_Bitacora),0)+1,'Sistema',@Ahora,'DEMO MF v1 cargado' FROM dbo.BITACORA;
 COMMIT;
 SELECT CAST(1 AS INT);
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
