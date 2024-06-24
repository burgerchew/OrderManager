using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.DTOs;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Forms;

public partial class PreorderCardForm : RibbonForm
{
    private ExcelExporter _excelExporter;
    private readonly BulkReportGenerator _reportGenerator;
    private HttpClient client;
    private bool _dataLoaded;
    private readonly ApiKeyManager _apiKeyManager;
    private FileExistenceGridViewHelper _fileExistenceGridViewHelper;
    private readonly string _location = "RUB"; // Define your location
    private readonly IConfiguration _configuration;
    private readonly ReportManager _reportManager;
    private readonly PickSlipGenerator _pickSlipGenerator;
    private readonly OMDbContext _context;
    private readonly StoredProcedureService _storedProcedureService;
    private readonly UserSession _userSession;

    public PreorderCardForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
    {
        InitializeComponent();
        _configuration = configuration;
        _context = context;
        _userSession = userSession;

        VisibleChanged += PreordersCard_VisibleChanged;
        _excelExporter = new ExcelExporter(gridView1);


        _reportGenerator = new BulkReportGenerator(configuration);

        var connectionString =
            _configuration.GetConnectionString("RubiesConnectionString");

        _apiKeyManager = new ApiKeyManager(connectionString);

        SetUpHttpClient(_location);

        _reportManager = new ReportManager(configuration);
        _pickSlipGenerator = new PickSlipGenerator(configuration, context);
    }

    private void PreordersCard_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible && !_dataLoaded)
        {
            LoadData();
            _dataLoaded = true;
        }
    }


    private void LoadData()
    {
        // Show the default splash screen
        SplashScreenManager.ShowDefaultWaitForm("Please wait", "Loading data...");

        try
        {
            var data = _context.PreOrderCardDatas.ToList();

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

    private void UpdateFileStatusForData(List<PreOrderCardData> data)
    {
        foreach (var item in data) item.FileStatus = CustomTextConverter.Convert(item.LabelFile);
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

    private FileExistenceGridViewHelper InitializeFileExistenceHelper(FileExistenceGridView gridView)
    {
        var fileExistenceGridViewHelper = new FileExistenceGridViewHelper(gridView);
        return fileExistenceGridViewHelper;
    }

    private void HighlightDuplicateRows(FileExistenceGridView gridView)
    {
        var highlighter = new DuplicateRowHighlighter();
        highlighter.HighlightDuplicates(gridView);
    }

    private void gridView1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            gridView1.SelectAll();
            e.Handled = true; // optional: prevents other handlers from receiving this event
        }
    }

    private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        _excelExporter.ExportToXls();
    }

    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {

    }

    private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {

    }
}