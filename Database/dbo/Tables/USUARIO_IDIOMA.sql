CREATE TABLE [dbo].[USUARIO_IDIOMA] (
    [Id_Usuario] INT NOT NULL,
    [Id_Idioma]  INT NOT NULL,
    CONSTRAINT [PK_Usuario_Idioma] PRIMARY KEY CLUSTERED ([Id_Usuario] ASC, [Id_Idioma] ASC),
    CONSTRAINT [FK_UsuarioIdioma_Idioma] FOREIGN KEY ([Id_Idioma]) REFERENCES [dbo].[IDIOMA] ([Id_Idioma])
);

