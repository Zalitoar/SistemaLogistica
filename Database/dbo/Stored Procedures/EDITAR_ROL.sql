CREATE PROC [dbo].[EDITAR_ROL]
    @Id_Permiso int,
    @Nombre_Permiso varchar(100)
as
begin
    update PERMISO
    set Nombre_Permiso = @Nombre_Permiso
    where Id_Permiso = @Id_Permiso and Tipo_Permiso = 'ROL'
end
