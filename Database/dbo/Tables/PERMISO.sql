CREATE TABLE [dbo].[PERMISO] (
    [Id_Permiso]     INT           IDENTITY (1, 1) NOT NULL,
    [Nombre_Permiso] VARCHAR (100) NOT NULL,
    [Tipo_Permiso]   VARCHAR (10)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id_Permiso] ASC),
    CHECK ([Tipo_Permiso]='ROL' OR [Tipo_Permiso]='PERMISO'),
    CHECK ([Tipo_Permiso]='ROL' OR [Tipo_Permiso]='PERMISO')
);

