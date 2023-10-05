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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF
{
    public partial class PackingDetailForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly StoredProcedureService _storedProcedureService;
        public PackingDetailForm(IConfiguration configuration, string OrderRef, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            this.WindowState = FormWindowState.Maximized;
            // Assign the Form_Load event
            Load += PackingDetailForm_Load;
            //ribbonPageGroup2.ItemLinks.Add(CreateSearchBar(OrderRef));
            //ribbonPageGroup2.ItemLinks.Add(CreateButton(OrderRef));
            this.gridView1.CustomDrawCell += gridView1_CustomDrawCell;
            // Assign the OrderRef value to the Text property of the textEdit1
            BarEditItem searchBar = CreateSearchBar(OrderRef);
            ribbonPageGroup2.ItemLinks.Add(searchBar);

            BarButtonItem searchButton = CreateButton(searchBar);
            ribbonPageGroup2.ItemLinks.Add(searchButton);


            var data = context.ExecuteScanPackReportLookup(OrderRef);
            gridControl1.DataSource = data;
        }


        private void PackingDetailForm_Load(object sender, EventArgs e)
        {
            InitializeHyperLink();
        }

        private void InitializeHyperLink()
        {
            var repositoryItemHyperLinkEdit1 = new RepositoryItemHyperLinkEdit();

            repositoryItemHyperLinkEdit1.OpenLink += (sender, e) =>
            {
                var hyperlink = sender as HyperLinkEdit;
                if (hyperlink != null)
                {
                    // Get SKU from EditValue
                    var SKU = hyperlink.EditValue.ToString();

                    // Create an instance of the PickandPack form
                    var pickAndPackForm = new PickandPackForm(_configuration, _context);

                    // Set the search text and click the search button
                    pickAndPackForm.SetSearchTextAndClickButton(SKU);

                    // Show the PickandPack form
                    pickAndPackForm.Show();

                    // Set e.Handled to true to prevent the link from being opened in a browser
                    e.Handled = true;
                }
            };

            // Assuming "SKU" is the name of your grid column where you want to put the hyperlink
            gridView1.Columns["SKU"].ColumnEdit = repositoryItemHyperLinkEdit1;
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


        private BarButtonItem CreateButton(BarEditItem searchBar)
        {
            BarButtonItem button = new BarButtonItem();
            button.Caption = "Search";

            button.ItemClick += (s, e) =>
            {
                // Get the current text from the search bar and perform the search
                string searchTerm = searchBar.EditValue.ToString();
                PerformSearch(searchTerm);
            };

            return button;
        }

        private void PerformSearch(string searchTerm)
        {
            // Use DbContext to fetch data from the stored procedure
            var data = _context.ExecuteScanPackReportLookup(searchTerm);

            // Bind data to your controls
            gridControl1.DataSource = data;
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "DeliverQty" || e.Column.FieldName == "ShipQty")
            {
                int value = Convert.ToInt32(e.CellValue);
                if (value == 0)
                {
                    e.Appearance.BackColor = Color.LightPink;
                    e.Appearance.ForeColor = Color.Black;
                }

                if (value != 0)
                {
                    e.Appearance.BackColor = Color.LightGreen;
                    e.Appearance.ForeColor = Color.Black;
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}