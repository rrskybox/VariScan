
namespace VariScan
{
    partial class FormCollectionManager
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
            this.CollectionListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AddNewButton = new System.Windows.Forms.Button();
            this.AddCollectionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CollectionListBox
            // 
            this.CollectionListBox.BackColor = System.Drawing.Color.MediumSpringGreen;
            this.CollectionListBox.ForeColor = System.Drawing.Color.Black;
            this.CollectionListBox.FormattingEnabled = true;
            this.CollectionListBox.Location = new System.Drawing.Point(12, 38);
            this.CollectionListBox.Name = "CollectionListBox";
            this.CollectionListBox.Size = new System.Drawing.Size(121, 121);
            this.CollectionListBox.TabIndex = 0;
            this.CollectionListBox.SelectedIndexChanged += new System.EventHandler(this.CollectionListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose A Collection\r\nFrom the List";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AddNewButton
            // 
            this.AddNewButton.Location = new System.Drawing.Point(54, 206);
            this.AddNewButton.Name = "AddNewButton";
            this.AddNewButton.Size = new System.Drawing.Size(36, 22);
            this.AddNewButton.TabIndex = 2;
            this.AddNewButton.Text = "Add";
            this.AddNewButton.UseVisualStyleBackColor = true;
            this.AddNewButton.Click += new System.EventHandler(this.AddNewButton_Click);
            // 
            // AddCollectionTextBox
            // 
            this.AddCollectionTextBox.Location = new System.Drawing.Point(12, 180);
            this.AddCollectionTextBox.Name = "AddCollectionTextBox";
            this.AddCollectionTextBox.Size = new System.Drawing.Size(121, 20);
            this.AddCollectionTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(64, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "or";
            // 
            // FormCollectionManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(145, 237);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AddCollectionTextBox);
            this.Controls.Add(this.AddNewButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CollectionListBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCollectionManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox CollectionListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddNewButton;
        private System.Windows.Forms.TextBox AddCollectionTextBox;
        private System.Windows.Forms.Label label2;
    }
}