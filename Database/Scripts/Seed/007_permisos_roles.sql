/*
Seed del catálogo base de permisos y composición de roles.

Es idempotente y resuelve las relaciones por nombre para no depender de IDs
que podrían estar ocupados en bases donde ya se ejecutó DevData.
*/

SET NOCOUNT ON;

DECLARE @ComponentesSeguridad TABLE (
    Nombre_Permiso VARCHAR(100) NOT NULL PRIMARY KEY,
    Tipo_Permiso VARCHAR(10) NOT NULL
);

INSERT INTO @ComponentesSeguridad (Nombre_Permiso, Tipo_Permiso)
VALUES
    ('ABM_USUARIOS',    'PERMISO'),
    ('ABM_ROLES',       'PERMISO'),
    ('VER_BITACORA',    'PERMISO'),
    ('BACKUP_RESTORE',  'PERMISO'),
    ('GESTION_VENTAS',  'PERMISO'),
    ('ABM_CLIENTES',    'PERMISO'),
    ('ABM_PRODUCTOS',   'PERMISO'),
    ('VER_HISTORIAL',   'PERMISO'),
    ('Seguridad',       'ROL'),
    ('Vendedor',        'ROL'),
    ('Jefe Inventario', 'ROL');

UPDATE destino
SET destino.Tipo_Permiso = origen.Tipo_Permiso
FROM dbo.PERMISO AS destino
INNER JOIN @ComponentesSeguridad AS origen
    ON origen.Nombre_Permiso = destino.Nombre_Permiso;

INSERT INTO dbo.PERMISO (Nombre_Permiso, Tipo_Permiso)
SELECT origen.Nombre_Permiso, origen.Tipo_Permiso
FROM @ComponentesSeguridad AS origen
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.PERMISO AS destino
    WHERE destino.Nombre_Permiso = origen.Nombre_Permiso
);

DECLARE @IdAdministrador INT = 11;
DECLARE @IdAbmUsuarios INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'ABM_USUARIOS');
DECLARE @IdAbmRoles INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'ABM_ROLES');
DECLARE @IdVerBitacora INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'VER_BITACORA');
DECLARE @IdBackupRestore INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'BACKUP_RESTORE');
DECLARE @IdGestionVentas INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'GESTION_VENTAS');
DECLARE @IdAbmClientes INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'ABM_CLIENTES');
DECLARE @IdAbmProductos INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'ABM_PRODUCTOS');
DECLARE @IdVerHistorial INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'VER_HISTORIAL');
DECLARE @IdSeguridad INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'Seguridad');
DECLARE @IdVendedor INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'Vendedor');
DECLARE @IdJefeInventario INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso = 'Jefe Inventario');

DECLARE @RelacionesBase TABLE (
    Id_Rol INT NOT NULL,
    Id_Componente INT NOT NULL,
    PRIMARY KEY (Id_Rol, Id_Componente)
);

INSERT INTO @RelacionesBase (Id_Rol, Id_Componente)
VALUES
    (@IdAdministrador, @IdBackupRestore),
    (@IdAdministrador, @IdSeguridad),
    (@IdAdministrador, @IdVendedor),
    (@IdAdministrador, @IdJefeInventario),
    (@IdSeguridad, @IdAbmUsuarios),
    (@IdSeguridad, @IdAbmRoles),
    (@IdSeguridad, @IdVerBitacora),
    (@IdVendedor, @IdGestionVentas),
    (@IdVendedor, @IdAbmClientes),
    (@IdVendedor, @IdVerHistorial),
    (@IdJefeInventario, @IdAbmProductos);

INSERT INTO dbo.ROL_COMPONENTE (Id_Rol, Id_Componente)
SELECT origen.Id_Rol, origen.Id_Componente
FROM @RelacionesBase AS origen
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.ROL_COMPONENTE AS destino
    WHERE destino.Id_Rol = origen.Id_Rol
      AND destino.Id_Componente = origen.Id_Componente
);

-- Permisos de Transporte y Distribución. No altera la composición previa.
DECLARE @PermisosTD TABLE (Nombre VARCHAR(100) PRIMARY KEY);
INSERT @PermisosTD VALUES ('TD_PLANIFICAR_DISTRIBUCION'), ('TD_PREPARAR_DESPACHO'), ('TD_EJECUTAR_VIAJE'), ('TD_REGISTRAR_ENTREGA'), ('TD_CONTROLAR_CUMPLIMIENTO');
INSERT dbo.PERMISO(Nombre_Permiso,Tipo_Permiso)
SELECT Nombre,'PERMISO' FROM @PermisosTD T
WHERE NOT EXISTS (SELECT 1 FROM dbo.PERMISO P WHERE P.Nombre_Permiso=T.Nombre);
DECLARE @AdministradorTD INT = (SELECT MIN(Id_Permiso) FROM dbo.PERMISO WHERE Nombre_Permiso='Administrador' AND Tipo_Permiso='ROL');
IF @AdministradorTD IS NULL THROW 50002, 'No existe el rol Administrador.', 1;
INSERT dbo.ROL_COMPONENTE(Id_Rol,Id_Componente)
SELECT @AdministradorTD,P.Id_Permiso FROM dbo.PERMISO P JOIN @PermisosTD T ON T.Nombre=P.Nombre_Permiso
WHERE NOT EXISTS (SELECT 1 FROM dbo.ROL_COMPONENTE R WHERE R.Id_Rol=@AdministradorTD AND R.Id_Componente=P.Id_Permiso);

-- Configuración de conexión: autorización por permiso, sin comprobar nombres en la UI.
IF NOT EXISTS (SELECT 1 FROM dbo.PERMISO WHERE Nombre_Permiso='CONFIGURAR_BD')
    INSERT dbo.PERMISO(Nombre_Permiso,Tipo_Permiso) VALUES ('CONFIGURAR_BD','PERMISO');
INSERT dbo.ROL_COMPONENTE(Id_Rol,Id_Componente)
SELECT @AdministradorTD,Id_Permiso FROM dbo.PERMISO P WHERE Nombre_Permiso='CONFIGURAR_BD'
AND NOT EXISTS (SELECT 1 FROM dbo.ROL_COMPONENTE R WHERE R.Id_Rol=@AdministradorTD AND R.Id_Componente=P.Id_Permiso);
