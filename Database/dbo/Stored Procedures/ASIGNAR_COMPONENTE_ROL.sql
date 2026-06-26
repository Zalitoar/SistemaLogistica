CREATE PROC [dbo].[ASIGNAR_COMPONENTE_ROL]
    @Id_Rol int,
    @Id_Componente int
as
begin
    insert into ROL_COMPONENTE (Id_Rol, Id_Componente)
    values (@Id_Rol, @Id_Componente)
end
