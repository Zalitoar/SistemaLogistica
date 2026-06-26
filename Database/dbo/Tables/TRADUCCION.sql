CREATE TABLE [dbo].[TRADUCCION] (
    [Id_Traduccion]    INT             IDENTITY (1, 1) NOT NULL,
    [Id_Idioma]        INT             NOT NULL,
    [Clave_Traduccion] NVARCHAR (200)  NOT NULL,
    [Valor_Traduccion] NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([Id_Traduccion] ASC),
    CONSTRAINT [FK_Traduccion_Idioma] FOREIGN KEY ([Id_Idioma]) REFERENCES [dbo].[IDIOMA] ([Id_Idioma])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Traduccion_Idioma_Clave]
    ON [dbo].[TRADUCCION]([Id_Idioma] ASC, [Clave_Traduccion] ASC);

