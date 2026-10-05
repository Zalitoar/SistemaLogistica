CREATE PROCEDURE dbo.TD_OBTENER_ENTREGAS @IdViaje INT AS
SELECT E.Id_Entrega,E.Id_Viaje,E.Destino,E.FechaPrevista,E.FechaRealizada,E.Estado,E.Observaciones,
    C.Id_Comprobante,C.Numero AS NumeroComprobante,C.Fecha AS FechaComprobante,C.Resultado,C.Observaciones AS ObservacionesComprobante
FROM dbo.ENTREGA E LEFT JOIN dbo.COMPROBANTE_ENTREGA C ON C.Id_Entrega=E.Id_Entrega
WHERE E.Id_Viaje=@IdViaje ORDER BY E.FechaPrevista,E.Id_Entrega;
