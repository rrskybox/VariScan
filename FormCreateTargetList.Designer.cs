
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
            this.CompileButton = new System.Windows.Forms.Button();
            this.FilterListBox = new System.Windows.Forms.CheckedListBox();
            this.FilterListGroup = new System.Windows.Forms.GroupBox();
            this.CreateAAVSOListButton = new System.Windows.Forms.Button();
            this.TargetListDropDown = new System.Windows.Forms.ComboBox();
            this.AddTargetButton = new System.Windows.Forms.Button();
            this.DoneButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DeleteTargetButton = new System.Windows.Forms.Button();
            this.FilterListGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImportCSVButton
            // 
            this.ImportCSVButton.BackColor = System.Drawing.Color.LightGreen;
            this.ImportCSVButton.ForeColor = System.Drawing.Color.Black;
            this.ImportCSVButton.Location = new System.Drawing.Point(7, 34);
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
            // CompileButton
            // 
            this.CompileButton.BackColor = System.Drawing.Color.LightGreen;
            this.CompileButton.Location = new System.Drawing.Point(7, 62);
            this.CompileButton.Name = "CompileButton";
            this.CompileButton.Size = new System.Drawing.Size(118, 22);
            this.CompileButton.TabIndex = 4;
            this.CompileButton.Text = "Compile List";
            this.CompileButton.UseVisualStyleBackColor = false;
            this.CompileButton.Click += new System.EventHandler(this.CompileButton_Click);
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
            this.FilterListGroup.Location = new System.Drawing.Point(131, 6);
            this.FilterListGroup.Name = "FilterListGroup";
            this.FilterListGroup.Size = new System.Drawing.Size(138, 226);
            this.FilterListGroup.TabIndex = 7;
            this.FilterListGroup.TabStop = false;
            this.FilterListGroup.Text = "Filter List";
            // 
            // CreateAAVSOListButton
            // 
            this.CreateAAVSOListButton.BackColor = System.Drawing.Color.LightGreen;
            this.CreateAAVSOListButton.ForeColor = System.Drawing.Color.Black;
            this.CreateAAVSOListButton.Location = new System.Drawing.Point(7, 6);
            this.CreateAAVSOListButton.Name = "CreateAAVSOListButton";
            this.CreateAAVSOListButton.Size = new System.Drawing.Size(118, 24);
            this.CreateAAVSOListButton.TabIndex = 9;
            this.CreateAAVSOListButton.Text = "Create AAVSO List";
            this.CreateAAVSOListButton.UseVisualStyleBackColor = false;
            this.CreateAAVSOListButton.Click += new System.EventHandler(this.CreateAAVSOTargetList_Button);
            // 
            // TargetListDropDown
            // 
            this.TargetListDropDown.FormattingEnabled = true;
            this.TargetListDropDown.Location = new System.Drawing.Point(8, 19);
            this.TargetListDropDown.Name = "TargetListDropDown";
            this.TargetListDropDown.Size = new System.Drawing.Size(106, 21);
            this.TargetListDropDown.TabIndex = 10;
            // 
            // AddTargetButton
            // 
            this.AddTargetButton.BackColor = System.Drawing.Color.LightGreen;
            this.AddTargetButton.ForeColor = System.Drawing.Color.Black;
            this.AddTargetButton.Location = new System.Drawing.Point(8, 46);
            this.AddTargetButton.Name = "AddTargetButton";
            this.AddTargetButton.Size = new System.Drawing.Size(106, 24);
            this.AddTargetButton.TabIndex = 11;
            this.AddTargetButton.Text = "Add Target";
            this.AddTargetButton.UseVisualStyleBackColor = false;
            this.AddTargetButton.Click += new System.EventHandler(this.AddTargetButton_Click);
            // 
            // DoneButton
            // 
            this.DoneButton.BackColor = System.Drawing.Color.LightGreen;
            this.DoneButton.ForeColor = System.Drawing.Color.Black;
            this.DoneButton.Location = new System.Drawing.Point(7, 208);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(118, 24);
            this.DoneButton.TabIndex = 12;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = false;
            this.DoneButton.Click += new System.EventHandler(this.FinishedButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DeleteTargetButton);
            this.groupBox1.Controls.Add(this.TargetListDropDown);
            this.groupBox1.Controls.Add(this.AddTargetButton);
            this.groupBox1.Location = new System.Drawing.Point(7, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(120, 112);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Targets";
            // 
            // DeleteTargetButton
            // 
            this.DeleteTargetButton.BackColor = System.Drawing.Color.LightGreen;
            this.DeleteTargetButton.ForeColor = System.Drawing.Color.Black;
            this.DeleteTargetButton.Location = new System.Drawing.Point(8, 76);
            this.DeleteTargetButton.Name = "DeleteTargetButton";
            this.DeleteTargetButton.Size = new System.Drawing.Size(106, 24);
            this.DeleteTargetButton.TabIndex = 14;
            this.DeleteTargetButton.Text = "Delete Target";
            this.DeleteTargetButton.UseVisualStyleBackColor = false;
            this.DeleteTargetButton.Click += new System.EventHandler(this.DeleteTargetButton_Click);
            // 
            // FormCreateTargetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(283, 243);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.CreateAAVSOListButton);
            this.Controls.Add(this.FilterListGroup);
            this.Controls.Add(this.CompileButton);
            this.Controls.Add(this.ImportCSVButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormCreateTargetList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Target List";
            this.TopMost = true;
            this.FilterListGroup.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ImportCSVButton;
        private System.Windows.Forms.OpenFileDialog OpenTargetTextFileDialog;
        private System.Windows.Forms.Button CompileButton;
        private System.Windows.Forms.CheckedListBox FilterListBox;
        private System.Windows.Forms.GroupBox FilterListGroup;
        private System.Windows.Forms.Button CreateAAVSOListButton;
        private System.Windows.Forms.ComboBox TargetListDropDown;
        private System.Windows.Forms.Button AddTargetButton;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button DeleteTargetButton;
    }
}