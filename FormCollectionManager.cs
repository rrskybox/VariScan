// --------------------------------------------------------------------------------
// VariScan module
//
// Description:	
//
// Environment:  Windows 10 executable, 32 and 64 bit
//
// Usage:        TBD
//
// Author:		(REM) Rick McAlister, rrskybox@yahoo.com
//
// Edit Log:     Rev 1.0 Initial Version
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 
// ---------------------------------------------------------------------------------
//

using System;
using System.IO;
using System.Windows.Forms;

namespace VariScan
{
    public partial class FormCollectionManager : Form
    {
        public FormCollectionManager()
        {
            InitializeComponent();
            //fill Collection list
            CollectionListBox.Items.Clear();
            foreach (var c in CollectionManagement.ListCollections())
                CollectionListBox.Items.Add(c);
            return;
        }

        private void CollectionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tgtListPath = CollectionManagement.OpenCollection(CollectionListBox.SelectedItem.ToString());
            Form ctForm = new FormCreateTargetList();
            ctForm.ShowDialog();
            //Upon return
            this.Close();
            return;
        }

        private void AddNewButton_Click(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            //Add new folder for Collection
            DialogResult dr = MessageBox.Show("Camera and Filter Wheel must be powered on and able to connect.", "Check Image Train", MessageBoxButtons.OKCancel);
            if ((dr == DialogResult.OK) && (AddCollectionTextBox.Text.Length > 0))
            {
                string choice = AddCollectionTextBox.Text;
                CollectionManagement.CreateCollection(choice);
                CollectionManagement.OpenCollection(choice);
            }
            if (!File.Exists(cfg.TargetListPath))
            {
                Form ctForm = new FormCreateTargetList();
                ctForm.ShowDialog();
            }
            //Upon return
            this.Close();
            return;
        }


    }
}
