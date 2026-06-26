CREATE PROC [dbo].[EDITAR_USUARIO]
    @Id_Usuario int,
    @Nombre_Usuario varchar(50),
    @Clave_Usuario varchar(64),
    @Id_Rol int,
    @dvh_Usuario varchar(64)
as
begin
    update USUARIO set
        Nombre_Usuario = @Nombre_Usuario,
        Clave_Usuario = @Clave_Usuario,
        Id_Rol = @Id_Rol,
        dvh_Usuario = @dvh_Usuario
    where Id_Usuario = @Id_Usuario
end
