CREATE PROC [dbo].[INSERTAR_USUARIO]
    @Nombre_Usuario varchar(50),
    @Clave_Usuario varchar(64),
    @Id_Rol int,
    @dvh_Usuario varchar(64)
as
begin
    declare @id int
    set @id = (select ISNULL(max(Id_Usuario),0) +1 from USUARIO)
    insert into USUARIO (Id_Usuario, Nombre_Usuario, Clave_Usuario, Id_Rol, Borrado_Usuario, dvh_Usuario)
    values (@id, @Nombre_Usuario, @Clave_Usuario, @Id_Rol, '0', @dvh_Usuario)
end
