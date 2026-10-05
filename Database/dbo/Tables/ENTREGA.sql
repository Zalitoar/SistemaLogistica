CREATE TABLE dbo.ENTREGA
(
    Id_Entrega INT IDENTITY PRIMARY KEY,
    Id_Viaje INT NOT NULL REFERENCES dbo.VIAJE(Id_Viaje),
    Destino NVARCHAR(200) NOT NULL,
    FechaPrevista DATETIME NOT NULL,
    FechaRealizada DATETIME NULL,
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Pendiente','Entregada','Parcial','Incumplida')),
    Observaciones NVARCHAR(1000) NULL,
    CONSTRAINT CK_ENTREGA_RESULTADO CHECK ((Estado = 'Pendiente' AND FechaRealizada IS NULL) OR (Estado <> 'Pendiente' AND FechaRealizada IS NOT NULL))
);
