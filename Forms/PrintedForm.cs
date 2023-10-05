using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;

namespace OrderManagerEF
{
    public partial class PrintedForm : RibbonForm
    {
        private readonly ExcelExporter _excelExporter;
        private bool _dataLoaded;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;


        public PrintedForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            VisibleChanged += Printed_VisibleChanged;
            _excelExporter = new ExcelExporter(gridView1);
            _configuration = configuration;
            _context = context;

        }

        private void LoadData()
        {
            var data = _context.PrintedOrderDatas.ToList();

            // Populate the grid control with the fetched data
            gridView1.GridControl.DataSource = data;

            //FileExistenceGridView gridView = new FileExistenceGridView()
            var newView = new FileExistenceGridView(_configuration)
            {
                FileLocationColumnNames =
                {
                    "LabelFile", "ArchiveFile", "PickSlipFile"
                }, // Add your column names containing the file locations
                FilterFileExists = false
            };

            gridControl1.MainView = newView;

            PopulateZEmployeeGroupLookup(newView);
            //AddPreviewLinkColumn(newView);

            newView.CellValueChanged += GridView_CellValueChanged;
        }



        private void Printed_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Printed_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            {
                var gridView = gridControl1.FocusedView as FileExistenceGridView;

                if (gridView.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Please select one or more rows to redo orders");
                    return;
                }

                if (gridView.SelectedRowsCount > 5)
                {
                    var result = XtraMessageBox.Show(
                        "You have selected more than 5 rows. Are you sure you want to continue?", "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No) return;
                }

                var selectedRowHandles = gridView.GetSelectedRows();
                var salesOrderReferences = new List<string>();

                foreach (var rowHandle in selectedRowHandles)
                {
                    var salesOrderReference = gridView.GetRowCellValue(rowHandle, "AccountingRef").ToString();
                    salesOrderReferences.Add(salesOrderReference);
                }

                UpdateZemployeeGroup(salesOrderReferences);
                // Refresh the GridView
                XtraMessageBox.Show("Orders have now been hidden on the scanners and are available to process again. ");
                var data = _context.PrintedOrderDatas.ToList();

                // Populate the grid control with the fetched data
                gridView.GridControl.DataSource = data;
                gridView.RefreshData();

            }
        }

        private void UpdateZemployeeGroup(List<string> salesOrderReferences)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var salesOrderReference in salesOrderReferences)
                    using (var command = new System.Data.SqlClient.SqlCommand("dbo.[ASP_COMPLETE_SSI_Redo]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Assuming the parameter name is @SalesOrderReference in your stored procedure
                        command.Parameters.AddWithValue("@SalesOrderReference", salesOrderReference);

                        command.ExecuteNonQuery();
                    }
            }
        }

        private void UpdateNoStock(List<string> salesOrderReferences)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var salesOrderReference in salesOrderReferences)
                    using (var command = new System.Data.SqlClient.SqlCommand("dbo.ASP_COMPLETE_NoStock", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Assuming the parameter name is @SalesOrderReference in your stored procedure
                        command.Parameters.AddWithValue("@SalesOrderReference", salesOrderReference);

                        command.ExecuteNonQuery();
                    }
            }
        }

        // Refactored PopulateZEmployeeGroupLookup method
        private void PopulateZEmployeeGroupLookup(FileExistenceGridView gridView)
        {
            var dataFromProcedure = _context.SampleOrderDatas.ToList();
            var zEmployeeGroupValues = dataFromProcedure.Select(d => d.ZEmployeeGroup).Distinct().ToList();

            var lookupTable = new DataTable();
            lookupTable.Columns.Add("ID", typeof(string));
            lookupTable.Columns.Add("Value", typeof(string));

            foreach (var value in zEmployeeGroupValues)
            {
                var newRow = lookupTable.NewRow();
                newRow["ID"] = value;
                newRow["Value"] = value;
                lookupTable.Rows.Add(newRow);
            }

            repositoryItemLookUpEdit1.DataSource = lookupTable;
            repositoryItemLookUpEdit1.ValueMember = "ID";
            repositoryItemLookUpEdit1.DisplayMember = "Value";

            gridView.Columns["ZEmployeeGroup"].ColumnEdit = repositoryItemLookUpEdit1;
        }

        private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var gridView = sender as FileExistenceGridView;

            // Check if the changed cell is in the ZEmployeeGroup column
            if (e.Column.FieldName == "ZEmployeeGroup")
            {
                // Get the new value from the repositoryItemLookUpEdit1
                var newZEmployeeGroupValue = e.Value.ToString();

                // Update the ZEmployeeGroup value in the data source (rubiesDataSet.ASP_COMPLETE_SSI)
                var row = gridView.GetDataRow(e.RowHandle);
                row["ZEmployeeGroup"] = newZEmployeeGroupValue;

                // Optional: If you need to save the changes immediately to the database, call TableAdapter.Update()
                // aSP_COMPLETE_SSITableAdapter.Update(rubiesDataSet.ASP_COMPLETE_SSI);
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            _excelExporter.ExportToXls();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            {
                var gridView = gridControl1.FocusedView as FileExistenceGridView;

                if (gridView.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Please select one or more rows");
                    return;
                }


                var selectedRowHandles = gridView.GetSelectedRows();
                var salesOrderReferences = new List<string>();

                foreach (var rowHandle in selectedRowHandles)
                {
                    var salesOrderReference = gridView.GetRowCellValue(rowHandle, "AccountingRef").ToString();
                    salesOrderReferences.Add(salesOrderReference);
                }

                UpdateNoStock(salesOrderReferences);
                // Refresh the GridView
                XtraMessageBox.Show(
                    "These orders has been marked as No Stock and have been removed from all import functions. ");
                var data = _context.PrintedOrderDatas.ToList();


                // Populate the grid control with the fetched data
                gridView.GridControl.DataSource = data;
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            {
                var gridView = gridControl1.FocusedView as FileExistenceGridView;

                if (gridView.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Please select one or more rows to redo orders");
                    return;
                }


                var selectedRowHandles = gridView.GetSelectedRows();
                var salesOrderReferences = new List<string>();

                foreach (var rowHandle in selectedRowHandles)
                {
                    var salesOrderReference = gridView.GetRowCellValue(rowHandle, "AccountingRef").ToString();
                    salesOrderReferences.Add(salesOrderReference);
                }

                UpdateZemployeeGroup(salesOrderReferences);
                // Refresh the GridView
                XtraMessageBox.Show("Orders have now been hidden on the scanners and are available to process again. ");
                var data = _context.PrintedOrderDatas.ToList();


                // Populate the grid control with the fetched data
                gridView.GridControl.DataSource = data;
            }
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            var gridView = gridControl1.FocusedView as FileExistenceGridView;
            if (gridView != null)
            {
                var filter = new LateOrderFilter();
                filter.ApplyFilter(gridView);
            }
        }
    }
}