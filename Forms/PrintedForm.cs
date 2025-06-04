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
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using OrderManagerEF.Forms;

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
            BarButtonClicks();
            InitSoHyperLink();
        }

        private void BarButtonClicks()
        {   //Export to Excel
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
            //No Stock Order
            barButtonItem2.ItemClick += barButtonItem2_ItemClick;
            //Redo Order
            barButtonItem3.ItemClick += barButtonItem3_ItemClick;
            //Late Order
            barButtonItem4.ItemClick += barButtonItem4_ItemClick;


        }

        private void Printed_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
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

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            _excelExporter.ExportToXls();
        }

        private void InitSoHyperLink()
        {
            var repositoryItemHyperLinkEdit1 = new RepositoryItemHyperLinkEdit();

            repositoryItemHyperLinkEdit1.OpenLink += (sender, e) =>
            {
                var hyperlink = sender as HyperLinkEdit;
                if (hyperlink != null && !string.IsNullOrEmpty(hyperlink.EditValue?.ToString()))
                {
                    var OrderRef = hyperlink.EditValue.ToString();

                    // Run your operation
                    var detailForm = new OrderLookupForm(_configuration, _context, OrderRef);
                    detailForm.Show();
                    e.Handled = true; // Mark event as handled
                }
            };

            var gridView = gridControl1.MainView as FileExistenceGridView;
            if (gridView != null)
            {
                gridView.Columns["AccountingRef"].ColumnEdit = repositoryItemHyperLinkEdit1;
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
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
            var gridView = gridControl1.FocusedView as FileExistenceGridView;
            if (gridView != null)
            {
                var filter = new LateOrderFilter();
                filter.ApplyFilter(gridView);
            }
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
            
                // Get the focused view as FileExistenceGridView
                var gridView = gridControl1.FocusedView as FileExistenceGridView;
                if (gridView != null)
                {
                    // Create an SQL connection
                    var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Get selected rows from the FileExistenceGridView
                        var selectedRows = gridView.GetSelectedRows();
                        XtraMessageBox.Show($"Selected rows: {selectedRows.Length}");

                        // Dictionary to group records by customer group
                        var groupedRecords = new Dictionary<string, List<string>>();

                        if (selectedRows.Length > 0)
                        {
                            foreach (var rowHandle in selectedRows)
                            {
                                // Get values from the selected row
                                var customerGroup = gridView.GetRowCellValue(rowHandle, "ZEmployeeGroup")?.ToString();
                                var customerCode = gridView.GetRowCellValue(rowHandle, "CustomerCode")?.ToString();
                                var dueDate = gridView.GetRowCellValue(rowHandle, "DueDate") as DateTime?;
                                var accountingRef = gridView.GetRowCellValue(rowHandle, "AccountingRef")?.ToString();
                                var tradingRef = gridView.GetRowCellValue(rowHandle, "TradingRef")?.ToString();


                                // Ensure the required data exists before inserting
                                if (!string.IsNullOrEmpty(customerGroup) && !string.IsNullOrEmpty(customerCode)
                                    && dueDate.HasValue && !string.IsNullOrEmpty(accountingRef)
                                    && !string.IsNullOrEmpty(tradingRef))
                                {
                                    var salesOrder = accountingRef;
                                    var orderNumber = tradingRef;

                                    // Build the record string for bulk insert (adjust based on your database)
                                    var record = $"('{salesOrder}', '{orderNumber}', '{customerCode}', '{dueDate.Value:yyyy-MM-dd HH:mm:ss}')";

                                    if (!groupedRecords.ContainsKey(customerGroup))
                                    {
                                        groupedRecords[customerGroup] = new List<string>();
                                    }
                                    groupedRecords[customerGroup].Add(record);
                                }
                            }

                            // Now, insert records for each customer group in bulk
                            foreach (var group in groupedRecords)
                            {
                                var customerGroup = group.Key;
                                var records = group.Value;

                                var tableName = GetTableNameByCustomerGroup(customerGroup);
                                if (!string.IsNullOrEmpty(tableName))
                                {
                                    var insertSql = $"INSERT INTO {tableName} (SalesOrder, OrderNumber, CustomerCode, Date) VALUES " + string.Join(",", records);

                                    using (var insertCmd = new SqlCommand(insertSql, conn))
                                    {
                                        insertCmd.ExecuteNonQuery();
                                    }

                                    // After insertion, check the row count
                                    var countSql = $@"SELECT COUNT(*) FROM {tableName}";
                                    using (var countCmd = new SqlCommand(countSql, conn))
                                    {
                                        var rowCount = (int)countCmd.ExecuteScalar();

                                        if (rowCount > 0)
                                        {
                                            var result = XtraMessageBox.Show(
                                                $"Are you sure you want to run the job and download {rowCount} labels from {tableName}?",
                                                "Confirm Job Run", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                            if (result == DialogResult.Yes)
                                            {
                                                // Dynamically select the SQL Agent job based on customer group
                                                var jobName = GetJobNameByCustomerGroup(customerGroup);
                                                if (!string.IsNullOrEmpty(jobName))
                                                {
                                                    var jobRunner = new SqlAgentJobRunner("hvserver02\\ABM", "msdb", jobName);
                                                    jobRunner.RunJob();

                                                    // Show the row count in a message box
                                                    XtraMessageBox.Show($"Job started successfully! Number of labels queued from {tableName}: {rowCount}");
                                                }
                                                else
                                                {
                                                    XtraMessageBox.Show($"No job found for customer group {customerGroup}.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            XtraMessageBox.Show($"Warning: The {tableName} queue does not contain any rows!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }


        // Helper method to determine the table name based on customer group
        private string GetTableNameByCustomerGroup(string customerGroup)
        {
            switch (customerGroup)
            {
                case "DS":
                    return "LabelstoPrintDS";
                case "CSC":
                    return "LabelstoPrintCSC";
                // Add more cases as needed
                default:
                    return null;  // Return null if no table is matched
            }
        }

        // Helper method to determine the SQL Agent job name based on customer group
        private string GetJobNameByCustomerGroup(string customerGroup)
        {
            switch (customerGroup)
            {
                case "DS":
                    return "LabelPrintDS";  // Job name for DS group
                case "CSC":
                    return "LabelPrintCSC";  // Job name for CSC group
                // Add more cases as needed
                default:
                    return null;  // Return null if no job is matched
            }
        }


    }
}
