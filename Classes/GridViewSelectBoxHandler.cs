using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraGrid.Views.Base;

namespace OrderManager.Classes
{
    using System;
    using DevExpress.XtraEditors.Repository;
    using DevExpress.XtraGrid.Views.Grid;

    public class GridViewSelectBoxHandler
    {
        private readonly GridView _gridView;
        private readonly string _columnName;
        private readonly object _dataSource;
        private readonly string _displayMember;
        private readonly string _valueMember;
        private readonly Action<int, object> _updateDatabaseAction;

        public GridViewSelectBoxHandler(GridView gridView, string columnName, object dataSource, string displayMember, string valueMember,
            Action<int, object> updateDatabaseAction)
        {
            _gridView = gridView;
            _columnName = columnName;
            _dataSource = dataSource;
            _displayMember = displayMember;
            _valueMember = valueMember;
            _updateDatabaseAction = updateDatabaseAction;

            InitializeSelectBox();
        }

        private void InitializeSelectBox()
        {
            RepositoryItemLookUpEdit repLookup = new RepositoryItemLookUpEdit();
            repLookup.DataSource = _dataSource;
            repLookup.DisplayMember = _displayMember;
            repLookup.ValueMember = _valueMember;

            _gridView.Columns[_columnName].ColumnEdit = repLookup;
            _gridView.CellValueChanged += GridView_CellValueChanged;
        }

        private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == _columnName)
            {
                // Get the new value
                object newValue = e.Value;

                // Get the primary key value for the affected row
                int id = (int)_gridView.GetRowCellValue(e.RowHandle, "Id");

                // Call the provided update method
                _updateDatabaseAction(id, newValue);

                // Refresh the data source if needed
                _gridView.RefreshData();
            }
        }
    }



}
