using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.DTOs;
using OrderManagerEF.Entities;
using OrderManagerEF.Forms;

namespace OrderManagerEF;

public partial class NecaForm : XtraForm
{
    private readonly ExcelExporter _excelExporter;
    private readonly BulkReportGenerator _reportGenerator;
    private FileExistenceGridViewHelper _fileExistenceGridViewHelper;
    private bool _dataLoaded;
    private HttpClient client;
    private readonly ApiKeyManager _apiKeyManager;
    private readonly string _location = "NECA"; // Define your location
    private readonly IConfiguration _configuration;
    private readonly ReportManager _reportManager;
    private readonly PickSlipGenerator _pickSlipGenerator;
    private readonly OMDbContext _context;
    private readonly StoredProcedureService _storedProcedureService;
    private readonly UserSession _userSession;

    public NecaForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
    {
        InitializeComponent();
        _configuration = configuration;
        _context = context;
        _userSession = userSession;


        VisibleChanged += Neca_VisibleChanged;

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
    }

    private void LoadData()
    {
        // Show the default splash screen
        SplashScreenManager.ShowDefaultWaitForm("Please wait", "Loading data...");

        try
        {

            var data = _context.NecaOrderDatas.ToList();

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
            InitSoHyperLink();
        }
        finally
        {
            // Close the splash screen once data is loaded
            SplashScreenManager.CloseForm(false);
        }
    }



    private void SetUpHttpClient(string location)
    {
        var (starshipItApiKey, ocpApimSubscriptionKey) = _apiKeyManager.GetApiKeysByLocation(location);


        client = new HttpClient();
        client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", starshipItApiKey);
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);
    }

    private void gridView1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            gridView1.SelectAll();
            e.Handled = true; // optional: prevents other handlers from receiving this event
        }
    }


    private void UpdateFileStatusForData(List<NecaOrderData> data)
    {
        foreach (var item in data) item.FileStatus = CustomTextConverter.Convert(item.LabelFile);
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
        if (gridView != null) gridView.Columns["AccountingRef"].ColumnEdit = repositoryItemHyperLinkEdit1;
    }

    private FileExistenceGridViewHelper InitializeFileExistenceHelper(FileExistenceGridView gridView)
    {
        var fileExistenceGridViewHelper = new FileExistenceGridViewHelper(gridView);
        return fileExistenceGridViewHelper;
    }

    private void Neca_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible && !_dataLoaded)
        {
            LoadData();
            InitializeHyperLink();
            _dataLoaded = true;
        }
    }

    private void InitializeHyperLink()
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
        if (gridView != null) gridView.Columns["AccountingRef"].ColumnEdit = repositoryItemHyperLinkEdit1;
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

    private void UpdateBinSortNECA()
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

    private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            // create an SQL connection
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            // Assuming you have a connection string called "connectionString" and a table called "myTable"
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                conn.Open();

                var sql = "SELECT COUNT(*) FROM LabelstoPrintNECA";
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                var rowCount = (int)cmd.ExecuteScalar();

                if (rowCount > 0)
                {
                    // Show a message box asking the user if they want to continue
                    var result = XtraMessageBox.Show(
                        "Are you sure you want to run the job and download " + rowCount + " labels ?",
                        "Confirm Job Run", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // If the user clicks Yes, continue with the operation
                    if (result == DialogResult.Yes)
                    {
                        var jobRunner = new SqlAgentJobRunner("HVSERVER02\\ABM", "msdb", "LabelPrintNECA");
                        jobRunner.RunJob();
                        // Show the row count in a message box
                        XtraMessageBox.Show("Job started successfully! Number of labels queued " + rowCount);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Warning: The NECA Queue does not contain any rows!");
                }
            }
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Error starting job: {ex.Message}");
        }
    }


    private bool CheckZShipmentID(FileExistenceGridView gridView)
    {
        // Assuming ZshipmentID is the column name
        var zShipmentIDColumnName = "ZShipmentID";

        var allRowsHaveShipmentID = true;

        foreach (var rowHandle in gridView.GetSelectedRows())
        {
            var zShipmentIDValue = gridView.GetRowCellValue(rowHandle, zShipmentIDColumnName)?.ToString();

            if (string.IsNullOrEmpty(zShipmentIDValue) || zShipmentIDValue.Trim().Length == 0)
            {
                allRowsHaveShipmentID = false;
                break;
            }
        }

        return allRowsHaveShipmentID;
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
        Action<string> errorCallback = errorMessage =>
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
            var report = reportGenerator.GenerateReportPreorderPortrait(salesOrderReference, errorCallback);

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

    private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
    {
        _excelExporter.ExportToXls();
    }

    private async void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            // Sync and update orders
            await SyncAndUpdateOrders();

            // Refresh the GridView
            var gridView = gridControl1.FocusedView as GridView;

            var data = _context.NecaOrderDatas.ToList();

            // Populate the grid control with the fetched data
            gridView.GridControl.DataSource = data;

            gridView.RefreshData();

            XtraMessageBox.Show("Sync and update operation was a success!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            // Log error
            XtraMessageBox.Show($"An error occurred during the sync and update operation: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public async Task SyncAndUpdateOrders()
    {
        var page = 1;
        var limit = 50;
        var sinceOrderDate = Uri.EscapeDataString(DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'"));

        while (true)
            try
            {
                var responseString = await client.GetStringAsync(
                    $"https://api.starshipit.com/api/orders/unshipped?limit={limit}&page={page}&since_order_date={sinceOrderDate}");
                var orders = JObject.Parse(responseString)["orders"];
                using (var connection = new SqlConnection(_configuration.GetConnectionString("RubiesConnectionString")))
                {
                    connection.Open();
                    foreach (var order in orders)
                        if (order["order_id"] != null && order["order_number"] != null)
                        {
                            var orderId = (string)order["order_id"];
                            var orderNumber = (string)order["order_number"];
                            using (var cmdTransHeader = new SqlCommand("ASP_ShipmentIDSync", connection))
                            {
                                cmdTransHeader.CommandType = CommandType.StoredProcedure;
                                cmdTransHeader.Parameters.AddWithValue("@OrderID", orderId);
                                cmdTransHeader.Parameters.AddWithValue("@OrderNumber", orderNumber);
                                cmdTransHeader.ExecuteNonQuery();
                            }
                        }

                    // Check if orders are still available for the next page
                    if (orders.Count() < limit)
                        break;

                    page++; // Move to next page
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error: " + ex.Message);
                if (ex.InnerException != null) XtraMessageBox.Show("Inner Exception: " + ex.InnerException.Message);
                break;
            }
    }

    private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
    {
        StartImportTask();
    }

    private void StartImportTask()
    {
        try
        {
            // Fetch server and job name from configuration
            var serverName = _configuration["ServerName1"]; // 
            var jobName = _configuration["JobName1"]; // 

            // Show a message box asking the user if they want to continue
            var result = XtraMessageBox.Show(
                "Are you sure you want to run the job?",
                "Confirm Job Run", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicks Yes, continue with the operation
            if (result == DialogResult.Yes)
            {
                // Initialize SqlAgentJobRunner using configuration data
                var jobRunner = new SqlAgentJobRunner(serverName, "msdb", jobName);
                jobRunner.RunJob();

                // Show success message
                XtraMessageBox.Show("Job started successfully!");
            }
        }
        catch (Exception ex)
        {
            // Show error message if there is an exception
            XtraMessageBox.Show($"Error starting job: {ex.Message}");
        }
    }

    //Show iDs
    private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
    {
        var gridView = gridControl1.FocusedView as FileExistenceGridView;

        if (gridView != null) FilterZShipmentID(gridView);
    }

    private void FilterZShipmentID(FileExistenceGridView gridView)
    {
        // Assuming ZShipmentID is in the first cell, change 0 to the correct cell index if needed
        var zShipmentIDColumnIndex = 0;
        var zShipmentIDColumn = gridView.Columns[zShipmentIDColumnIndex];

        if (gridView.ActiveFilterString.Contains("Not(IsNullOrEmpty([ZShipmentID]))"))
        {
            gridView.ActiveFilterCriteria = null;
            gridView.ActiveFilterString = string.Empty;
        }
        else
        {
            gridView.ActiveFilterCriteria = new NotOperator(
                new FunctionOperator(FunctionOperatorType.IsNullOrEmpty,
                    new OperandProperty(zShipmentIDColumn.FieldName)));
            gridView.ActiveFilterString = "Not(IsNullOrEmpty([ZShipmentID]))";
        }

        gridView.RefreshData();
    }

    //Create a Batch
    private void barButtonItem4_ItemClick_1(object sender, ItemClickEventArgs e)
    {
        var tableName = "LabelstoPrintNECA";
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
                if (XtraMessageBox.Show(
                        "This record does not have a ShipmentID and will not generate a label. Are you sure you wish to continue?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

            manager.InsertData(gridView, columnMappings, parameterNames);

            var rowCount = gridView.GetSelectedRows().Length;
            manager.ShowRowCountMessage(rowCount);
        }

        manager.CloseConnection();
    }

    //Show Batch
    private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
    {
        var newForm = new BatchForm(_configuration, _context);
        newForm.Show();
    }



    private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
    {
        // Show the SplashScreen
        SplashScreenManager.ShowDefaultWaitForm();
        try
        {
            // Call the stored procedure
            UpdateBinSortNECA();

            // Refresh the GridView
            var gridView = gridControl1.FocusedView as GridView;

            // Refresh the GridView
            // Fetch the updated data from the database using the new EF Core method
            var data = _context.NecaOrderDatas.ToList();

            // Set the fetched data as the grid's data source and refresh the grid view
            gridView.GridControl.DataSource = data;
            gridView.RefreshData();

        }

        finally
        {
            // If SplashScreen was shown, close it
            if (SplashScreenManager.Default != null)
            {
                SplashScreenManager.CloseForm(false);
            }
        }

        // Show a message box indicating all reports were saved
        XtraMessageBox.Show("Operation was successful. Sorting by BinNumber has been completed.");
    }

    //Hold an Order
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

        // Fetch the updated data from the database using the new EF Core method
        var data = _context.NecaOrderDatas.ToList();

        // Set the fetched data as the grid's data source and refresh the grid view
        gridView.GridControl.DataSource = data;
        gridView.RefreshData();
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

    //Show Ready orders
    private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
    {
        var gridView = gridControl1.FocusedView as FileExistenceGridView;

        if (gridView != null) gridView.ToggleFileExistenceFilter();
    }
    //Check Duplicates
    private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
    {
        FilterDuplicateRows((FileExistenceGridView)gridControl1.MainView);
    }

    //SelectandProcess
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

        _reportGenerator.GenerateAndSavePreorderReportsProgressPath(salesOrderReferences,
            progress => SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetProgress, progress),
            errorMessage => SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetMessage, errorMessage)
        );

        // Ensure the splash screen is closed
        SplashScreenUtility.CloseSplashScreenIfNeeded();


        var defaultPrinterName = PrinterHelperEF.GetUserPrinter(_context, _userSession.CurrentUser.Id);


        // Call the ExecuteDefaultPrinter method and pass in the default printer name
        var programPath = "C:\\Program Files (x86)\\2Printer\\2Printer.exe";
        var printerProgram = new PrinterProgram(programPath, _configuration);
        printerProgram.ExecuteDefaultPrinter(defaultPrinterName);


        /// Read the EnablePrintLog key value from appsettings.json or other configuration source
        bool usePrintLog = bool.Parse(_configuration["EnablePrintLog"]);
        // Parses the string to a boolean. Assumes that "EnablePrintLog" exists and its value is either "true" or "false".

        // If EnablePrintLog is true, then log the user activity
        if (usePrintLog)
        {
            // Loop through each sales order reference
            foreach (var salesOrderReference in salesOrderReferences)
            {
                // Create a new user activity instance with required properties
                UserActivity userActivity = new UserActivity
                {
                    ActivityDescription = $"User {_userSession.CurrentUser.Username} printed pickslip with AccountingRef: {salesOrderReference} to {defaultPrinterName}",
                    Timestamp = DateTime.Now,
                    UserId = _userSession.CurrentUser.Id
                };

                // Add the user activity to the Entity Framework context
                _context.UserActivities.Add(userActivity);
            }

            // Commit changes to the database
            _context.SaveChanges();
        }


        // Show a message box indicating all reports were saved
        XtraMessageBox.Show($"{salesOrderReferences.Count} reports were saved successfully.");


        // Refresh the GridView
        // Fetch the updated data from the database using the new EF Core method
        var data = _context.NecaOrderDatas.ToList();

        // Set the fetched data as the grid's data source and refresh the grid view
        gridView.GridControl.DataSource = data;
        gridView.RefreshData();
    }
}