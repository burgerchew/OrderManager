using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;
using DevExpress.XtraEditors.Repository;
using OrderManagerEF.Classes;

namespace OrderManagerEF.Forms
{
    public partial class EntryForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        delegate Form FormCreator(IConfiguration configuration, OMDbContext context, UserSession userSession);

        // Create a dictionary to map form names to their constructors
        private readonly Dictionary<string, FormCreator> formMap;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly UserSession _userSession;
        private List<Form> openedForms = new List<Form>();
        private string OrderRef = "Search SO Number";


        public EntryForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            navBarControl1.LinkClicked += NavBarControl1_LinkClicked;
            _userSession = userSession;

            // Initialize the formMap dictionary
            formMap = new Dictionary<string, FormCreator>
            {
                { "navBarItem1", (c, ctx,us) => new CSCForm(c, ctx,us) },
                { "navBarItem2", (c, ctx,us) => new DSForm(c, ctx,us) },
                { "navBarItem3", (c, ctx,us) => new NZForm(c, ctx,us) },
                { "navBarItem4", (c, ctx,us) => new SamplesForm(c, ctx,us) },
                { "navBarItem5", (c, ctx,us) => new PreOrdersForm(c, ctx, us) },
                { "navBarItem6", (c, ctx, us) => new WebstoreUnder5Form(c, ctx, us) },
                { "navBarItem8", (c, ctx, us) => new WebstoreOver5Form(c, ctx, us) },
                { "navBarItem7", (c, ctx, u) => new PrintedForm(c,ctx) },
                { "navBarItem9", (c, ctx, u) => new LabelPrintQueueForm(c,ctx) },
                { "navBarItem10", (c, ctx, u) => new PackingForm(c,ctx) },
                { "navBarItem11", (c, ctx, u) => new HoldForm(c,ctx) },
                { "navBarItem12", (c, ctx, u) => new Import1Form(c,ctx) },
                { "navBarItem13", (c, ctx, u) => new CreateShipmentForm(c,ctx) },
                { "navBarItem14", (c, ctx, u) => new CreateLabelForm1(c,ctx) },
                { "navBarItem15", (c, ctx, u) => new ArchiveLabelForm(c,ctx) },
                { "navBarItem16", (c, ctx, u) => new ReplenForm(c,ctx) },
                { "navBarItem17", (c, ctx, u) => new PickandPackForm(c,ctx) },
                { "navBarItem18", (c, ctx, u) => new MajorsForm(c,ctx) },
                { "navBarItem19", (c, ctx, u) => new UserForm(c,ctx) },
                { "navBarItem20", (c, ctx, u) => new PrinterSelectionForm(c,ctx,u) },
                { "navBarItem21", (c, ctx, u) => new ReportSettingForm(c,ctx) },
                { "navBarItem22", (c, ctx, us) => new ActivityLogForm(c, ctx, us) },
                { "navBarItem23", (c, ctx, us) => new ApiKeyForm(c,ctx,us) },
            };

            InitSearchForm();
        }

        private void InitSearchForm()
        {
            BarEditItem searchBar = CreateSearchBar(OrderRef);
            ribbonPageGroup2.ItemLinks.Add(searchBar);

            BarButtonItem searchButton = CreateButton(searchBar);
            ribbonPageGroup2.ItemLinks.Add(searchButton);
        }


        private BarEditItem CreateSearchBar(string OrderRef)
        {
            BarEditItem searchItem = new BarEditItem();

            // Now, you can set its value like this:
            searchItem.EditValue = OrderRef;
            RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
            edit.AutoHeight = false;
            searchItem.Width = 200; // Set the width of the BarEditItem, not the RepositoryItemTextEdit


            searchItem.Edit = edit;

            searchItem.EditValueChanged += (s, e) =>
            {
                string searchTerm = searchItem.EditValue.ToString();
                PerformSearch(searchTerm); // Call PerformSearch method whenever the search term changes
            };

            return searchItem;
        }

        private List<SearchResult> PerformSearch(string searchText)
        {
            List<SearchResult> searchResults = new List<SearchResult>();

            // Iterate through all the forms in the application
            foreach (Form form in Application.OpenForms)
            {
                // Check if the form contains a DevExpress GridView
                DevExpress.XtraGrid.GridControl gridControl = form.Controls.OfType<DevExpress.XtraGrid.GridControl>().FirstOrDefault();
                if (gridControl != null)
                {
                    // Get the GridView
                    DevExpress.XtraGrid.Views.Grid.GridView gridView = gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

                    // Iterate through all rows in the GridView
                    for (int i = 0; i < gridView.RowCount; i++)
                    {
                        // Iterate through all columns in the GridView
                        for (int j = 0; j < gridView.Columns.Count; j++)
                        {
                            // Get the cell value
                            string cellValue = gridView.GetRowCellValue(i, gridView.Columns[j])?.ToString();

                            // Check if the cell value contains the search text
                            if (!string.IsNullOrEmpty(cellValue) && cellValue.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
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
                }
            }

            return searchResults;
        }

        private BarButtonItem CreateButton(BarEditItem searchBar)
        {
            BarButtonItem button = new BarButtonItem();
            button.Caption = "Search";

            button.ItemClick += (s, e) =>
            {
                string searchText = searchBar.EditValue.ToString();
                List<SearchResult> searchResults = PerformSearch(searchText);

                if (searchResults.Count > 0)
                {
                    SearchResult firstResult = searchResults[0];
                    firstResult.Form.BringToFront();

                    DevExpress.XtraGrid.GridControl gridControl = firstResult.Form.Controls.OfType<DevExpress.XtraGrid.GridControl>().FirstOrDefault();
                    DevExpress.XtraGrid.Views.Grid.GridView gridView = gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

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
            if (formMap.TryGetValue(e.Link.Item.Name, out FormCreator formCreator))
            {
                Form formToOpen = formCreator(_configuration, _context, _userSession);

                if (formToOpen != null)
                {

                    // Remove form from list once closed
                    formToOpen.FormClosed += (sender, e) =>
                    {
                        openedForms.Remove(formToOpen);
                    };

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
            Form[] formsToClose = openedForms.ToArray();

            // Iterate over the copied array to close the forms
            foreach (var form in formsToClose)
            {
                if (form != null && !form.IsDisposed)
                {
                    form.Close();
                }
            }

            // Clear the list after closing all forms
            openedForms.Clear();
        }


        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CloseAllOpenedForms();
        }
    }
}