CREATE PROCEDURE dbo.TD_OBTENER_CARGA @IdViaje INT AS
SELECT C.Id_Mercaderia, M.Codigo, M.Descripcion, C.CantidadPrevista, COALESCE(D.Cantidad,0) AS CantidadPreparada
FROM dbo.DETALLE_CARGA_PLANIFICADA C JOIN dbo.MERCADERIA M ON M.Id_Mercaderia=C.Id_Mercaderia
LEFT JOIN dbo.DOCUMENTO_DESPACHO H ON H.Id_Viaje=C.Id_Viaje
LEFT JOIN dbo.DETALLE_DESPACHO D ON D.Id_Despacho=H.Id_Despacho AND D.Id_Mercaderia=C.Id_Mercaderia
WHERE C.Id_Viaje=@IdViaje ORDER BY M.Codigo;
