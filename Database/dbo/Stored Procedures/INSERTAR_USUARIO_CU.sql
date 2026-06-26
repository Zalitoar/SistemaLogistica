
Create PROC [dbo].[INSERTAR_USUARIO_CU]
	@Id_UsuarioCU int,
    @Nombre_UsuarioCU varchar(50),
    @Clave_UsuarioCU varchar(64),
    @Borrado_UsuarioCU int,
	@Id_RolCU int
    
as
begin
    declare @id int
    set @id = (select ISNULL(max(Id_CU),0) +1 from CambioUsuario)
    insert into CambioUsuario (Id_CU, Id_UsuarioCU, Nombre_UsuarioCU, Clave_UsuarioCU, Id_RolCU, Borrado_UsuarioCU)
    values (@id, @Id_UsuarioCU,  @Nombre_UsuarioCU, @Clave_UsuarioCU, @Id_RolCU, @Borrado_UsuarioCU)
end
