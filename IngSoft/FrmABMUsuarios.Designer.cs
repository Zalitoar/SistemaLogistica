namespace IngSoft
{
    partial class FrmABMUsuarios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.btnlistar = new System.Windows.Forms.Button();
            this.btnborrar = new System.Windows.Forms.Button();
            this.btnmodificar = new System.Windows.Forms.Button();
            this.btnInsertar = new System.Windows.Forms.Button();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.txtIdUsuario = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblD = new System.Windows.Forms.Label();
            this.txtPerfil = new System.Windows.Forms.TextBox();
            this.lblPerfil = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblClave = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblUsuarioSeleccionado = new System.Windows.Forms.Label();
            this.dgvVerisionesAnteriores = new System.Windows.Forms.DataGridView();
            this.btnRestaurarVersAnt = new System.Windows.Forms.Button();
            this.lblIdUsuarioSeleccionado = new System.Windows.Forms.Label();
            this.lblid = new System.Windows.Forms.Label();
            this.lblususel = new System.Windows.Forms.Label();
            this.lblid_usu_sel = new System.Windows.Forms.Label();
            this.lblnombre_sel = new System.Windows.Forms.Label();
            this.lblclave_usu_sel = new System.Windows.Forms.Label();
            this.lblborr_usu_sel = new System.Windows.Forms.Label();
            this.lblid_rol_sel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerisionesAnteriores)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Location = new System.Drawing.Point(26, 124);
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersWidth = 51;
            this.dgvUsuarios.Size = new System.Drawing.Size(644, 185);
            this.dgvUsuarios.TabIndex = 17;
            this.dgvUsuarios.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUsuarios_CellClick);
            // 
            // btnlistar
            // 
            this.btnlistar.Location = new System.Drawing.Point(245, 96);
            this.btnlistar.Name = "btnlistar";
            this.btnlistar.Size = new System.Drawing.Size(112, 23);
            this.btnlistar.TabIndex = 7;
            this.btnlistar.Text = "Listar";
            this.btnlistar.UseVisualStyleBackColor = true;
            this.btnlistar.Click += new System.EventHandler(this.btnlistar_Click);
            // 
            // btnborrar
            // 
            this.btnborrar.Location = new System.Drawing.Point(245, 67);
            this.btnborrar.Name = "btnborrar";
            this.btnborrar.Size = new System.Drawing.Size(112, 23);
            this.btnborrar.TabIndex = 6;
            this.btnborrar.Text = "Borrar";
            this.btnborrar.UseVisualStyleBackColor = true;
            this.btnborrar.Click += new System.EventHandler(this.btnborrar_Click);
            // 
            // btnmodificar
            // 
            this.btnmodificar.Location = new System.Drawing.Point(245, 38);
            this.btnmodificar.Name = "btnmodificar";
            this.btnmodificar.Size = new System.Drawing.Size(112, 23);
            this.btnmodificar.TabIndex = 5;
            this.btnmodificar.Text = "Modificar";
            this.btnmodificar.UseVisualStyleBackColor = true;
            this.btnmodificar.Click += new System.EventHandler(this.btnmodificar_Click);
            // 
            // btnInsertar
            // 
            this.btnInsertar.Location = new System.Drawing.Point(245, 9);
            this.btnInsertar.Name = "btnInsertar";
            this.btnInsertar.Size = new System.Drawing.Size(112, 23);
            this.btnInsertar.TabIndex = 4;
            this.btnInsertar.Text = "Insertar";
            this.btnInsertar.UseVisualStyleBackColor = true;
            this.btnInsertar.Click += new System.EventHandler(this.btnInsertar_Click);
            // 
            // txtNombreUsuario
            // 
            this.txtNombreUsuario.Location = new System.Drawing.Point(79, 46);
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(100, 20);
            this.txtNombreUsuario.TabIndex = 1;
            // 
            // txtIdUsuario
            // 
            this.txtIdUsuario.Location = new System.Drawing.Point(79, 19);
            this.txtIdUsuario.Name = "txtIdUsuario";
            this.txtIdUsuario.Size = new System.Drawing.Size(100, 20);
            this.txtIdUsuario.TabIndex = 0;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(23, 49);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(44, 13);
            this.lblNombre.TabIndex = 10;
            this.lblNombre.Text = "Nombre";
            // 
            // lblD
            // 
            this.lblD.AutoSize = true;
            this.lblD.Location = new System.Drawing.Point(23, 19);
            this.lblD.Name = "lblD";
            this.lblD.Size = new System.Drawing.Size(18, 13);
            this.lblD.TabIndex = 9;
            this.lblD.Text = "ID";
            // 
            // txtPerfil
            // 
            this.txtPerfil.Location = new System.Drawing.Point(79, 72);
            this.txtPerfil.Name = "txtPerfil";
            this.txtPerfil.Size = new System.Drawing.Size(100, 20);
            this.txtPerfil.TabIndex = 2;
            // 
            // lblPerfil
            // 
            this.lblPerfil.AutoSize = true;
            this.lblPerfil.Location = new System.Drawing.Point(23, 75);
            this.lblPerfil.Name = "lblPerfil";
            this.lblPerfil.Size = new System.Drawing.Size(30, 13);
            this.lblPerfil.TabIndex = 18;
            this.lblPerfil.Text = "Perfil";
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(79, 98);
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(100, 20);
            this.txtClave.TabIndex = 3;
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Location = new System.Drawing.Point(23, 101);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(34, 13);
            this.lblClave.TabIndex = 20;
            this.lblClave.Text = "Clave";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblid_rol_sel);
            this.groupBox1.Controls.Add(this.lblborr_usu_sel);
            this.groupBox1.Controls.Add(this.lblclave_usu_sel);
            this.groupBox1.Controls.Add(this.lblnombre_sel);
            this.groupBox1.Controls.Add(this.lblid_usu_sel);
            this.groupBox1.Controls.Add(this.lblid);
            this.groupBox1.Controls.Add(this.lblususel);
            this.groupBox1.Controls.Add(this.lblIdUsuarioSeleccionado);
            this.groupBox1.Controls.Add(this.btnRestaurarVersAnt);
            this.groupBox1.Controls.Add(this.dgvVerisionesAnteriores);
            this.groupBox1.Controls.Add(this.lblUsuarioSeleccionado);
            this.groupBox1.Location = new System.Drawing.Point(26, 330);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(644, 292);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recuperar Estados Previos";
            // 
            // lblUsuarioSeleccionado
            // 
            this.lblUsuarioSeleccionado.AutoSize = true;
            this.lblUsuarioSeleccionado.Location = new System.Drawing.Point(6, 44);
            this.lblUsuarioSeleccionado.Name = "lblUsuarioSeleccionado";
            this.lblUsuarioSeleccionado.Size = new System.Drawing.Size(111, 13);
            this.lblUsuarioSeleccionado.TabIndex = 0;
            this.lblUsuarioSeleccionado.Text = "Usuario Seleccionado";
            // 
            // dgvVerisionesAnteriores
            // 
            this.dgvVerisionesAnteriores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVerisionesAnteriores.Location = new System.Drawing.Point(9, 63);
            this.dgvVerisionesAnteriores.Name = "dgvVerisionesAnteriores";
            this.dgvVerisionesAnteriores.Size = new System.Drawing.Size(629, 123);
            this.dgvVerisionesAnteriores.TabIndex = 1;
            this.dgvVerisionesAnteriores.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVerisionesAnteriores_CellClick);
            // 
            // btnRestaurarVersAnt
            // 
            this.btnRestaurarVersAnt.Location = new System.Drawing.Point(508, 192);
            this.btnRestaurarVersAnt.Name = "btnRestaurarVersAnt";
            this.btnRestaurarVersAnt.Size = new System.Drawing.Size(130, 38);
            this.btnRestaurarVersAnt.TabIndex = 22;
            this.btnRestaurarVersAnt.Text = "Restaurar Versión";
            this.btnRestaurarVersAnt.UseVisualStyleBackColor = true;
            this.btnRestaurarVersAnt.Click += new System.EventHandler(this.btnRestaurarVersAnt_Click);
            // 
            // lblIdUsuarioSeleccionado
            // 
            this.lblIdUsuarioSeleccionado.AutoSize = true;
            this.lblIdUsuarioSeleccionado.Location = new System.Drawing.Point(6, 20);
            this.lblIdUsuarioSeleccionado.Name = "lblIdUsuarioSeleccionado";
            this.lblIdUsuarioSeleccionado.Size = new System.Drawing.Size(84, 13);
            this.lblIdUsuarioSeleccionado.TabIndex = 23;
            this.lblIdUsuarioSeleccionado.Text = "Id Seleccionado";
            // 
            // lblid
            // 
            this.lblid.AutoSize = true;
            this.lblid.Location = new System.Drawing.Point(123, 20);
            this.lblid.Name = "lblid";
            this.lblid.Size = new System.Drawing.Size(16, 13);
            this.lblid.TabIndex = 25;
            this.lblid.Text = "...";
            // 
            // lblususel
            // 
            this.lblususel.AutoSize = true;
            this.lblususel.Location = new System.Drawing.Point(123, 44);
            this.lblususel.Name = "lblususel";
            this.lblususel.Size = new System.Drawing.Size(16, 13);
            this.lblususel.TabIndex = 24;
            this.lblususel.Text = "...";
            // 
            // lblid_usu_sel
            // 
            this.lblid_usu_sel.AutoSize = true;
            this.lblid_usu_sel.Location = new System.Drawing.Point(17, 192);
            this.lblid_usu_sel.Name = "lblid_usu_sel";
            this.lblid_usu_sel.Size = new System.Drawing.Size(35, 13);
            this.lblid_usu_sel.TabIndex = 26;
            this.lblid_usu_sel.Text = "label1";
            // 
            // lblnombre_sel
            // 
            this.lblnombre_sel.AutoSize = true;
            this.lblnombre_sel.Location = new System.Drawing.Point(17, 205);
            this.lblnombre_sel.Name = "lblnombre_sel";
            this.lblnombre_sel.Size = new System.Drawing.Size(35, 13);
            this.lblnombre_sel.TabIndex = 27;
            this.lblnombre_sel.Text = "label1";
            // 
            // lblclave_usu_sel
            // 
            this.lblclave_usu_sel.AutoSize = true;
            this.lblclave_usu_sel.Location = new System.Drawing.Point(17, 218);
            this.lblclave_usu_sel.Name = "lblclave_usu_sel";
            this.lblclave_usu_sel.Size = new System.Drawing.Size(35, 13);
            this.lblclave_usu_sel.TabIndex = 28;
            this.lblclave_usu_sel.Text = "label2";
            // 
            // lblborr_usu_sel
            // 
            this.lblborr_usu_sel.AutoSize = true;
            this.lblborr_usu_sel.Location = new System.Drawing.Point(17, 231);
            this.lblborr_usu_sel.Name = "lblborr_usu_sel";
            this.lblborr_usu_sel.Size = new System.Drawing.Size(35, 13);
            this.lblborr_usu_sel.TabIndex = 29;
            this.lblborr_usu_sel.Text = "label3";
            // 
            // lblid_rol_sel
            // 
            this.lblid_rol_sel.AutoSize = true;
            this.lblid_rol_sel.Location = new System.Drawing.Point(17, 244);
            this.lblid_rol_sel.Name = "lblid_rol_sel";
            this.lblid_rol_sel.Size = new System.Drawing.Size(35, 13);
            this.lblid_rol_sel.TabIndex = 30;
            this.lblid_rol_sel.Text = "label4";
            // 
            // FrmABMUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(824, 667);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.lblClave);
            this.Controls.Add(this.txtPerfil);
            this.Controls.Add(this.lblPerfil);
            this.Controls.Add(this.dgvUsuarios);
            this.Controls.Add(this.btnlistar);
            this.Controls.Add(this.btnborrar);
            this.Controls.Add(this.btnmodificar);
            this.Controls.Add(this.btnInsertar);
            this.Controls.Add(this.txtNombreUsuario);
            this.Controls.Add(this.txtIdUsuario);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblD);
            this.Name = "FrmABMUsuarios";
            this.Text = "ABM Usuarios";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmABMUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerisionesAnteriores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.Button btnlistar;
        private System.Windows.Forms.Button btnborrar;
        private System.Windows.Forms.Button btnmodificar;
        private System.Windows.Forms.Button btnInsertar;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.TextBox txtIdUsuario;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblD;
        private System.Windows.Forms.TextBox txtPerfil;
        private System.Windows.Forms.Label lblPerfil;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvVerisionesAnteriores;
        private System.Windows.Forms.Label lblUsuarioSeleccionado;
        private System.Windows.Forms.Button btnRestaurarVersAnt;
        private System.Windows.Forms.Label lblIdUsuarioSeleccionado;
        private System.Windows.Forms.Label lblid;
        private System.Windows.Forms.Label lblususel;
        private System.Windows.Forms.Label lblid_usu_sel;
        private System.Windows.Forms.Label lblid_rol_sel;
        private System.Windows.Forms.Label lblborr_usu_sel;
        private System.Windows.Forms.Label lblclave_usu_sel;
        private System.Windows.Forms.Label lblnombre_sel;
    }
}