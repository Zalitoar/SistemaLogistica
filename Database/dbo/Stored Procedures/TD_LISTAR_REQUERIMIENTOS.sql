CREATE PROCEDURE dbo.TD_LISTAR_REQUERIMIENTOS AS
SELECT Id_Requerimiento, Numero, Fecha, Origen, Estado FROM dbo.REQUERIMIENTO_DISTRIBUCION WHERE Estado = 'Pendiente' ORDER BY Fecha, Id_Requerimiento;
