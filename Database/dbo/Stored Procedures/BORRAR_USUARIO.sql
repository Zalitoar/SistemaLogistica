
CREATE proc [dbo].[BORRAR_USUARIO]
@Id_Usuario int,
@dvh_Usuario varchar(64)

as
begin

update USUARIO set
Borrado_Usuario = '1',
dvh_Usuario = @dvh_Usuario
where Id_Usuario = @Id_Usuario

end
