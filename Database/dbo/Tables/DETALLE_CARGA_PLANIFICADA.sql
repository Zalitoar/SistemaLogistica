CREATE TABLE dbo.DETALLE_CARGA_PLANIFICADA
(
    Id_DetalleCargaPlanificada INT IDENTITY PRIMARY KEY,
    Id_Viaje INT NOT NULL REFERENCES dbo.VIAJE(Id_Viaje),
    Id_Mercaderia INT NOT NULL REFERENCES dbo.MERCADERIA(Id_Mercaderia),
    CantidadPrevista DECIMAL(18,3) NOT NULL CHECK (CantidadPrevista > 0),
    CONSTRAINT UQ_CARGA_VIAJE_MERCADERIA UNIQUE(Id_Viaje,
    Id_Mercaderia)
);
