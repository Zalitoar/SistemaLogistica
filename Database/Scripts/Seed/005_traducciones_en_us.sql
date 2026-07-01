/*
Seed de traducciones base: English.

Este script inserta solamente las traducciones que no existen.
No actualiza traducciones existentes para no pisar cambios hechos desde la aplicación por un administrador.
*/

DECLARE @IdIdioma_en_US INT;

SELECT @IdIdioma_en_US = Id_Idioma
FROM dbo.IDIOMA
WHERE Codigo_Idioma = N'en-US';

IF @IdIdioma_en_US IS NULL
BEGIN
    RAISERROR('No existe el idioma en-US. Revisar el script 001_idiomas.sql.', 16, 1);
    RETURN;
END;

DECLARE @Traducciones_en_US TABLE (
    Clave_Traduccion NVARCHAR(200) NOT NULL PRIMARY KEY,
    Valor_Traduccion NVARCHAR(1000) NULL
);

INSERT INTO @Traducciones_en_US (
    Clave_Traduccion,
    Valor_Traduccion
)
VALUES
    (N'Common.btnAceptar.Text', N'OK'),
    (N'Common.btnBorrar.Text', N'Delete'),
    (N'Common.btnBuscar.Text', N'Search'),
    (N'Common.btnCancelar.Text', N'Cancel'),
    (N'Common.btnEditar.Text', N'Edit'),
    (N'Common.btnGuardar.Text', N'Save'),
    (N'Common.btnNuevo.Text', N'New'),
    (N'Common.btnSalir.Text', N'Exit'),
    (N'Common.msgConfirmar', N'Confirm'),
    (N'Common.msgError', N'Error'),
    (N'FrmABMRoles.Title', N'Role management'),
    (N'FrmABMRoles.btnAsignar.Text', N'Assign'),
    (N'FrmABMRoles.btnInsertar.Text', N'Insert'),
    (N'FrmABMRoles.btnQuitar.Text', N'Remove'),
    (N'FrmABMRoles.btnborrar.Text', N'Delete'),
    (N'FrmABMRoles.btnlistar.Text', N'List'),
    (N'FrmABMRoles.btnmodificar.Text', N'Modify'),
    (N'FrmABMRoles.lblIdRol.Text', N'Role id'),
    (N'FrmABMRoles.lblNombreRol.Text', N'Name'),
    (N'FrmABMUsuarios.Title', N'User management'),
    (N'FrmABMUsuarios.btnInsertar.Text', N'Insert'),
    (N'FrmABMUsuarios.btnRestaurarVersAnt.Text', N'Restore version'),
    (N'FrmABMUsuarios.btnborrar.Text', N'Delete'),
    (N'FrmABMUsuarios.btnlistar.Text', N'List'),
    (N'FrmABMUsuarios.btnmodificar.Text', N'Modify'),
    (N'FrmABMUsuarios.groupBox1.Text', N'Recover previous states'),
    (N'FrmABMUsuarios.lblClave.Text', N'Password'),
    (N'FrmABMUsuarios.lblD.Text', N'ID'),
    (N'FrmABMUsuarios.lblIdUsuarioSeleccionado.Text', N'Selected id'),
    (N'FrmABMUsuarios.lblNombre.Text', N'Name'),
    (N'FrmABMUsuarios.lblPerfil.Text', N'Profile'),
    (N'FrmABMUsuarios.lblUsuarioSeleccionado.Text', N'Selected user'),
    (N'FrmABMUsuarios.msgClaveNoCumple', N'The password does not meet the requirements. It must have at least 6 characters, one uppercase letter, and one number.'),
    (N'FrmApp.Title', N'Administrative management system'),
    (N'FrmApp.archivoToolStripMenuItem.Text', N'File'),
    (N'FrmApp.bitácoraToolStripMenuItem.Text', N'Audit log'),
    (N'FrmApp.cerrarSesiónToolStripMenuItem.Text', N'Log out'),
    (N'FrmApp.clientesToolStripMenuItem.Text', N'New sale'),
    (N'FrmApp.gestiónToolStripMenuItem.Text', N'Management'),
    (N'FrmApp.historialToolStripMenuItem.Text', N'History'),
    (N'FrmApp.idiomasToolStripMenuItem.Text', N'Languages'),
    (N'FrmApp.msgConfirmarCerrarSesion', N'Do you really want to log out?'),
    (N'FrmApp.msgConfirmarSalir', N'Do you really want to log out and exit?'),
    (N'FrmApp.msgErrorAbrirIdiomas', N'The language form could not be opened.'),
    (N'FrmApp.msgTituloConfirmarSalida', N'Confirm exit'),
    (N'FrmApp.perfilesToolStripMenuItem.Text', N'Profiles'),
    (N'FrmApp.productosToolStripMenuItem.Text', N'Customers'),
    (N'FrmApp.productosToolStripMenuItem1.Text', N'Products'),
    (N'FrmApp.salirToolStripMenuItem.Text', N'Exit'),
    (N'FrmApp.seguridadToolStripMenuItem.Text', N'Security'),
    (N'FrmApp.usuariosToolStripMenuItem.Text', N'Users'),
    (N'FrmBitacora.Title', N'Audit log'),
    (N'FrmBitacora.btnBuscar.Text', N'Search'),
    (N'FrmBitacora.gbFiltros.Text', N'Filters'),
    (N'FrmBitacora.lblFechaDesde.Text', N'Start date'),
    (N'FrmBitacora.lblFechaHasta.Text', N'End date'),
    (N'FrmBitacora.lblUsuario.Text', N'User'),
    (N'FrmIdioma.Title', N'Language management'),
    (N'FrmIdioma.btnActivar.Text', N'Enable'),
    (N'FrmIdioma.btnBorrar.Text', N'Delete'),
    (N'FrmIdioma.btnCargarTraducciones.Text', N'Load translations'),
    (N'FrmIdioma.btnDesactivar.Text', N'Disable'),
    (N'FrmIdioma.btnEditar.Text', N'Edit'),
    (N'FrmIdioma.btnGuardarTraducciones.Text', N'Save translations'),
    (N'FrmIdioma.btnNuevo.Text', N'New'),
    (N'FrmIdioma.btnSetDefault.Text', N'Default'),
    (N'FrmIdioma.dgvIdiomas.Codigo.HeaderText', N'Code'),
    (N'FrmIdioma.dgvIdiomas.Habilitado.HeaderText', N'Enabled'),
    (N'FrmIdioma.dgvIdiomas.Id_Idioma.HeaderText', N'Id'),
    (N'FrmIdioma.dgvIdiomas.Nombre.HeaderText', N'Name'),
    (N'FrmIdioma.dgvTraducciones.Clave.HeaderText', N'Key'),
    (N'FrmIdioma.dgvTraducciones.Id_Idioma.HeaderText', N'Language'),
    (N'FrmIdioma.dgvTraducciones.Id_Traduccion.HeaderText', N'Id'),
    (N'FrmIdioma.dgvTraducciones.Valor.HeaderText', N'Value'),
    (N'FrmIdioma.msgAccesoDenegado', N'Access denied. Administrator role required.'),
    (N'FrmIdioma.msgBorrarIdioma', N'Delete language?'),
    (N'FrmIdioma.msgConfirmar', N'Confirm'),
    (N'FrmIdioma.msgTraduccionesGuardadas', N'Translations saved.'),
    (N'FrmIdiomaEditor.Title', N'Language'),
    (N'FrmIdiomaEditor.btnCancel.Text', N'Cancel'),
    (N'FrmIdiomaEditor.btnOk.Text', N'OK'),
    (N'FrmIdiomaEditor.chkHabilitado.Text', N'Enabled'),
    (N'FrmIdiomaEditor.lblCodigo.Text', N'Code:'),
    (N'FrmIdiomaEditor.lblNombre.Text', N'Name:'),
    (N'FrmLogin.Title', N'Login'),
    (N'FrmLogin.btnIngresar.Text', N'Log in'),
    (N'FrmLogin.btnSalir.Text', N'Exit'),
    (N'FrmLogin.cmbIdiomasLogin.Text', N'Language'),
    (N'FrmLogin.lblClave.Text', N'Password'),
    (N'FrmLogin.lblUsuario.Text', N'User'),
    (N'FrmLogin.msgCamposRequeridos', N'All fields are required.'),
    (N'FrmLogin.msgCredencialesInvalidas', N'Incorrect or nonexistent username and/or password.'),
    (N'FrmLogin.msgIngresoExitoso', N'Login successful.'),
    (N'FrmLogin.msgSistemaNoDisponible', N'The system is not available at this time. Please try again later.'),
    (N'frmRestore.Title', N'Integrity restore'),
    (N'frmRestore.btnCancelar.Text', N'Cancel'),
    (N'frmRestore.btnRecalcular.Text', N'Recalculate'),
    (N'frmRestore.btnRestore.Text', N'Database backup'),
    (N'frmRestore.lblIntegridadDVV.Text', N'Table integrity'),
    (N'frmRestore.lblRegistros.Text', N'Record integrity'),
    (N'frmRestore.msgIntegridadDVVCorrecta', N'Table integrity is correct.'),
    (N'frmRestore.msgIntegridadDVVFallida', N'Table integrity failed.'),
    (N'frmRestore.msgIntegridadRegistrosCorrecta', N'Record integrity is correct.'),
    (N'frmRestore.msgIntegridadRegistrosFallida', N'Record integrity failed.'),
    (N'frmRestore.msgRestauracionError', N'Error restoring the database.'),
    (N'frmRestore.msgRestauracionExitosa', N'Restore completed successfully.');

INSERT INTO dbo.TRADUCCION (
    Id_Idioma,
    Clave_Traduccion,
    Valor_Traduccion
)
SELECT
    @IdIdioma_en_US,
    origen.Clave_Traduccion,
    origen.Valor_Traduccion
FROM @Traducciones_en_US AS origen
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.TRADUCCION AS destino
    WHERE destino.Id_Idioma = @IdIdioma_en_US
      AND destino.Clave_Traduccion = origen.Clave_Traduccion
);
