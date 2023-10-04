using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManager.Classes
{
    public class LateOrderFilter
    {
        public void ApplyFilter(FileExistenceGridView gridView)
        {
            // Get the EntryDateTime and DueDate columns by their field names
            GridColumn entryDateTimeColumn = gridView.Columns["EntryDateTime"];
            GridColumn dueDateColumn = gridView.Columns["DueDate"];

            // Ensure that both columns exist in the grid view
            if (entryDateTimeColumn != null && dueDateColumn != null)
            {
                string filterString = $"[EntryDateTime] < #{DateTime.Now.AddDays(-7)}# AND [DueDate] < #{DateTime.Now}#";

                if (gridView.ActiveFilterString == filterString)
                {
                    gridView.ActiveFilter.Clear();
                }
                else
                {
                    gridView.ActiveFilter.Clear();
                    gridView.ActiveFilterString = filterString;
                }

                gridView.RefreshData();
            }
            else
            {
                // Handle the case where either of the columns doesn't exist
                string missingColumns = string.Empty;

                if (entryDateTimeColumn == null)
                {
                    missingColumns += "EntryDateTime";
                }

                if (dueDateColumn == null)
                {
                    missingColumns += (string.IsNullOrEmpty(missingColumns) ? "" : ", ") + "DueDate";
                }

                XtraMessageBox.Show($"Unable to filter. The following columns do not exist: {missingColumns}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
