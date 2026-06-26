CREATE PROC [dbo].[QUITAR_COMPONENTE_ROL]
    @Id_Rol int,
    @Id_Componente int
as
begin
    delete from ROL_COMPONENTE
    where Id_Rol = @Id_Rol and Id_Componente = @Id_Componente
end
