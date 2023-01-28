using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace VariScan
{
    public partial class FormSampleCatalog : Form
    {
        public struct TargetShoot
        {
            public string Target;
            public string Date;
            public string Set;
        }

        public static List<TargetShoot> SessionList { get; set; }

        //public static List<string> SessionSets { get; set; }

        public FormSampleCatalog()
        {
            InitializeComponent();
            //make this form topmost, if selected in calling form
            Configuration cfg = new Configuration();
            if (Convert.ToBoolean(cfg.AnalysisFormOnTop.ToString()))
                this.TopMost = true;

            //Dimension the target/date array
            List<string> targetList = VariScanFileManager.GetTargetNameList();
            List<string> dateList = VariScanFileManager.GetAllImageDates();
            //If no targets, return
            if (targetList.Count == 0)
                return;
            //Create the row and column headers by target and date
            imageBankGridView.RowCount = targetList.Count;
            for (int i = 0; i < targetList.Count; i++)
                imageBankGridView.Rows[i].HeaderCell.Value = targetList[i];
            imageBankGridView.ColumnCount = dateList.Count;
            for (int i = 0; i < dateList.Count; i++)
                imageBankGridView.Columns[i].HeaderCell.Value = dateList[i];
            //Fill in Filters and image count for each cell, if any
            for (int iRow = 0; iRow < imageBankGridView.RowCount; iRow++)
                for (int iCol = 0; iCol < imageBankGridView.ColumnCount; iCol++)
                {
                    //Get the list of image paths for this cell
                    string cellTgt = imageBankGridView.Rows[iRow].HeaderCell.Value.ToString();
                    DateTime cellDate = Convert.ToDateTime(imageBankGridView.Columns[iCol].HeaderCell.Value.ToString());
                    List<string> imagePaths = VariScanFileManager.GetTargetSessionPaths(cellTgt, cellDate);
                    //Check to see if there are some images, if so get a list of filters, then a count of images per filter
                    if (imagePaths.Count > 0)
                    {
                        List<string> filterList = new List<string>();
                        foreach (string path in imagePaths)
                        {
                            (string tName, string iDate, string iFilter, string iSeq, string iSet) = VariScanFileManager.ParseImageFileName(Path.GetFileNameWithoutExtension(path));
                            //SessionSets.Add(iSet);
                            filterList.Add(iFilter);
                        }
                        filterList = filterList.Distinct().ToList();
                        //SessionSets = SessionSets.Distinct().ToList();
                        foreach (string filter in filterList)
                            imageBankGridView.Rows[iRow].Cells[iCol].Value += "F" + filter +
                            "(" +
                            VariScanFileManager.GetTargetSessionPaths(cellTgt, cellDate, filter).Count().ToString() +
                            ") ";
                    }
                }
            SessionList = new List<TargetShoot>();
            return;
        }

        private void Select_Click(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            //Assemble selection of cells
            DataGridViewSelectedCellCollection selectedCells = imageBankGridView.SelectedCells;
            //Clear session list
            SessionList.Clear();
            //Add selected list of target dates to session list
            foreach (DataGridViewCell s in selectedCells)
            {
                if (s.Value != null)
                {
                    string t = s.OwningRow.HeaderCell.Value.ToString();
                    string d = s.OwningColumn.HeaderCell.Value.ToString();
                    //Add session sets
                    List<string> imagePaths = VariScanFileManager.GetTargetSessionPaths(t, Convert.ToDateTime(d));
                    foreach (string f in imagePaths)
                    {
                        //get session set and add to session list
                        (string tName, string iDate, string iFilter, string iSeq, string iSet) = VariScanFileManager.ParseImageFileName(Path.GetFileNameWithoutExtension(f));
                        SessionList.Add(new TargetShoot { Target = t, Date = d, Set = iSet });
                    }
                }
                SessionList = SessionList.Distinct().ToList();
            }
            this.Close();
            return;
        }


    }
}
