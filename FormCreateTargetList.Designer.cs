
namespace VariScan
{
    partial class FormCreateTargetList
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
            this.ImportCSVButton = new System.Windows.Forms.Button();
            this.OpenTargetTextFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.DoneButton = new System.Windows.Forms.Button();
            this.FilterListBox = new System.Windows.Forms.CheckedListBox();
            this.FilterListGroup = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.FilterListGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImportCSVButton
            // 
            this.ImportCSVButton.BackColor = System.Drawing.Color.LightGreen;
            this.ImportCSVButton.ForeColor = System.Drawing.Color.Black;
            this.ImportCSVButton.Location = new System.Drawing.Point(22, 45);
            this.ImportCSVButton.Name = "ImportCSVButton";
            this.ImportCSVButton.Size = new System.Drawing.Size(118, 24);
            this.ImportCSVButton.TabIndex = 3;
            this.ImportCSVButton.Text = "Convert Target List";
            this.ImportCSVButton.UseVisualStyleBackColor = false;
            this.ImportCSVButton.Click += new System.EventHandler(this.ImportCSVButton_Click);
            // 
            // OpenTargetTextFileDialog
            // 
            this.OpenTargetTextFileDialog.FileName = "  ";
            // 
            // DoneButton
            // 
            this.DoneButton.Location = new System.Drawing.Point(36, 323);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(85, 26);
            this.DoneButton.TabIndex = 4;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = true;
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // FilterListBox
            // 
            this.FilterListBox.BackColor = System.Drawing.Color.LightCyan;
            this.FilterListBox.ForeColor = System.Drawing.Color.Black;
            this.FilterListBox.FormattingEnabled = true;
            this.FilterListBox.Location = new System.Drawing.Point(9, 19);
            this.FilterListBox.Name = "FilterListBox";
            this.FilterListBox.Size = new System.Drawing.Size(118, 199);
            this.FilterListBox.TabIndex = 6;
            // 
            // FilterListGroup
            // 
            this.FilterListGroup.Controls.Add(this.FilterListBox);
            this.FilterListGroup.ForeColor = System.Drawing.Color.White;
            this.FilterListGroup.Location = new System.Drawing.Point(13, 78);
            this.FilterListGroup.Name = "FilterListGroup";
            this.FilterListGroup.Size = new System.Drawing.Size(138, 236);
            this.FilterListGroup.TabIndex = 7;
            this.FilterListGroup.TabStop = false;
            this.FilterListGroup.Text = "Filter List";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightGreen;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(22, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 24);
            this.button1.TabIndex = 9;
            this.button1.Text = "Create Target List";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormCreateTargetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(167, 361);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.FilterListGroup);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.ImportCSVButton);
            this.Name = "FormCreateTargetList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Target List";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormCreateTargetList_Load);
            this.FilterListGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ImportCSVButton;
        private System.Windows.Forms.OpenFileDialog OpenTargetTextFileDialog;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.CheckedListBox FilterListBox;
        private System.Windows.Forms.GroupBox FilterListGroup;
        private System.Windows.Forms.Button button1;
    }
}