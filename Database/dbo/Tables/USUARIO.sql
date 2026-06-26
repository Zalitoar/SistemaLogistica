CREATE TABLE [dbo].[USUARIO] (
    [Id_Usuario]      INT          NOT NULL,
    [Nombre_Usuario]  VARCHAR (50) NOT NULL,
    [Clave_Usuario]   VARCHAR (64) NOT NULL,
    [Borrado_Usuario] INT          NULL,
    [dvh_Usuario]     VARCHAR (64) NULL,
    [Id_Rol]          INT          NOT NULL,
    CONSTRAINT [FK_USUARIO_ROL] FOREIGN KEY ([Id_Rol]) REFERENCES [dbo].[PERMISO] ([Id_Permiso])
);

