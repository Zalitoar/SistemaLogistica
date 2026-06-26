CREATE PROC [dbo].[LISTAR_ROLES]
as
begin
    select Id_Permiso, Nombre_Permiso, Tipo_Permiso
    from PERMISO
    where Tipo_Permiso = 'ROL'
end
