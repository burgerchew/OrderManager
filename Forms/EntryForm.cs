using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using DevExpress.XtraReports.UserDesigner;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Forms;

public partial class EntryForm : RibbonForm
{
    private delegate Form FormCreator(IConfiguration configuration, OMDbContext context, UserSession userSession,
        ReplenService replenService);

    // Create a dictionary to map form names to their constructors
    private readonly Dictionary<string, FormCreator> formMap;
    private readonly IConfiguration _configuration;
    private readonly OMDbContext _context;
    private readonly UserSession _userSession;
    private readonly List<Form> openedForms = new();
    private readonly string OrderRef = "Search SO Number";
    private readonly ReplenService _replenService;
    private readonly ReportManager _reportManager;


    public EntryForm(IConfiguration configuration, OMDbContext context, UserSession userSession,
        ReplenService replenService)
    {
        InitializeComponent();
        _configuration = configuration;
        _context = context;
        navBarControl1.LinkClicked += NavBarControl1_LinkClicked;
        _userSession = userSession;
        _replenService = replenService;
        _reportManager = new ReportManager(_configuration);
        // Initialize the formMap dictionary
        formMap = new Dictionary<string, FormCreator>
        {
            { "navBarItem1", (c, ctx, us, rs) => new CSCForm(c, ctx, us) },
            { "navBarItem2", (c, ctx, us, rs) => new DSForm(c, ctx, us) },
            { "navBarItem3", (c, ctx, us, rs) => new NZForm(c, ctx, us) },
            { "navBarItem4", (c, ctx, us, rs) => new SamplesForm(c, ctx, us) },
            { "navBarItem5", (c, ctx, us, rs) => new PreOrdersForm(c, ctx, us) },
            { "navBarItem6", (c, ctx, us, rs) => new WebstoreUnder5Form(c, ctx, us) },
            { "navBarItem8", (c, ctx, us, rs) => new WebstoreOver5Form(c, ctx, us) },
            { "navBarItem7", (c, ctx, u, rs) => new PrintedForm(c, ctx) },
            { "navBarItem9", (c, ctx, u, rs) => new LabelPrintQueueForm(c, ctx) },
            { "navBarItem10", (c, ctx, us, rs) => new PackingForm(c, ctx, us) },
            { "navBarItem11", (c, ctx, u, rs) => new HoldForm(c, ctx) },
            { "navBarItem12", (c, ctx, u, rs) => new Import1Form(c, ctx) },
            { "navBarItem13", (c, ctx, u, rs) => new CreateShipmentForm(c, ctx) },
            { "navBarItem14", (c, ctx, u, rs) => new CreateLabelForm1(c, ctx) },
            { "navBarItem15", (c, ctx, u, rs) => new ArchiveLabelForm(c, ctx) },
            { "navBarItem16", (c, ctx, us, rs) => new ReplenForm(c, ctx, us, rs) },
            { "navBarItem17", (c, ctx, u, rs) => new PickandPackForm(c, ctx) },
            { "navBarItem18", (c, ctx, u, rs) => new MajorsForm(c, ctx) },
            { "navBarItem19", (c, ctx, u, rs) => new UserForm(c, ctx) },
            { "navBarItem20", (c, ctx, u, rs) => new PrinterSelectionForm(c, ctx, u) },
            { "navBarItem21", (c, ctx, u, rs) => new ReportSettingForm(c, ctx) },
            { "navBarItem22", (c, ctx, us, rs) => new ActivityLogForm(c, ctx, us) },
            { "navBarItem23", (c, ctx, us, rs) => new ApiKeyForm(c, ctx, us) },
            { "navBarItem24", (c, ctx, us, rs) => new ReplenWizardForm(c, ctx, us, rs) },
            { "navBarItem25", (c, ctx, us, rs) => new NecaForm(c, ctx, us) },
            { "navBarItem27", (c, ctx, us, rs) => new NecaB2BForm(c, ctx, us) }
        };

        InitSearchForm();
        LoadOrderChartForm();
        PopulateUsername();
        ButtonColour();
    }


    private void ButtonColour()
    {
        // Set background color

        // Set background color
        barButtonItem2.Appearance.BackColor = Color.ForestGreen;
        // Set text color
        barButtonItem2.Appearance.ForeColor = Color.White;
    }

    private void InitSearchForm()
    {
        var searchBar = CreateSearchBar(OrderRef);
        ribbonPageGroup2.ItemLinks.Add(searchBar);

        var searchButton = CreateButton(searchBar);
        ribbonPageGroup2.ItemLinks.Add(searchButton);
    }

    // Load up the default OrderChartForm
    private void LoadOrderChartForm()
    {
        try
        {
            var orderChartForm = new OrderChartForm(_configuration, _context, _userSession);
            orderChartForm.MdiParent = this;
            orderChartForm.Show();
        }
        catch (Exception ex)
        {
            // Use XtraMessage to show any errors
            XtraMessageBox.Show($"Error while loading OrderChartForm: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }


    private BarEditItem CreateSearchBar(string OrderRef)
    {
        var searchItem = new BarEditItem();

        // Now, you can set its value like this:
        searchItem.EditValue = OrderRef;
        var edit = new RepositoryItemTextEdit();
        edit.AutoHeight = false;
        searchItem.Width = 200; // Set the width of the BarEditItem, not the RepositoryItemTextEdit


        searchItem.Edit = edit;

        searchItem.EditValueChanged += (s, e) =>
        {
            var currentText = searchItem.EditValue.ToString();
            if (currentText == OrderRef)
            {
                searchItem.EditValue = string.Empty;
            }
            else
            {
                PerformSearch(currentText);
            }
        };
        return searchItem;
    }



    private List<SearchResult> PerformSearch(string searchText)
    {
        var searchResults = new List<SearchResult>();

        // Iterate through all the forms in the application
        foreach (Form form in Application.OpenForms)
        {
            // Check if the form contains a DevExpress GridView
            var gridControl =
                form.Controls.OfType<GridControl>().FirstOrDefault();
            if (gridControl != null)
            {
                // Get the GridView
                var gridView =
                    gridControl.MainView as GridView;

                // Iterate through all rows in the GridView
                for (var i = 0; i < gridView.RowCount; i++)
                    // Iterate through all columns in the GridView
                    for (var j = 0; j < gridView.Columns.Count; j++)
                    {
                        // Get the cell value
                        var cellValue = gridView.GetRowCellValue(i, gridView.Columns[j])?.ToString();

                        // Check if the cell value contains the search text
                        if (!string.IsNullOrEmpty(cellValue) &&
                            cellValue.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            // Add the search result to the list
                            searchResults.Add(new SearchResult
                            {
                                Form = form,
                                RowHandle = i,
                                Column = gridView.Columns[j]
                            });
                    }
            }
        }

        return searchResults;
    }

    private BarButtonItem CreateButton(BarEditItem searchBar)
    {
        var button = new BarButtonItem();
        button.Caption = "Search";

        // Set background and foreground colors for the button
        button.Appearance.BackColor = Color.ForestGreen;
        button.Appearance.ForeColor = Color.White;

        button.ItemClick += (s, e) =>
        {
            var searchText = searchBar.EditValue.ToString();
            var searchResults = PerformSearch(searchText);

            if (searchResults.Count > 0)
            {
                var firstResult = searchResults[0];
                firstResult.Form.BringToFront();

                var gridControl = firstResult.Form.Controls
                    .OfType<GridControl>().FirstOrDefault();
                var gridView =
                    gridControl.MainView as GridView;

                gridView.FocusedRowHandle = firstResult.RowHandle;
                gridView.FocusedColumn = firstResult.Column;

                gridView.ClearSelection();
                gridView.SelectRow(firstResult.RowHandle);

                gridView.RefreshRow(firstResult.RowHandle);
            }
            else
            {
                XtraMessageBox.Show("No results found.", "Search");
            }
        };

        return button;
    }


    private void NavBarControl1_LinkClicked(object sender, NavBarLinkEventArgs e)
    {
        // Lookup the form to open based on the clicked link's name
        if (formMap.TryGetValue(e.Link.Item.Name, out var formCreator))
        {
            var formToOpen = formCreator(_configuration, _context, _userSession, _replenService);

            if (formToOpen != null)
            {
                // Remove form from list once closed
                formToOpen.FormClosed += (sender, e) => { openedForms.Remove(formToOpen); };

                // Add the form to the list
                openedForms.Add(formToOpen);

                // Set the main form as the MDI parent
                formToOpen.MdiParent = this;

                // Show the form
                formToOpen.Show();
            }
        }
        else
        {
            // Optionally, show an error message using XtraMessage
            XtraMessageBox.Show("Invalid form name", "Error");
        }
    }


    public void CloseAllOpenedForms()
    {
        // Copy the list to an array
        var formsToClose = openedForms.ToArray();

        // Iterate over the copied array to close the forms
        foreach (var form in formsToClose)
            if (form != null && !form.IsDisposed)
                form.Close();

        // Clear the list after closing all forms
        openedForms.Clear();
    }


    private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
    {
        CloseAllOpenedForms();
    }


    private void PopulateUsername()
    {
        if (_userSession?.CurrentUser != null)
            barStaticItem1.Caption = $"Logged in as: {_userSession.CurrentUser.Username}";
        else
            barStaticItem1.Caption = "Not logged in";
    }

    private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            // Get the sales order reference from BarEditItem
            string salesOrderRef = barEditItem1.EditValue?.ToString();

            // Validate sales order reference
            if (string.IsNullOrEmpty(salesOrderRef))
            {
                XtraMessageBox.Show("Please enter a sales order reference.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Retrieve the report setting
            ReportSetting reportSetting = _reportManager.GetReportSetting();

            // Get the default printer for the current user
            string defaultPrinterName = PrinterHelperEF.GetUserPrinter(_context, _userSession.CurrentUser.Id);

            // Validate default printer
            if (string.IsNullOrEmpty(defaultPrinterName))
            {
                XtraMessageBox.Show("No default printer is set up. Please set a default printer before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use the original pickSlipPath without adding "archive" here
            string originalPickSlipPath = reportSetting.PickSlipPath;

            // Append "archive" to the original path
            string archivePath = Path.Combine(originalPickSlipPath, "archive", salesOrderRef);

            // Add ".pdf" extension
            string pdfPath = Path.ChangeExtension(archivePath, "pdf");

            if (!File.Exists(pdfPath))
            {
                XtraMessageBox.Show("PickSlip does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Execute the quick print
            var programPath = "C:\\Program Files (x86)\\2Printer\\2Printer.exe";
            PrinterProgram printerProgram = new PrinterProgram(programPath, _configuration);

            // Use originalPickSlipPath; the ExecuteDefaultPrinterQuickPrint method will handle adding "archive"
            printerProgram.ExecuteDefaultPrinterQuickPrint(defaultPrinterName, originalPickSlipPath, salesOrderRef);

            // Show success message
            XtraMessageBox.Show("Pick slip printed successfully.", "Success", MessageBoxButtons.OK);
        }
        catch (Exception ex)
        {
            // Show error message
            XtraMessageBox.Show($"Failed to print pick slip: {ex.Message}", "Error", MessageBoxButtons.OK);
        }
    }

    private void barButtonItem4_ItemClick_1(object sender, ItemClickEventArgs e)
    {
  

        try
        {
            // Get the sales order range from BarEditItem fields
            string startSalesOrderRef = barEditItem2.EditValue?.ToString();
            string endSalesOrderRef = barEditItem3.EditValue?.ToString();

            // Validate sales order references
            if (string.IsNullOrEmpty(startSalesOrderRef) || string.IsNullOrEmpty(endSalesOrderRef))
            {
                XtraMessageBox.Show("Please enter both start and end sales order references.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Ensure the start value is less than or equal to the end value
            if (string.Compare(startSalesOrderRef, endSalesOrderRef, StringComparison.Ordinal) > 0)
            {
                XtraMessageBox.Show("The start sales order reference must be less than or equal to the end sales order reference.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Retrieve the report setting
            ReportSetting reportSetting = _reportManager.GetReportSetting();

            // Get the default printer for the current user
            string defaultPrinterName = PrinterHelperEF.GetUserPrinter(_context, _userSession.CurrentUser.Id);

            // Validate default printer
            if (string.IsNullOrEmpty(defaultPrinterName))
            {
                XtraMessageBox.Show("No default printer is set up. Please set a default printer before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use the original pickSlipPath without adding "archive" here
            string originalPickSlipPath = reportSetting.PickSlipPath;
            // Execute the quick print
            var programPath = "C:\\Program Files (x86)\\2Printer\\2Printer.exe";
            PrinterProgram printerProgram = new PrinterProgram(programPath, _configuration);
            printerProgram.ExecuteDefaultPrinterQuickPrintRange(defaultPrinterName, originalPickSlipPath, startSalesOrderRef, endSalesOrderRef);

            // Show success message
            XtraMessageBox.Show("Pick slips printed successfully.", "Success", MessageBoxButtons.OK);
        }
        catch (Exception ex)
        {
            // Show error message
            XtraMessageBox.Show($"Failed to print pick slip: {ex.Message}", "Error", MessageBoxButtons.OK);
        }
    }



    private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            // Get the sales order range from BarEditItem
            string salesOrderRange = barEditItem1.EditValue?.ToString();

            // Validate sales order range
            if (string.IsNullOrEmpty(salesOrderRange))
            {
                XtraMessageBox.Show("Please enter a sales order range.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Split the range into start and end
            var rangeParts = salesOrderRange.Split('-');
            if (rangeParts.Length != 2)
            {
                XtraMessageBox.Show("Invalid sales order range format.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Extract start and end order numbers from the range
            int startOrder, endOrder;
            if (!int.TryParse(rangeParts[0].Substring(2), out startOrder) || !int.TryParse(rangeParts[1].Substring(2), out endOrder))
            {
                XtraMessageBox.Show("Invalid sales order numbers.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Check if range is valid
            if (startOrder > endOrder)
            {
                XtraMessageBox.Show("Start order number must be less than end order number.", "Error", MessageBoxButtons.OK);
                return;
            }

            // Calculate the number of pickslips
            int numberOfPickslips = endOrder - startOrder + 1;

            // Confirm with the user
            var result = XtraMessageBox.Show($"Are you sure you want to print {numberOfPickslips} pickslips?", "Confirm", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes)
            {
                return;
            }

            // Retrieve the report setting
            ReportSetting reportSetting = _reportManager.GetReportSetting();

            // Get the default printer for the current user
            string defaultPrinterName = PrinterHelperEF.GetUserPrinter(_context, _userSession.CurrentUser.Id);

            for (int i = startOrder; i <= endOrder; i++)
            {
                string salesOrderRef = "SO" + i.ToString("D6"); // Format the sales order reference



                // Use the original pickSlipPath without adding "archive" here
                string originalPickSlipPath = reportSetting.PickSlipPath;

                // Append "archive" and the sales order reference to the original path
                string archivePath = Path.Combine(originalPickSlipPath, "archive", salesOrderRef);

                // Add ".pdf" extension to the path
                string pdfPath = Path.ChangeExtension(archivePath, "pdf");

                // Check if the pickslip file exists
                if (!File.Exists(pdfPath))
                {
                    XtraMessageBox.Show($"PickSlip {salesOrderRef} does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue; // Skip to the next iteration if the pickslip doesn't exist
                }

                // Execute the quick print
                var programPath = "C:\\Program Files (x86)\\2Printer\\2Printer.exe";
                PrinterProgram printerProgram = new PrinterProgram(programPath, _configuration);

                // Execute the printing for the current sales order reference
                printerProgram.ExecuteDefaultPrinterQuickPrint(defaultPrinterName, originalPickSlipPath, salesOrderRef);

                // Optionally, you can add a delay or a success message for each printed pickslip here
            }

            // Show success message for the entire range
            XtraMessageBox.Show("Pick slips printed successfully.", "Success", MessageBoxButtons.OK);
        }
        catch (Exception ex)
        {
            // Show error message
            XtraMessageBox.Show($"Failed to print pick slips: {ex.Message}", "Error", MessageBoxButtons.OK);
        }
    }


}