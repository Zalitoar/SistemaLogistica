CREATE PROC [dbo].[INSERTAR_ROL]
    @Nombre_Permiso varchar(100)
as
begin
    insert into PERMISO (Nombre_Permiso, Tipo_Permiso)
    values (@Nombre_Permiso, 'ROL')

    select SCOPE_IDENTITY() as Id_Permiso
end
