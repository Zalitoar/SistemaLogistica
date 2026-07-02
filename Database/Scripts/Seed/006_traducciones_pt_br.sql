/*
Seed de traducciones base: Português.

Este script inserta solamente las traducciones que no existen.
No actualiza traducciones existentes para no pisar cambios hechos desde la aplicación por un administrador.
*/

DECLARE @IdIdioma_pt_BR INT;

SELECT @IdIdioma_pt_BR = Id_Idioma
FROM dbo.IDIOMA
WHERE Codigo_Idioma = N'pt-BR';

IF @IdIdioma_pt_BR IS NULL
BEGIN
    RAISERROR('No existe el idioma pt-BR. Revisar el script 001_idiomas.sql.', 16, 1);
    RETURN;
END;

DECLARE @Traducciones_pt_BR TABLE (
    Clave_Traduccion NVARCHAR(200) NOT NULL PRIMARY KEY,
    Valor_Traduccion NVARCHAR(1000) NULL
);

INSERT INTO @Traducciones_pt_BR (
    Clave_Traduccion,
    Valor_Traduccion
)
VALUES
    (N'Common.btnAceptar.Text', N'Aceitar'),
    (N'Common.btnBorrar.Text', N'Excluir'),
    (N'Common.btnBuscar.Text', N'Pesquisar'),
    (N'Common.btnCancelar.Text', N'Cancelar'),
    (N'Common.btnEditar.Text', N'Editar'),
    (N'Common.btnGuardar.Text', N'Salvar'),
    (N'Common.btnNuevo.Text', N'Novo'),
    (N'Common.btnSalir.Text', N'Sair'),
    (N'Common.msgConfirmar', N'Confirmar'),
    (N'Common.msgError', N'Erro'),
    (N'FrmABMRoles.Title', N'Administração de funções'),
    (N'FrmABMRoles.btnAsignar.Text', N'Atribuir'),
    (N'FrmABMRoles.btnInsertar.Text', N'Inserir'),
    (N'FrmABMRoles.btnQuitar.Text', N'Remover'),
    (N'FrmABMRoles.btnborrar.Text', N'Excluir'),
    (N'FrmABMRoles.btnlistar.Text', N'Listar'),
    (N'FrmABMRoles.btnmodificar.Text', N'Modificar'),
    (N'FrmABMRoles.lblIdRol.Text', N'Id função'),
    (N'FrmABMRoles.lblNombreRol.Text', N'Nome'),
    (N'FrmABMUsuarios.Title', N'Administração de usuários'),
    (N'FrmABMUsuarios.btnInsertar.Text', N'Inserir'),
    (N'FrmABMUsuarios.btnRestaurarVersAnt.Text', N'Restaurar versão'),
    (N'FrmABMUsuarios.btnborrar.Text', N'Excluir'),
    (N'FrmABMUsuarios.btnlistar.Text', N'Listar'),
    (N'FrmABMUsuarios.btnmodificar.Text', N'Modificar'),
    (N'FrmABMUsuarios.groupBox1.Text', N'Recuperar estados anteriores'),
    (N'FrmABMUsuarios.lblClave.Text', N'Senha'),
    (N'FrmABMUsuarios.lblD.Text', N'ID'),
    (N'FrmABMUsuarios.lblIdUsuarioSeleccionado.Text', N'Id selecionado'),
    (N'FrmABMUsuarios.lblNombre.Text', N'Nome'),
    (N'FrmABMUsuarios.lblPerfil.Text', N'Perfil'),
    (N'FrmABMUsuarios.lblUsuarioSeleccionado.Text', N'Usuário selecionado'),
    (N'FrmABMUsuarios.msgClaveNoCumple', N'A senha não atende aos requisitos. Deve ter pelo menos 6 caracteres, uma letra maiúscula e um número.'),
    (N'FrmApp.Title', N'Sistema de gestão administrativa'),
    (N'FrmApp.archivoToolStripMenuItem.Text', N'Arquivo'),
    (N'FrmApp.bitácoraToolStripMenuItem.Text', N'Registro de auditoria'),
    (N'FrmApp.cerrarSesiónToolStripMenuItem.Text', N'Encerrar sessão'),
    (N'FrmApp.clientesToolStripMenuItem.Text', N'Nova venda'),
    (N'FrmApp.gestiónToolStripMenuItem.Text', N'Gestão'),
    (N'FrmApp.historialToolStripMenuItem.Text', N'Histórico'),
    (N'FrmApp.idiomasToolStripMenuItem.Text', N'Idiomas'),
    (N'FrmApp.msgConfirmarCerrarSesion', N'Deseja realmente encerrar a sessão?'),
    (N'FrmApp.msgConfirmarSalir', N'Deseja realmente encerrar a sessão e sair?'),
    (N'FrmApp.msgErrorAbrirIdiomas', N'Não foi possível abrir o formulário de idiomas.'),
    (N'FrmApp.msgTituloConfirmarSalida', N'Confirmar saída'),
    (N'FrmApp.perfilesToolStripMenuItem.Text', N'Perfis'),
    (N'FrmApp.productosToolStripMenuItem.Text', N'Clientes'),
    (N'FrmApp.productosToolStripMenuItem1.Text', N'Produtos'),
    (N'FrmApp.salirToolStripMenuItem.Text', N'Sair'),
    (N'FrmApp.seguridadToolStripMenuItem.Text', N'Segurança'),
    (N'FrmApp.usuariosToolStripMenuItem.Text', N'Usuários'),
    (N'FrmBitacora.Title', N'Registro de auditoria'),
    (N'FrmBitacora.btnBuscar.Text', N'Pesquisar'),
    (N'FrmBitacora.gbFiltros.Text', N'Filtros'),
    (N'FrmBitacora.lblFechaDesde.Text', N'Data inicial'),
    (N'FrmBitacora.lblFechaHasta.Text', N'Data final'),
    (N'FrmBitacora.lblUsuario.Text', N'Usuário'),
    (N'FrmIdioma.Title', N'Gestão de idiomas'),
    (N'FrmIdioma.btnActivar.Text', N'Ativar'),
    (N'FrmIdioma.btnBorrar.Text', N'Excluir'),
    (N'FrmIdioma.btnCargarTraducciones.Text', N'Carregar traduções'),
    (N'FrmIdioma.btnDesactivar.Text', N'Desativar'),
    (N'FrmIdioma.btnEditar.Text', N'Editar'),
    (N'FrmIdioma.btnGuardarTraducciones.Text', N'Salvar traduções'),
    (N'FrmIdioma.btnNuevo.Text', N'Novo'),
    (N'FrmIdioma.btnSetDefault.Text', N'Padrão'),
    (N'FrmIdioma.dgvIdiomas.Codigo.HeaderText', N'Código'),
    (N'FrmIdioma.dgvIdiomas.Habilitado.HeaderText', N'Ativado'),
    (N'FrmIdioma.dgvIdiomas.EsDefault.HeaderText', N'Padrão'),
    (N'FrmIdioma.dgvIdiomas.Id_Idioma.HeaderText', N'Id'),
    (N'FrmIdioma.dgvIdiomas.Nombre.HeaderText', N'Nome'),
    (N'FrmIdioma.dgvTraducciones.Clave.HeaderText', N'Chave'),
    (N'FrmIdioma.dgvTraducciones.Id_Idioma.HeaderText', N'Idioma'),
    (N'FrmIdioma.dgvTraducciones.Id_Traduccion.HeaderText', N'Id'),
    (N'FrmIdioma.dgvTraducciones.Valor.HeaderText', N'Valor'),
    (N'FrmIdioma.msgAccesoDenegado', N'Acesso negado. Requer função de administrador.'),
    (N'FrmIdioma.msgBorrarIdioma', N'Excluir idioma?'),
    (N'FrmIdioma.msgConfirmar', N'Confirmar'),
    (N'FrmIdioma.msgTraduccionesGuardadas', N'Traduções salvas.'),
    (N'FrmIdiomaEditor.Title', N'Idioma'),
    (N'FrmIdiomaEditor.btnCancel.Text', N'Cancelar'),
    (N'FrmIdiomaEditor.btnOk.Text', N'OK'),
    (N'FrmIdiomaEditor.chkHabilitado.Text', N'Ativado'),
    (N'FrmIdiomaEditor.lblCodigo.Text', N'Código:'),
    (N'FrmIdiomaEditor.lblNombre.Text', N'Nome:'),
    (N'FrmLogin.Title', N'Login'),
    (N'FrmLogin.btnIngresar.Text', N'Entrar'),
    (N'FrmLogin.btnSalir.Text', N'Sair'),
    (N'FrmLogin.cmbIdiomasLogin.Text', N'Idioma'),
    (N'FrmLogin.lblClave.Text', N'Senha'),
    (N'FrmLogin.lblUsuario.Text', N'Usuário'),
    (N'FrmLogin.msgCamposRequeridos', N'Todos os campos são obrigatórios.'),
    (N'FrmLogin.msgCredencialesInvalidas', N'Usuário e/ou senha incorretos ou inexistentes.'),
    (N'FrmLogin.msgIngresoExitoso', N'Login realizado com sucesso.'),
    (N'FrmLogin.msgSistemaNoDisponible', N'O sistema não está disponível no momento. Tente novamente mais tarde.'),
    (N'frmRestore.Title', N'Restauração de integridade'),
    (N'frmRestore.btnCancelar.Text', N'Cancelar'),
    (N'frmRestore.btnRecalcular.Text', N'Recalcular'),
    (N'frmRestore.btnRestore.Text', N'Backup do banco de dados'),
    (N'frmRestore.lblIntegridadDVV.Text', N'Integridade da tabela'),
    (N'frmRestore.lblRegistros.Text', N'Integridade dos registros'),
    (N'frmRestore.msgIntegridadDVVCorrecta', N'Integridade da tabela correta.'),
    (N'frmRestore.msgIntegridadDVVFallida', N'Falha na integridade da tabela.'),
    (N'frmRestore.msgIntegridadRegistrosCorrecta', N'Integridade dos registros correta.'),
    (N'frmRestore.msgIntegridadRegistrosFallida', N'Falha na integridade dos registros.'),
    (N'frmRestore.msgRestauracionError', N'Erro ao restaurar o banco de dados.'),
    (N'frmRestore.msgRestauracionExitosa', N'Restauração concluída com sucesso.');

INSERT INTO dbo.TRADUCCION (
    Id_Idioma,
    Clave_Traduccion,
    Valor_Traduccion
)
SELECT
    @IdIdioma_pt_BR,
    origen.Clave_Traduccion,
    origen.Valor_Traduccion
FROM @Traducciones_pt_BR AS origen
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.TRADUCCION AS destino
    WHERE destino.Id_Idioma = @IdIdioma_pt_BR
      AND destino.Clave_Traduccion = origen.Clave_Traduccion
);
