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
using System.Windows.Forms;

namespace VariScan
{
    public partial class FormCreateTargetList : Form
    {

        public FormCreateTargetList()
        {
            InitializeComponent();
            //Load Filter assignments
            Configuration cfg = new Configuration();
            //Load color standard defaults
            //Load filter list from TSX and 
            string[] zeroBasedFilters = Filters.FilterNameSet();
            if (zeroBasedFilters != null)
            {
                FilterListBox.Items.AddRange(zeroBasedFilters);
            }
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
        }

        private void DoneButton_Click(object sender, EventArgs e)
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
            this.Close();
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
