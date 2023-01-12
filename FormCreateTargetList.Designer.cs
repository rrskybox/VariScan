
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
            this.CreateAAVSOListButton = new System.Windows.Forms.Button();
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
            this.ImportCSVButton.Text = "Import CSV List";
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
            // CreateAAVSOListButton
            // 
            this.CreateAAVSOListButton.BackColor = System.Drawing.Color.LightGreen;
            this.CreateAAVSOListButton.ForeColor = System.Drawing.Color.Black;
            this.CreateAAVSOListButton.Location = new System.Drawing.Point(22, 12);
            this.CreateAAVSOListButton.Name = "CreateAAVSOListButton";
            this.CreateAAVSOListButton.Size = new System.Drawing.Size(118, 24);
            this.CreateAAVSOListButton.TabIndex = 9;
            this.CreateAAVSOListButton.Text = "Create AAVSO List";
            this.CreateAAVSOListButton.UseVisualStyleBackColor = false;
            this.CreateAAVSOListButton.Click += new System.EventHandler(this.CreateAAVSOTargetList_Button);
            // 
            // FormCreateTargetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(167, 361);
            this.Controls.Add(this.CreateAAVSOListButton);
            this.Controls.Add(this.FilterListGroup);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.ImportCSVButton);
            this.Name = "FormCreateTargetList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Target List";
            this.TopMost = true;
            this.FilterListGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ImportCSVButton;
        private System.Windows.Forms.OpenFileDialog OpenTargetTextFileDialog;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.CheckedListBox FilterListBox;
        private System.Windows.Forms.GroupBox FilterListGroup;
        private System.Windows.Forms.Button CreateAAVSOListButton;
    }
}