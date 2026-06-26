CREATE PROC [dbo].[BORRAR_ROL]
    @Id_Permiso int
as
begin
    delete from PERMISO
    where Id_Permiso = @Id_Permiso and Tipo_Permiso = 'ROL'
end

