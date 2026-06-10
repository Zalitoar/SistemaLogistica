namespace IngSoft
{
    partial class frmRestore
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
            this.dgvRegistros = new System.Windows.Forms.DataGridView();
            this.lblIntegridadDVV = new System.Windows.Forms.Label();
            this.lblRegistros = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnRecalcular = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRegistros
            // 
            this.dgvRegistros.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRegistros.Location = new System.Drawing.Point(50, 112);
            this.dgvRegistros.Name = "dgvRegistros";
            this.dgvRegistros.Size = new System.Drawing.Size(907, 150);
            this.dgvRegistros.TabIndex = 0;
            // 
            // lblIntegridadDVV
            // 
            this.lblIntegridadDVV.AutoSize = true;
            this.lblIntegridadDVV.Location = new System.Drawing.Point(47, 58);
            this.lblIntegridadDVV.Name = "lblIntegridadDVV";
            this.lblIntegridadDVV.Size = new System.Drawing.Size(35, 13);
            this.lblIntegridadDVV.TabIndex = 1;
            this.lblIntegridadDVV.Text = "label1";
            // 
            // lblRegistros
            // 
            this.lblRegistros.AutoSize = true;
            this.lblRegistros.Location = new System.Drawing.Point(47, 96);
            this.lblRegistros.Name = "lblRegistros";
            this.lblRegistros.Size = new System.Drawing.Size(35, 13);
            this.lblRegistros.TabIndex = 2;
            this.lblRegistros.Text = "label1";
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(225, 297);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(174, 60);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "Backup Base de Datos";
            this.btnRestore.UseVisualStyleBackColor = true;
            // 
            // btnRecalcular
            // 
            this.btnRecalcular.Location = new System.Drawing.Point(414, 297);
            this.btnRecalcular.Name = "btnRecalcular";
            this.btnRecalcular.Size = new System.Drawing.Size(174, 60);
            this.btnRecalcular.TabIndex = 4;
            this.btnRecalcular.Text = "Recalcular";
            this.btnRecalcular.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(606, 297);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(174, 60);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // frmRestore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1044, 450);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnRecalcular);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.lblRegistros);
            this.Controls.Add(this.lblIntegridadDVV);
            this.Controls.Add(this.dgvRegistros);
            this.Name = "frmRestore";
            this.Text = "frmRestore";
            this.Load += new System.EventHandler(this.frmRestore_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRegistros;
        private System.Windows.Forms.Label lblIntegridadDVV;
        private System.Windows.Forms.Label lblRegistros;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnRecalcular;
        private System.Windows.Forms.Button btnCancelar;
    }
}