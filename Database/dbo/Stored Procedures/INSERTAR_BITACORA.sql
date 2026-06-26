create proc [dbo].[INSERTAR_BITACORA]
@Usuario_Bitacora varchar(50),
@FechaHora_Bitacora datetime,
@Actividad_Bitacora varchar(50)
as
begin

declare @id int

set @id = (select ISNULL(max(Id_Bitacora),0) +1 from BITACORA)

insert into BITACORA(Id_Bitacora, Usuario_Bitacora,FechaHora_Bitacora, Actividad_Bitacora)
values (@id, @Usuario_Bitacora,@FechaHora_Bitacora,@Actividad_Bitacora)

end
