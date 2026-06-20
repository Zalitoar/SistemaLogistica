namespace IngSoft
{
    partial class FrmABMRoles
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
            this.tvRoles = new System.Windows.Forms.TreeView();
            this.btnlistar = new System.Windows.Forms.Button();
            this.lblIdRol = new System.Windows.Forms.Label();
            this.txtIdRol = new System.Windows.Forms.TextBox();
            this.txtNombreRol = new System.Windows.Forms.TextBox();
            this.lblNombreRol = new System.Windows.Forms.Label();
            this.btnInsertar = new System.Windows.Forms.Button();
            this.btnmodificar = new System.Windows.Forms.Button();
            this.btnborrar = new System.Windows.Forms.Button();
            this.cmbDisponibles = new System.Windows.Forms.ComboBox();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.btnAsignar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tvRoles
            // 
            this.tvRoles.Location = new System.Drawing.Point(47, 71);
            this.tvRoles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tvRoles.Name = "tvRoles";
            this.tvRoles.Size = new System.Drawing.Size(343, 378);
            this.tvRoles.TabIndex = 0;
            this.tvRoles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvRoles_AfterSelect_1);
            // 
            // btnlistar
            // 
            this.btnlistar.Location = new System.Drawing.Point(47, 36);
            this.btnlistar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnlistar.Name = "btnlistar";
            this.btnlistar.Size = new System.Drawing.Size(343, 28);
            this.btnlistar.TabIndex = 1;
            this.btnlistar.Text = "Listar";
            this.btnlistar.UseVisualStyleBackColor = true;
            this.btnlistar.Click += new System.EventHandler(this.btnlistar_Click);
            // 
            // lblIdRol
            // 
            this.lblIdRol.AutoSize = true;
            this.lblIdRol.Location = new System.Drawing.Point(443, 96);
            this.lblIdRol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIdRol.Name = "lblIdRol";
            this.lblIdRol.Size = new System.Drawing.Size(42, 16);
            this.lblIdRol.TabIndex = 2;
            this.lblIdRol.Text = "Id Rol";
            // 
            // txtIdRol
            // 
            this.txtIdRol.Location = new System.Drawing.Point(497, 92);
            this.txtIdRol.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtIdRol.Name = "txtIdRol";
            this.txtIdRol.Size = new System.Drawing.Size(132, 22);
            this.txtIdRol.TabIndex = 3;
            // 
            // txtNombreRol
            // 
            this.txtNombreRol.Location = new System.Drawing.Point(497, 142);
            this.txtNombreRol.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNombreRol.Name = "txtNombreRol";
            this.txtNombreRol.Size = new System.Drawing.Size(132, 22);
            this.txtNombreRol.TabIndex = 5;
            // 
            // lblNombreRol
            // 
            this.lblNombreRol.AutoSize = true;
            this.lblNombreRol.Location = new System.Drawing.Point(443, 145);
            this.lblNombreRol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNombreRol.Name = "lblNombreRol";
            this.lblNombreRol.Size = new System.Drawing.Size(56, 16);
            this.lblNombreRol.TabIndex = 4;
            this.lblNombreRol.Text = "Nombre";
            // 
            // btnInsertar
            // 
            this.btnInsertar.Location = new System.Drawing.Point(447, 203);
            this.btnInsertar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInsertar.Name = "btnInsertar";
            this.btnInsertar.Size = new System.Drawing.Size(184, 44);
            this.btnInsertar.TabIndex = 6;
            this.btnInsertar.Text = "Insertar";
            this.btnInsertar.UseVisualStyleBackColor = true;
            this.btnInsertar.Click += new System.EventHandler(this.btnInsertar_Click);
            // 
            // btnmodificar
            // 
            this.btnmodificar.Location = new System.Drawing.Point(447, 254);
            this.btnmodificar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnmodificar.Name = "btnmodificar";
            this.btnmodificar.Size = new System.Drawing.Size(184, 44);
            this.btnmodificar.TabIndex = 7;
            this.btnmodificar.Text = "Modificar";
            this.btnmodificar.UseVisualStyleBackColor = true;
            this.btnmodificar.Click += new System.EventHandler(this.btnmodificar_Click);
            // 
            // btnborrar
            // 
            this.btnborrar.Location = new System.Drawing.Point(447, 305);
            this.btnborrar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnborrar.Name = "btnborrar";
            this.btnborrar.Size = new System.Drawing.Size(184, 44);
            this.btnborrar.TabIndex = 8;
            this.btnborrar.Text = "Borrar";
            this.btnborrar.UseVisualStyleBackColor = true;
            this.btnborrar.Click += new System.EventHandler(this.btnborrar_Click);
            // 
            // cmbDisponibles
            // 
            this.cmbDisponibles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDisponibles.FormattingEnabled = true;
            this.cmbDisponibles.Location = new System.Drawing.Point(744, 96);
            this.cmbDisponibles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbDisponibles.Name = "cmbDisponibles";
            this.cmbDisponibles.Size = new System.Drawing.Size(160, 24);
            this.cmbDisponibles.TabIndex = 9;
            // 
            // btnQuitar
            // 
            this.btnQuitar.Location = new System.Drawing.Point(837, 145);
            this.btnQuitar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnQuitar.Name = "btnQuitar";
            this.btnQuitar.Size = new System.Drawing.Size(100, 28);
            this.btnQuitar.TabIndex = 11;
            this.btnQuitar.Text = "Quitar";
            this.btnQuitar.UseVisualStyleBackColor = true;
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            // 
            // btnAsignar
            // 
            this.btnAsignar.Location = new System.Drawing.Point(729, 145);
            this.btnAsignar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAsignar.Name = "btnAsignar";
            this.btnAsignar.Size = new System.Drawing.Size(100, 28);
            this.btnAsignar.TabIndex = 10;
            this.btnAsignar.Text = "Asignar";
            this.btnAsignar.UseVisualStyleBackColor = true;
            this.btnAsignar.Click += new System.EventHandler(this.btnAsignar_Click);
            // 
            // FrmABMRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.btnQuitar);
            this.Controls.Add(this.btnAsignar);
            this.Controls.Add(this.cmbDisponibles);
            this.Controls.Add(this.btnborrar);
            this.Controls.Add(this.btnmodificar);
            this.Controls.Add(this.btnInsertar);
            this.Controls.Add(this.txtNombreRol);
            this.Controls.Add(this.lblNombreRol);
            this.Controls.Add(this.txtIdRol);
            this.Controls.Add(this.lblIdRol);
            this.Controls.Add(this.btnlistar);
            this.Controls.Add(this.tvRoles);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmABMRoles";
            this.Text = "FrmABMRoles";
            this.Load += new System.EventHandler(this.FrmABMRoles_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvRoles;
        private System.Windows.Forms.Button btnlistar;
        private System.Windows.Forms.Label lblIdRol;
        private System.Windows.Forms.TextBox txtIdRol;
        private System.Windows.Forms.TextBox txtNombreRol;
        private System.Windows.Forms.Label lblNombreRol;
        private System.Windows.Forms.Button btnInsertar;
        private System.Windows.Forms.Button btnmodificar;
        private System.Windows.Forms.Button btnborrar;
        private System.Windows.Forms.ComboBox cmbDisponibles;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.Button btnAsignar;
    }
}