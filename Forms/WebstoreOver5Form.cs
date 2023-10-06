using DevExpress.Data.Filtering;
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.Forms;
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
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json.Linq;
using OrderManagerEF.DTOs;

namespace OrderManagerEF
{
    public partial class WebstoreOver5Form : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private ExcelExporter _excelExporter;
        private readonly BulkReportGenerator _reportGenerator;
        private FileExistenceGridViewHelper _fileExistenceGridViewHelper;
        private bool _dataLoaded;
        private HttpClient client;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly string _location = "RUB"; // Define your location
        private readonly IConfiguration _configuration;
        private readonly ReportManager _reportManager;
        private readonly PickSlipGenerator _pickSlipGenerator;
        private readonly OMDbContext _context;
        private readonly StoredProcedureService _storedProcedureService;

        public WebstoreOver5Form(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;


            VisibleChanged += WebstoreOver5_VisibleChanged;

            _excelExporter = new ExcelExporter(gridView1);

            _reportGenerator = new BulkReportGenerator(configuration);

            // Enable multi-row selection in the GridView
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            var connectionString =
                _configuration.GetConnectionString("RubiesConnectionString");

            _apiKeyManager = new ApiKeyManager(connectionString);

            SetUpHttpClient(_location);

            _reportManager = new ReportManager(configuration);
            _pickSlipGenerator = new PickSlipGenerator(configuration, context);
             BarButtonClicks();
        }


        private void BarButtonClicks()
        {   //Export to Excel
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
            //Sync IDs
            barButtonItem2.ItemClick += barButtonItem2_ItemClick;
            //Show IDS
            barButtonItem3.ItemClick += barButtonItem3_ItemClick;
            //Create Batch
            barButtonItem4.ItemClick += barButtonItem4_ItemClick;
            //Show Batch
            barButtonItem5.ItemClick += barButtonItem5_ItemClick;
            //Process Batch
            barButtonItem6.ItemClick += barButtonItem6_ItemClick;
            //Sort By BinNumber
            barButtonItem7.ItemClick += barButtonItem7_ItemClick;
            //Hold Order
            barButtonItem8.ItemClick += barButtonItem8_ItemClick;
            //Show Ready Orders
            barButtonItem9.ItemClick += barButtonItem9_ItemClick;
            //Show Duplicates
            barButtonItem10.ItemClick += barButtonItem10_ItemClick;
            //Select and Process
            barButtonItem11.ItemClick += barButtonItem11_ItemClick;
        }



        private List<ASP_SSI_Result> LoadDataFromStoredProcedure()
        {
            return _storedProcedureService.ExecuteStoredProcedure("ASP_RUB_SSI_OVER5");
        }

        private void UpdateFileStatusForData(List<RubiesOver5OrderData> data)
        {
            foreach (var item in data)
            {
                item.FileStatus = CustomTextConverter.Convert(item.LabelFile);
            }
        }


        private void LoadData()
        {
            // Show the default splash screen
            SplashScreenManager.ShowDefaultWaitForm("Please wait", "Loading data...");

            try
            {
                LoadPickSlipData();
                var data = _context.RubiesOver5OrderDatas.ToList();

                // Update the FileStatus property for each item in the data list.
                UpdateFileStatusForData(data);

                // Populate the grid control with the fetched data
                gridView1.GridControl.DataSource = data;
                gridView1.RefreshData();

                var newView = new FileExistenceGridView(_configuration)
                {
                    FileLocationColumnNames =
                        { "LabelFile", "PickSlipFile" }, // Add your column names containing the file locations
                    FilterFileExists = false
                };

                gridControl1.MainView = newView;
                AddPreviewLinkColumn(newView);
                gridControl1.DataSource = data; // Here, we set the data directly instead of updatedDataTable
                HighlightDuplicateRows(newView);

                _fileExistenceGridViewHelper = InitializeFileExistenceHelper(newView);
                gridView1.KeyDown += gridView1_KeyDown;
            }
            finally
            {
                // Close the splash screen once data is loaded
                SplashScreenManager.CloseForm(false);
            }
        }

        private FileExistenceGridViewHelper InitializeFileExistenceHelper(FileExistenceGridView gridView)
        {
            var fileExistenceGridViewHelper = new FileExistenceGridViewHelper(gridView);
            return fileExistenceGridViewHelper;
        }


        private void LoadPickSlipData()
        {
            // Define the customer groups dictionary that you want to merge
            Dictionary<string, string> customerGroups = new Dictionary<string, string>
            {
                {"MOVIEWO", "MOVIEWO"},
                {"COSTUME BOX", "COSTUME BOX"},
                {"CASEYS", "CASEYS"},
                {"COSTCO", "COSTCO"},
                {"EXPORT", "EXPORT"},
                {"INDEPENDENTS", "INDEPENDENTS"},
                {"HIGHEST HEEL", "HIGHEST HEEL"},
                {"NX", "NX"},
                {"MRTOYS", "MRTOYS"},
                {"ONLINE", "ONLINE"},
                {"KIDSTUFF", "KIDSTUFF"},
                {"ARL", "ARL"},
                {"LICENSOR", "LICENSOR"},
                {"AMAZON", "AMAZON"},
                {"CATCH", "CATCH"},
                {"DISC10", "DISC10"},
                {"TOYMATE", "TOYMATE"},
                {"DISC15", "DISC15"},
                {"MEXPORT", "MEXPORT"},
                {"COSTCODTC", "COSTCODTC"},
                {"DISC20", "DISC20"},
                {"STAFF", "STAFF"},
                {"COLES", "COLES"}
              
                // Add other customer groups as needed
            };

            _pickSlipGenerator.MergeTable(customerGroups); // Call the merge method

        }

        private void WebstoreOver5_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }

        private void FilterDuplicateRows(FileExistenceGridView gridView)
        {
            var highlighter = new DuplicateRowHighlighter();
            highlighter.HighlightDuplicates(gridView);

            highlighter.FilterDuplicates(gridView);
        }


        private void HighlightDuplicateRows(FileExistenceGridView gridView)
        {
            var highlighter = new DuplicateRowHighlighter();
            highlighter.HighlightDuplicates(gridView);
        }



        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            _excelExporter.ExportToXls();
        }



        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                gridView1.SelectAll();
                e.Handled = true; // optional: prevents other handlers from receiving this event
            }
        }

        private void UpdateBinSortRUB()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("RubiesConnectionString")))

            {
                using (var command = new SqlCommand("ASP_PickSortListRUB", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Show the SplashScreen
            SplashScreenManager.ShowDefaultWaitForm();

            try
            {
                // Call the stored procedure
                UpdateBinSortRUB();

                // Refresh the GridView
                var gridView = gridControl1.FocusedView as GridView;

                // Fetch the updated data from the database
                var data = _context.CscOrderDatas.ToList();

                // Set the fetched data as the grid's data source
                gridView.GridControl.DataSource = data;

                // Refresh the grid view to reflect the changes
                gridView.RefreshData();
            }
            finally
            {
                // If SplashScreen was shown, close it
                if (SplashScreenManager.Default != null) SplashScreenManager.CloseForm(false);
            }

            // Show a message box indicating the sorting by BinNumber has been completed
            XtraMessageBox.Show("Operation was successful. Sorting by BinNumber has been completed.");
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tableName = "LabelstoPrintRUB";
            var manager = new LabelQueueManager(tableName, _configuration);

            if (manager.ConfirmTruncate())
            {
                manager.TruncateTable();

                var gridView = gridControl1.FocusedView as FileExistenceGridView;

                var columnMappings = new Dictionary<string, string>
                {
                    { "AccountingRef", "SalesOrder" },
                    { "TradingRef", "OrderNumber" },
                    { "CustomerCode", "CustomerCode" },
                    { "EntryDateTime", "Date" }
                };

                string[] parameterNames = { "@column1", "@column2", "@column3", "@column4" };

                if (!CheckZShipmentID(gridView))
                {
                    if (XtraMessageBox.Show("This record does not have a ShipmentID and will not generate a label. Are you sure you wish to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                manager.InsertData(gridView, columnMappings, parameterNames);

                int rowCount = gridView.GetSelectedRows().Length;
                manager.ShowRowCountMessage(rowCount);
            }

            manager.CloseConnection();
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            var newForm = new BatchForm(_configuration, _context);
            newForm.Show();
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // create an SQL connection
                var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
                // Assuming you have a connection string called "connectionString" and a table called "myTable"
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var sql = "SELECT COUNT(*) FROM LabelstoPrintRUB";
                    var cmd = new SqlCommand(sql, conn);
                    var rowCount = (int)cmd.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        // Show a message box asking the user if they want to continue
                        DialogResult result = XtraMessageBox.Show("Are you sure you want to run the job and download " + rowCount + " labels ?", "Confirm Job Run", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        // If the user clicks Yes, continue with the operation
                        if (result == DialogResult.Yes)
                        {
                            var jobRunner = new SqlAgentJobRunner("HVSERVER02\\ABM", "msdb", "LabelPrintRUB");
                            jobRunner.RunJob();
                            // Show the row count in a message box
                            XtraMessageBox.Show("Job started successfully! Number of labels queued " + rowCount);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Warning: The DS Queue does not contain any rows!");
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error starting job: {ex.Message}");
            }
        }

        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            FilterDuplicateRows((FileExistenceGridView)gridControl1.MainView);
        }

        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            var gridView = gridControl1.FocusedView as FileExistenceGridView;


            if (gridView.SelectedRowsCount == 0)
            {
                XtraMessageBox.Show("Please select one or more rows to generate reports.");
                return;
            }

            // Call the method from ReportManager to show a warning if the path is not empty
            _reportManager.ShowWarningIfPathNotEmpty();

            if (gridView.SelectedRowsCount > 100)
            {
                var result =
                    XtraMessageBox.Show("You have selected more than 100 rows. Are you sure you want to continue?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
            }

            //Check Label File Exists
            foreach (var rowHandle in gridView.GetSelectedRows())
            {
                var filePath = gridView.GetRowCellValue(rowHandle, "LabelFile").ToString();
                if (!File.Exists(filePath))
                {
                    var result =
                        XtraMessageBox.Show(
                            "The label file does not exist for one or more selected rows. Do you want to continue processing?",
                            "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No) return; // Exit the method to prevent processing

                    {
                        break; // Break out of the loop and continue processing
                    }
                }
            }


            var selectedRowHandles = gridView.GetSelectedRows();
            var salesOrderReferences = new List<string>();

            foreach (var rowHandle in selectedRowHandles)
            {
                var salesOrderReference = gridView.GetRowCellValue(rowHandle, "AccountingRef").ToString();
                salesOrderReferences.Add(salesOrderReference);
            }
            // Ensure the splash screen is closed
            SplashScreenUtility.CloseSplashScreenIfNeeded();

            // Show the custom splash screen
            SplashScreenManager.ShowForm(typeof(ProgressForm));



            _reportGenerator.GenerateAndSaveReportsProgressPath(salesOrderReferences,
                progress => SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetProgress, progress),
                errorMessage => SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetMessage, errorMessage)
            );

            // Ensure the splash screen is closed
            SplashScreenUtility.CloseSplashScreenIfNeeded();

            //// Close the custom splash screen
            //SplashScreenManager.CloseForm();


            var defaultPrinterName = PrinterHelper.GetDefaultPrinter(_configuration);

            // Call the ExecuteDefaultPrinter method and pass in the default printer name
            var programPath = "C:\\Program Files (x86)\\2Printer\\2Printer.exe";
            var printerProgram = new PrinterProgram(programPath, _configuration);
            printerProgram.ExecuteDefaultPrinter(defaultPrinterName);


            // Refresh the GridView
            var data = _context.RubiesOver5OrderDatas.ToList();

            // Populate the grid control with the fetched data
            gridView.GridControl.DataSource = data;
            gridView.RefreshData();

            // Show a message box indicating all reports were saved
            XtraMessageBox.Show($"{salesOrderReferences.Count} pickslip reports were saved successfully.");
        }

        private bool CheckZShipmentID(FileExistenceGridView gridView)
        {
            // Assuming ZshipmentID is the column name
            string zShipmentIDColumnName = "ZShipmentID";

            bool allRowsHaveShipmentID = true;

            foreach (var rowHandle in gridView.GetSelectedRows())
            {
                string zShipmentIDValue = gridView.GetRowCellValue(rowHandle, zShipmentIDColumnName)?.ToString();

                if (string.IsNullOrEmpty(zShipmentIDValue) || zShipmentIDValue.Trim().Length == 0)
                {
                    allRowsHaveShipmentID = false;
                    break;
                }
            }

            return allRowsHaveShipmentID;
        }

        private void FilterZShipmentID(FileExistenceGridView gridView)
        {
            // Assuming ZShipmentID is in the first cell, change 0 to the correct cell index if needed
            int zShipmentIDColumnIndex = 0;
            GridColumn zShipmentIDColumn = gridView.Columns[zShipmentIDColumnIndex];

            if (gridView.ActiveFilterString.Contains("Not(IsNullOrEmpty([ZShipmentID]))"))
            {
                gridView.ActiveFilterCriteria = null;
                gridView.ActiveFilterString = string.Empty;
            }
            else
            {
                gridView.ActiveFilterCriteria = new NotOperator(
                    new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(zShipmentIDColumn.FieldName)));
                gridView.ActiveFilterString = "Not(IsNullOrEmpty([ZShipmentID]))";
            }

            gridView.RefreshData();
        }



        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            var gridView = gridControl1.FocusedView as FileExistenceGridView;

            if (gridView != null)
            {
                FilterZShipmentID(gridView);
            }
        }


        private async void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // Sync and update orders
                await SyncAndUpdateOrders();

                // Refresh the GridView
                var gridView = gridControl1.FocusedView as GridView;

                var data = _context.RubiesOver5OrderDatas.ToList();

                // Populate the grid control with the fetched data
                gridView.GridControl.DataSource = data;

                gridView.RefreshData();

                XtraMessageBox.Show("Sync and update operation was a success!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Log error
                XtraMessageBox.Show($"An error occurred during the sync and update operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void AddPreviewLinkColumn(GridView gridView)
        {
            // Part 1: Add a new column and specify the column editor
            var column = gridView.Columns.AddField("PreviewLink");
            column.VisibleIndex = gridView.Columns.Count;
            column.UnboundType = UnboundColumnType.String;

            var hyperlink = new RepositoryItemHyperLinkEdit();
            hyperlink.OpenLink += Hyperlink_OpenLink;
            column.ColumnEdit = hyperlink;
            column.Caption = "PickSlip Preview";

            // Populate column with some data
            column.UnboundExpression = "'Preview'";
        }


        private void Hyperlink_OpenLink(object sender, OpenLinkEventArgs e)
        {
            Action<string> errorCallback = (errorMessage) =>
            {
                // Show the XtraMessageBox if an error occurs
                XtraMessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            var gridView = gridControl1.MainView as GridView;
            if (gridView != null)
            {
                var salesOrderReference = gridView.GetFocusedRowCellValue("AccountingRef").ToString();

                // Create an instance of BulkReportGenerator
                var reportGenerator = new BulkReportGenerator(_configuration);

                // Call the GenerateReportPortrait method
                var report = reportGenerator.GenerateReportPortrait(salesOrderReference, errorCallback);

                // Check if the report is not null (i.e., data was found)
                if (report != null)
                {
                    var printTool = new ReportPrintTool(report);
                    printTool.ShowPreviewDialog();
                }
            }

            // Prevent the link from being opened in a browser or another default action
            e.Handled = true;
        }


        private void SetUpHttpClient(string location)
        {
            var (starshipItApiKey, ocpApimSubscriptionKey) = _apiKeyManager.GetApiKeysByLocation(location);


            client = new HttpClient();
            client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", starshipItApiKey);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);
        }

        public async Task SyncAndUpdateOrders()
        {
            int page = 1;
            int limit = 50;
            string sinceOrderDate = Uri.EscapeDataString(DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'"));

            while (true)
            {
                try
                {
                    var responseString = await client.GetStringAsync($"https://api.starshipit.com/api/orders/unshipped?limit={limit}&page={page}&since_order_date={sinceOrderDate}");
                    var orders = JObject.Parse(responseString)["orders"];
                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("RubiesConnectionString")))
                    {
                        connection.Open();
                        foreach (var order in orders)
                        {
                            if (order["order_id"] != null && order["order_number"] != null)
                            {
                                string orderId = (string)order["order_id"];
                                string orderNumber = (string)order["order_number"];
                                using (SqlCommand cmdTransHeader = new SqlCommand("ASP_ShipmentIDSync", connection))
                                {
                                    cmdTransHeader.CommandType = CommandType.StoredProcedure;
                                    cmdTransHeader.Parameters.AddWithValue("@OrderID", orderId);
                                    cmdTransHeader.Parameters.AddWithValue("@OrderNumber", orderNumber);
                                    cmdTransHeader.ExecuteNonQuery();
                                }
                            }
                        }
                        // Check if orders are still available for the next page
                        if (orders.Count() < limit)
                            break;

                        page++;  // Move to next page
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Error: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        XtraMessageBox.Show("Inner Exception: " + ex.InnerException.Message);
                    }
                    break;
                }
            }
        }


        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
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

            CancelOrder(salesOrderReferences);
            // Refresh the GridView
            XtraMessageBox.Show(
                "These orders has been moved to the Hold Tab.");

            // Refresh the GridView
            // Fetch the updated data from the database
            var data = _context.RubiesOver5OrderDatas.ToList();

            // Set the fetched data as the grid's data source
            gridView.GridControl.DataSource = data;
            gridView.RefreshData();
        }



        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            var gridView = gridControl1.FocusedView as FileExistenceGridView;

            if (gridView != null) gridView.ToggleFileExistenceFilter();
        }
        private void CancelOrder(List<string> salesOrderReferences)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var salesOrderReference in salesOrderReferences)
                    using (var command = new SqlCommand("dbo.ASP_CANCEL", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Assuming the parameter name is @SalesOrderReference in your stored procedure
                        command.Parameters.AddWithValue("@SalesOrderReference", salesOrderReference);

                        command.ExecuteNonQuery();
                    }
            }
        }
    }
}