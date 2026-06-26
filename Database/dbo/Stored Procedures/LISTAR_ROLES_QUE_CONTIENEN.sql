CREATE PROC [dbo].[LISTAR_ROLES_QUE_CONTIENEN]
    @Id_Componente int
as
begin
    select Id_Rol
    from ROL_COMPONENTE
    where Id_Componente = @Id_Componente
end
