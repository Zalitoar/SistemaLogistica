CREATE TABLE [dbo].[IDIOMA] (
    [Id_Idioma]         INT            IDENTITY (1, 1) NOT NULL,
    [Codigo_Idioma]     NVARCHAR (10)  NOT NULL,
    [Nombre_Idioma]     NVARCHAR (100) NOT NULL,
    [EsDefault]         BIT            DEFAULT ((0)) NOT NULL,
    [Habilitado_Idioma] BIT            DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id_Idioma] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Idioma_Codigo]
    ON [dbo].[IDIOMA]([Codigo_Idioma] ASC);

