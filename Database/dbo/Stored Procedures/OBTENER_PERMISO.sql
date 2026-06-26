CREATE PROC [dbo].[OBTENER_PERMISO]
    @Id_Permiso int
as
begin
    select Id_Permiso, Nombre_Permiso, Tipo_Permiso
    from PERMISO
    where Id_Permiso = @Id_Permiso
end


