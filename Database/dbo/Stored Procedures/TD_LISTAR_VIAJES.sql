CREATE PROCEDURE dbo.TD_LISTAR_VIAJES AS
SELECT Id_Viaje, Id_Plan, Numero, FechaPrevista, FechaInicio, FechaFinalizacion, Estado, PorcentajeCumplimiento
FROM dbo.VIAJE ORDER BY FechaPrevista DESC, Id_Viaje DESC;
