CREATE PROC [dbo].[LISTAR_COMPONENTES_ROL]
    @Id_Rol int
as
begin
    select Id_Componente
    from ROL_COMPONENTE
    where Id_Rol = @Id_Rol
end
