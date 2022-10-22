
namespace VariScan
{
    partial class FormSampleCatalog
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
            this.imageBankGridView = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imageBankGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBankGridView
            // 
            this.imageBankGridView.BackgroundColor = System.Drawing.Color.Azure;
            this.imageBankGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.imageBankGridView.Location = new System.Drawing.Point(4, 12);
            this.imageBankGridView.Name = "imageBankGridView";
            this.imageBankGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.imageBankGridView.Size = new System.Drawing.Size(976, 467);
            this.imageBankGridView.TabIndex = 0;
            // 
            // Select
            // 
            this.Select.BackColor = System.Drawing.Color.PaleGreen;
            this.Select.ForeColor = System.Drawing.Color.Black;
            this.Select.Location = new System.Drawing.Point(470, 491);
            this.Select.Name = "Select";
            this.Select.Size = new System.Drawing.Size(94, 23);
            this.Select.TabIndex = 1;
            this.Select.Text = "Select";
            this.Select.UseVisualStyleBackColor = false;
            this.Select.Click += new System.EventHandler(this.Select_Click);
            // 
            // FormSampleCatalog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(992, 526);
            this.Controls.Add(this.Select);
            this.Controls.Add(this.imageBankGridView);
            this.Name = "FormSampleCatalog";
            this.Text = "Session Map";
            ((System.ComponentModel.ISupportInitialize)(this.imageBankGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView imageBankGridView;
        private System.Windows.Forms.Button Select;
    }
}