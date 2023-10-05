using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;

namespace OrderManagerEF.Classes
{
    public class GridViewSumHelper
    {
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;

        public GridViewSumHelper(DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            this.gridView = gridView;
        }

        public void AddSumToGroupedColumn(string columnName, string groupedColumnName)
        {
            if (gridView.Columns[columnName] == null)
            {
                XtraMessageBox.Show($"The column '{columnName}' does not exist in the GridView.");
                return;
            }

            gridView.OptionsView.ShowFooter = true;
            gridView.Columns[columnName].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            gridView.Columns[columnName].SummaryItem.DisplayFormat = "{0:n2}";
            gridView.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
                new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, columnName, gridView.Columns[columnName], "{0:n2}")
            });

            // Remove existing groupings
            gridView.ClearGrouping();

            // Group by the specified column
            gridView.Columns[groupedColumnName].GroupIndex = 0;

            // Expand all groups
            gridView.ExpandAllGroups();
        }


    }

}
