using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace OrderManager.Classes
{
    public class FileExistenceGridViewHelper
    {
        private readonly GridView _gridView;
        private const string FileStatusColumnName = "FileStatus";

        public FileExistenceGridViewHelper(GridView gridView)
        {
            _gridView = gridView;
            _gridView.CustomColumnDisplayText += GridView_CustomColumnDisplayText;
            _gridView.CustomRowFilter += GridView_CustomRowFilter;
            AddUnboundColumn();
        }

        private void AddUnboundColumn()
        {
            GridColumn unboundColumn = new GridColumn
            {
                Caption = "Label Status",
                FieldName = FileStatusColumnName,
                Visible = true,
                UnboundType = DevExpress.Data.UnboundColumnType.String
            };

            _gridView.Columns.Add(unboundColumn);
        }

        private void GridView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == FileStatusColumnName && e.ListSourceRowIndex != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                string filePath = _gridView.GetListSourceRowCellValue(e.ListSourceRowIndex, "LabelFile").ToString();
                e.DisplayText = CustomTextConverter.Convert(filePath);
            }
        }

        private void GridView_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            string filePath = view.GetListSourceRowCellValue(e.ListSourceRow, "LabelFile").ToString();
            string customText = CustomTextConverter.Convert(filePath);

            if (view.ActiveFilterString == $"[{FileStatusColumnName}] = 'File exists'")
            {
                e.Visible = customText == "File exists";
            }
            else if (view.ActiveFilterString == $"[{FileStatusColumnName}] = 'File missing'")
            {
                e.Visible = customText == "File missing";
            }

            e.Handled = true;
        }
    }


}
