CREATE TABLE dbo.ORDEN_MANTENIMIENTO
(
    IdOrdenMantenimiento INT IDENTITY CONSTRAINT PK_ORDEN_MANTENIMIENTO PRIMARY KEY,
    IdUnidadFlota INT NOT NULL CONSTRAINT FK_ORDEN_UNIDAD REFERENCES dbo.UNIDAD_FLOTA(IdUnidadFlota),
    IdParteEstadoUnidad INT NOT NULL CONSTRAINT UQ_ORDEN_PARTE UNIQUE,
    FechaEmision DATETIME NOT NULL,
    FechaProgramada DATETIME NOT NULL,
    TipoMantenimiento VARCHAR(30) NOT NULL CONSTRAINT CK_ORDEN_TIPO CHECK(TipoMantenimiento IN ('Preventivo','Correctivo')),
    Prioridad VARCHAR(20) NOT NULL CONSTRAINT CK_ORDEN_PRIORIDAD CHECK(Prioridad IN ('Baja','Media','Alta','Urgente')),
    DescripcionTrabajo NVARCHAR(500) NOT NULL,
    EstadoOrden VARCHAR(30) NOT NULL CONSTRAINT CK_ORDEN_ESTADO CHECK(EstadoOrden IN ('Programada','EnCurso','PendienteVerificacion','Cerrada')),
    CONSTRAINT FK_ORDEN_PARTE_UNIDAD FOREIGN KEY(IdParteEstadoUnidad,IdUnidadFlota) REFERENCES dbo.PARTE_ESTADO_UNIDAD(IdParteEstadoUnidad,IdUnidadFlota)
);
