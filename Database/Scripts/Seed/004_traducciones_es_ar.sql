/*
Seed de traducciones base: Español Argentina.

Este script inserta solamente las traducciones que no existen.
No actualiza traducciones existentes para no pisar cambios hechos desde la aplicación por un administrador.
*/

DECLARE @IdIdioma_es_AR INT;

SELECT @IdIdioma_es_AR = Id_Idioma
FROM dbo.IDIOMA
WHERE Codigo_Idioma = N'es-AR';

IF @IdIdioma_es_AR IS NULL
BEGIN
    RAISERROR('No existe el idioma es-AR. Revisar el script 001_idiomas.sql.', 16, 1);
    RETURN;
END;

DECLARE @Traducciones_es_AR TABLE (
    Clave_Traduccion NVARCHAR(200) NOT NULL PRIMARY KEY,
    Valor_Traduccion NVARCHAR(1000) NULL
);

INSERT INTO @Traducciones_es_AR (
    Clave_Traduccion,
    Valor_Traduccion
)
VALUES
    (N'Common.btnAceptar.Text', N'Aceptar'),
    (N'Common.btnBorrar.Text', N'Borrar'),
    (N'Common.btnBuscar.Text', N'Buscar'),
    (N'Common.btnCancelar.Text', N'Cancelar'),
    (N'Common.btnEditar.Text', N'Editar'),
    (N'Common.btnGuardar.Text', N'Guardar'),
    (N'Common.btnNuevo.Text', N'Nuevo'),
    (N'Common.btnSalir.Text', N'Salir'),
    (N'Common.msgConfirmar', N'Confirmar'),
    (N'Common.msgError', N'Error'),
    (N'FrmABMRoles.Title', N'Administración de roles'),
    (N'FrmABMRoles.btnAsignar.Text', N'Asignar'),
    (N'FrmABMRoles.btnInsertar.Text', N'Insertar'),
    (N'FrmABMRoles.btnQuitar.Text', N'Quitar'),
    (N'FrmABMRoles.btnborrar.Text', N'Borrar'),
    (N'FrmABMRoles.btnlistar.Text', N'Listar'),
    (N'FrmABMRoles.btnmodificar.Text', N'Modificar'),
    (N'FrmABMRoles.lblIdRol.Text', N'Id rol'),
    (N'FrmABMRoles.lblNombreRol.Text', N'Nombre'),
    (N'FrmABMUsuarios.Title', N'Administración de usuarios'),
    (N'FrmABMUsuarios.btnInsertar.Text', N'Insertar'),
    (N'FrmABMUsuarios.btnRestaurarVersAnt.Text', N'Restaurar versión'),
    (N'FrmABMUsuarios.btnborrar.Text', N'Borrar'),
    (N'FrmABMUsuarios.btnlistar.Text', N'Listar'),
    (N'FrmABMUsuarios.btnmodificar.Text', N'Modificar'),
    (N'FrmABMUsuarios.groupBox1.Text', N'Recuperar estados previos'),
    (N'FrmABMUsuarios.lblClave.Text', N'Clave'),
    (N'FrmABMUsuarios.lblD.Text', N'ID'),
    (N'FrmABMUsuarios.lblIdUsuarioSeleccionado.Text', N'Id seleccionado'),
    (N'FrmABMUsuarios.lblNombre.Text', N'Nombre'),
    (N'FrmABMUsuarios.lblPerfil.Text', N'Perfil'),
    (N'FrmABMUsuarios.lblUsuarioSeleccionado.Text', N'Usuario seleccionado'),
    (N'FrmABMUsuarios.msgClaveNoCumple', N'La clave no cumple con los requisitos. Debe tener al menos 6 caracteres, una letra mayúscula y un número.'),
    (N'FrmApp.Title', N'Sistema de gestión administrativa'),
    (N'FrmApp.archivoToolStripMenuItem.Text', N'Archivo'),
    (N'FrmApp.bitácoraToolStripMenuItem.Text', N'Bitácora'),
    (N'FrmApp.cerrarSesiónToolStripMenuItem.Text', N'Cerrar sesión'),
    (N'FrmApp.clientesToolStripMenuItem.Text', N'Nueva venta'),
    (N'FrmApp.gestiónToolStripMenuItem.Text', N'Gestión'),
    (N'FrmApp.historialToolStripMenuItem.Text', N'Historial'),
    (N'FrmApp.idiomasToolStripMenuItem.Text', N'Idiomas'),
    (N'FrmApp.msgConfirmarCerrarSesion', N'¿Realmente desea cerrar sesión?'),
    (N'FrmApp.msgConfirmarSalir', N'¿Realmente desea cerrar sesión y salir?'),
    (N'FrmApp.msgErrorAbrirIdiomas', N'No se pudo abrir el formulario de idiomas.'),
    (N'FrmApp.msgTituloConfirmarSalida', N'Confirmar salida'),
    (N'FrmApp.perfilesToolStripMenuItem.Text', N'Perfiles'),
    (N'FrmApp.productosToolStripMenuItem.Text', N'Clientes'),
    (N'FrmApp.productosToolStripMenuItem1.Text', N'Productos'),
    (N'FrmApp.salirToolStripMenuItem.Text', N'Salir'),
    (N'FrmApp.seguridadToolStripMenuItem.Text', N'Seguridad'),
    (N'FrmApp.usuariosToolStripMenuItem.Text', N'Usuarios'),
    (N'FrmBitacora.Title', N'Bitácora'),
    (N'FrmBitacora.btnBuscar.Text', N'Buscar'),
    (N'FrmBitacora.gbFiltros.Text', N'Filtros'),
    (N'FrmBitacora.lblFechaDesde.Text', N'Fecha desde'),
    (N'FrmBitacora.lblFechaHasta.Text', N'Fecha hasta'),
    (N'FrmBitacora.lblUsuario.Text', N'Usuario'),
    (N'FrmIdioma.Title', N'Gestión de idiomas'),
    (N'FrmIdioma.btnActivar.Text', N'Activar'),
    (N'FrmIdioma.btnBorrar.Text', N'Borrar'),
    (N'FrmIdioma.btnCargarTraducciones.Text', N'Cargar traducciones'),
    (N'FrmIdioma.btnDesactivar.Text', N'Desactivar'),
    (N'FrmIdioma.btnEditar.Text', N'Editar'),
    (N'FrmIdioma.btnGuardarTraducciones.Text', N'Guardar traducciones'),
    (N'FrmIdioma.btnNuevo.Text', N'Nuevo'),
    (N'FrmIdioma.btnSetDefault.Text', N'Predeterminado'),
    (N'FrmIdioma.dgvIdiomas.Codigo.HeaderText', N'Código'),
    (N'FrmIdioma.dgvIdiomas.Habilitado.HeaderText', N'Habilitado'),
    (N'FrmIdioma.dgvIdiomas.EsDefault.HeaderText', N'Predeterminado'),
    (N'FrmIdioma.dgvIdiomas.Id_Idioma.HeaderText', N'Id'),
    (N'FrmIdioma.dgvIdiomas.Nombre.HeaderText', N'Nombre'),
    (N'FrmIdioma.dgvTraducciones.Clave.HeaderText', N'Clave'),
    (N'FrmIdioma.dgvTraducciones.Id_Idioma.HeaderText', N'Idioma'),
    (N'FrmIdioma.dgvTraducciones.Id_Traduccion.HeaderText', N'Id'),
    (N'FrmIdioma.dgvTraducciones.Valor.HeaderText', N'Valor'),
    (N'FrmIdioma.msgAccesoDenegado', N'Acceso denegado. Requiere rol administrador.'),
    (N'FrmIdioma.msgBorrarIdioma', N'¿Borrar idioma?'),
    (N'FrmIdioma.msgConfirmar', N'Confirmar'),
    (N'FrmIdioma.msgTraduccionesGuardadas', N'Traducciones guardadas.'),
    (N'FrmIdiomaEditor.Title', N'Idioma'),
    (N'FrmIdiomaEditor.btnCancel.Text', N'Cancelar'),
    (N'FrmIdiomaEditor.btnOk.Text', N'OK'),
    (N'FrmIdiomaEditor.chkHabilitado.Text', N'Habilitado'),
    (N'FrmIdiomaEditor.lblCodigo.Text', N'Código:'),
    (N'FrmIdiomaEditor.lblNombre.Text', N'Nombre:'),
    (N'FrmLogin.Title', N'Login'),
    (N'FrmLogin.btnIngresar.Text', N'Ingresar'),
    (N'FrmLogin.btnSalir.Text', N'Salir'),
    (N'FrmLogin.cmbIdiomasLogin.Text', N'Idioma'),
    (N'FrmLogin.lblClave.Text', N'Clave'),
    (N'FrmLogin.lblUsuario.Text', N'Usuario'),
    (N'FrmLogin.msgCamposRequeridos', N'Debe completar todos los campos.'),
    (N'FrmLogin.msgCredencialesInvalidas', N'Usuario y/o clave incorrecta o inexistente.'),
    (N'FrmLogin.msgIngresoExitoso', N'Ingreso exitoso.'),
    (N'FrmLogin.msgSistemaNoDisponible', N'El sistema no está disponible en este momento. Por favor, intente más tarde.'),
    (N'frmRestore.Title', N'Restauración de integridad'),
    (N'frmRestore.btnCancelar.Text', N'Cancelar'),
    (N'frmRestore.btnRecalcular.Text', N'Recalcular'),
    (N'frmRestore.btnRestore.Text', N'Backup base de datos'),
    (N'frmRestore.lblIntegridadDVV.Text', N'Integridad de tabla'),
    (N'frmRestore.lblRegistros.Text', N'Integridad de registros'),
    (N'frmRestore.msgIntegridadDVVCorrecta', N'Integridad correcta de tabla.'),
    (N'frmRestore.msgIntegridadDVVFallida', N'Integridad fallida de tabla.'),
    (N'frmRestore.msgIntegridadRegistrosCorrecta', N'Integridad correcta de registros.'),
    (N'frmRestore.msgIntegridadRegistrosFallida', N'Integridad fallida de registros.'),
    (N'frmRestore.msgRestauracionError', N'Error al restaurar la base de datos.'),
    (N'frmRestore.msgRestauracionExitosa', N'Restauración completada con éxito.');

INSERT INTO dbo.TRADUCCION (
    Id_Idioma,
    Clave_Traduccion,
    Valor_Traduccion
)
SELECT
    @IdIdioma_es_AR,
    origen.Clave_Traduccion,
    origen.Valor_Traduccion
FROM @Traducciones_es_AR AS origen
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.TRADUCCION AS destino
    WHERE destino.Id_Idioma = @IdIdioma_es_AR
      AND destino.Clave_Traduccion = origen.Clave_Traduccion
);
