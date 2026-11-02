CREATE TABLE dbo.VIAJE
(
    Id_Viaje INT IDENTITY PRIMARY KEY,
    IdUnidadFlota INT NULL CONSTRAINT FK_VIAJE_UNIDAD REFERENCES dbo.UNIDAD_FLOTA(IdUnidadFlota),
    IdInformeDisponibilidad INT NULL,
    CONSTRAINT FK_VIAJE_INFORME_UNIDAD FOREIGN KEY(IdInformeDisponibilidad,IdUnidadFlota) REFERENCES dbo.INFORME_DISPONIBILIDAD(IdInformeDisponibilidad,IdUnidadFlota),
    CONSTRAINT CK_VIAJE_ASIGNACION CHECK((IdUnidadFlota IS NULL AND IdInformeDisponibilidad IS NULL) OR (IdUnidadFlota IS NOT NULL AND IdInformeDisponibilidad IS NOT NULL)),
    Id_Plan INT NOT NULL REFERENCES dbo.PLAN_DISTRIBUCION(Id_Plan),
    Numero NVARCHAR(50) NOT NULL UNIQUE,
    FechaPrevista DATETIME NOT NULL,
    FechaInicio DATETIME NULL,
    FechaFinalizacion DATETIME NULL,
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Planificado','Preparado','EnCurso','Cerrado')),
    PorcentajeCumplimiento DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (PorcentajeCumplimiento BETWEEN 0 AND 100),
    CONSTRAINT CK_VIAJE_FECHAS CHECK ((Estado IN ('Planificado','Preparado') AND FechaInicio IS NULL AND FechaFinalizacion IS NULL) OR (Estado = 'EnCurso' AND FechaInicio IS NOT NULL AND FechaFinalizacion IS NULL) OR (Estado = 'Cerrado' AND FechaInicio IS NOT NULL AND FechaFinalizacion >= FechaInicio)),
    CONSTRAINT CK_VIAJE_CIERRE CHECK (Estado <> 'Cerrado' OR (FechaFinalizacion IS NOT NULL AND PorcentajeCumplimiento = 100))
);
