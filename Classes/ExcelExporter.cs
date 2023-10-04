using DevExpress.XtraGrid;
using DevExpress.XtraExport;
using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace OrderManager
{
    public class ExcelExporter

    {

        public GridView GridView { get; set; }

        public ExcelExporter(GridView gridView)
        {
            GridView = gridView;
        }

        public void ExportToXls()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xls",
                Title = "Save as Excel File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;

                // Create an XlsExportOptions instance and configure it.
                XlsExportOptions options = new XlsExportOptions
                {
                    TextExportMode = TextExportMode.Value,
                    ExportMode = XlsExportMode.SingleFile,
                    SheetName = "Sheet1"
                };

                // Export the GridView data to the XLS file.
                GridView.ExportToXls(fileName, options);

                XtraMessageBox.Show("Data exported successfully!", "Export to XLS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}