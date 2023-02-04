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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace VariScan
{
    public partial class FormCreateTargetList : Form
    {

        public string[] zeroBasedFilters;

        public FormCreateTargetList()
        {
            InitializeComponent();
            //fill Collection list
            CollectionListBox.Items.Clear();
            foreach (var c in CollectionManagement.ListCollections())
                CollectionListBox.Items.Add(c);
            return;
        }

        private void InitializeCollection()
        {
            Configuration cfg = new Configuration();
            this.Text = "Collection: " + cfg.TargetListPath;
            if (File.Exists(cfg.TargetListPath))
            {
                //file exists so populate window accordingly
                TargetXList tList = new TargetXList();
                List<TargetXList.TargetXDescriptor> tXList = tList.GetTargetXList();
                foreach (TargetXList.TargetXDescriptor tX in tXList)
                    TargetListDropDown.Items.Add(tX.Name);
                TargetListDropDown.Text = TargetListDropDown.Items[0].ToString();
                //Load Filter assignments
                ColorIndexing colorIndex = new ColorIndexing();
                zeroBasedFilters = colorIndex.GetSessionFilters().ToArray();
                //Add to list box
                if (zeroBasedFilters != null)
                    FilterListBox.Items.AddRange(zeroBasedFilters);
                for (int i = 0; i < FilterListBox.Items.Count; i++)
                    FilterListBox.SetItemChecked(i, true);
            }
            else                //Load filters from TSX
            {
                zeroBasedFilters = Filters.FilterNameSet();
                //Add to list box
                if (zeroBasedFilters != null)
                    FilterListBox.Items.AddRange(zeroBasedFilters);
            }
            Utility.ButtonRed(CompileButton);
            Utility.ButtonGreen(DoneButton);
        }


        private void CreateAAVSOTargetList_Button(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://filtergraph.com/aavso/default/index");
            CompileButton.Text = "Cancel";
        }

        private void ImportCSVButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(ImportCSVButton);
            DialogResult odr = OpenTargetTextFileDialog.ShowDialog();
            if (odr == DialogResult.OK)
            {
                string textFileName = OpenTargetTextFileDialog.FileName;
                Configuration cfg = new Configuration();
                TargetXList.CreateXList(textFileName, cfg.TargetListPath);
            }
            Utility.ButtonGreen(ImportCSVButton);
            CompileButton.Text = "Compile";
            return;
        }

        private void CompileButton_Click(object sender, EventArgs e)
        {
            //Save configurations to file
            ColorIndexing clist = new ColorIndexing();
            List<Filters.ActiveFilter> afList = new List<Filters.ActiveFilter>();
            for (int i = 0; i < FilterListBox.CheckedItems.Count; i++)
            {
                afList.Add(new Filters.ActiveFilter()
                {
                    FilterName = FilterListBox.CheckedItems[i].ToString(),
                    FilterIndex = (int)Filters.LookUpFilterIndex(FilterListBox.CheckedItems[i].ToString())
                });
            }
            clist.SaveActiveFilters(afList);
        }

        private void AddTargetButton_Click(object sender, EventArgs e)
        {
            //Add contents of target list field to target list
            //Read in from field
            string newTgtName = TargetListDropDown.Text;
            //Look up target from TSX and get Ra and Dec
            double ra; double dec;
            try { (ra, dec) = TSX_Resources.FindTarget(newTgtName); }
            catch
            {
                MessageBox.Show("Look up of target failed");
                return;
            }
            //Add new target
            TargetXList.AddToTargetXList(newTgtName, ra, dec, DateTime.Now);
            //Repopulate
            TargetXList tList = new TargetXList();
            List<TargetXList.TargetXDescriptor> tXList = tList.GetTargetXList();
            TargetListDropDown.Items.Clear();
            foreach (TargetXList.TargetXDescriptor tX in tXList)
                TargetListDropDown.Items.Add(tX.Name);
            TargetListDropDown.Text = TargetListDropDown.Items[0].ToString();
        }

        private void FinishedButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteTargetButton_Click(object sender, EventArgs e)
        {
            //Read in from field
            string newTgtName = TargetListDropDown.Text;
            //remove target
            TargetXList.DeleteFromTargetXList(newTgtName);
            //Repopulate
            TargetXList tList = new TargetXList();
            List<TargetXList.TargetXDescriptor> tXList = tList.GetTargetXList();
            TargetListDropDown.Items.Clear();
            foreach (TargetXList.TargetXDescriptor tX in tXList)
                TargetListDropDown.Items.Add(tX.Name);
            TargetListDropDown.Text = TargetListDropDown.Items[0].ToString();
        }

        private void CollectionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tgtListPath = CollectionManagement.OpenCollection(CollectionListBox.SelectedItem.ToString());
            InitializeCollection();
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
                InitializeCollection();
            }
            return;
        }

        private void AddCollectionTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        //private void PrimaryFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    PrimaryFilterZBBox.Text = Filters.LookUpFilterIndex(PrimaryFilterBox.Text).ToString() ?? "N/A";
        //}

        //private void DifferentialFilterZBBox_TextChanged(object sender, EventArgs e)
        //{
        //    DifferentialFilterZBBox.Text = Filters.LookUpFilterIndex(DifferentialFilterBox.Text).ToString() ?? "N/A";
        //}

        //private void OtherFilterZBBox_TextChanged(object sender, EventArgs e)
        //{
        //    OtherFilterZBBox.Text = Filters.LookUpFilterIndex(OtherFilterBox.Text).ToString() ?? "N/A";
        //}
    }
}
