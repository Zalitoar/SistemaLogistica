CREATE PROCEDURE dbo.TD_OBTENER_DESPACHO @IdViaje INT AS
SELECT Id_Despacho, Id_Viaje, Numero, Fecha, Estado, Observaciones FROM dbo.DOCUMENTO_DESPACHO WHERE Id_Viaje=@IdViaje;
