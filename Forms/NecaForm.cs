using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraSplashScreen;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;
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
using DevExpress.XtraBars;
using OrderManagerEF.Forms;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json.Linq;
using OrderManagerEF.DTOs;

namespace OrderManagerEF
{
    public partial class NecaForm : DevExpress.XtraEditors.XtraForm
    {

        private ExcelExporter _excelExporter;
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
                // Read the UseMergeTable key value from appsettings.json or other configuration source
                bool useMergeTable = bool.Parse(_configuration["UseMergeTable"]);


                // If UseMergeTable is true, then load pick slip data
                if (useMergeTable)
                {
                    LoadPickSlipData();
                }
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

        private void UpdateFileStatusForData(List<NecaOrderData> data)
        {
            foreach (var item in data)
            {
                item.FileStatus = CustomTextConverter.Convert(item.LabelFile);
            }
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
            if (gridView != null)
            {
                gridView.Columns["AccountingRef"].ColumnEdit = repositoryItemHyperLinkEdit1;
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

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tableName = "LabelstoPrintNECA";
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

                XtraMessageBox.Show("Sync and update operation was a success!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Log error
                XtraMessageBox.Show($"An error occurred during the sync and update operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartImportTask();
        }

        private void StartImportTask()
        {
            try
            {
                // Fetch server and job name from configuration
                var serverName = _configuration["ServerName1"];  // 
                var jobName = _configuration["JobName1"];  // 

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
    }
}