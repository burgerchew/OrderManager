using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Microsoft.Extensions.Configuration;
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
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using OrderManagerEF.DTOs;
using OrderManagerEF.Classes;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;

namespace OrderManagerEF.Forms
{
    public partial class ReplenForm : XtraForm
    {
        // Set up a flag to keep track of whether data has been loaded yet.
        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly ExcelExporter _excelExporter;

        public ReplenForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            this.VisibleChanged += Replen_VisibleChanged;
            repositoryItemComboBox1.Items.AddRange(new object[] { 1, 11 });
            repositoryItemComboBox2.Items.AddRange(new object[] { "DROPSHIP", "REGULAR", "PREORDER" });

            // Set the default values for each BarEditItem
            barEditItem2.EditValue = 1;               // Default value for sourceLocationNo
            barEditItem4.EditValue = "REGULAR";      // Default value for OrderType
            barEditItem1.EditValue = 60;               // Default value for DateRange
            barEditItem3.EditValue = 0;               // Default value for retailBinThreshold
            _excelExporter = new ExcelExporter(gridView1);
   
        }

        private void Replen_VisibleChanged(object sender, EventArgs e)
        {
            // Load data when the form is visible, but only if it hasn't been loaded already.
            if (this.Visible && !_dataLoaded)
            {
                LoadData();

                _dataLoaded = true;
            }
        }
        private async void LoadData(int sourceLocationNo = 1, string orderType = "DROPSHIP", int dateRange = 7, int retailBinThreshold = 0)
        {
            var results = await _context.GetReplenishmentDataAsync(sourceLocationNo, orderType, dateRange, retailBinThreshold);
            gridControl1.DataSource = results;
            LoadParamValues();
            InitializeHyperLink();
        }

        private void LoadParamValues()
        {

            var gridView = gridControl1.FocusedView as GridView;  // Assuming you're working with a GridView
            if (gridView != null)
            {
                GroupByParamValues(gridView);
                gridView.ExpandAllGroups();
            }


        }



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int sourceLocationNo = Convert.ToInt32(barEditItem2.EditValue);
            string orderType = (string)barEditItem4.EditValue;
            int dateRange = Convert.ToInt32(barEditItem1.EditValue);
            int retailBinThreshold = Convert.ToInt32(barEditItem3.EditValue);

            LoadData(sourceLocationNo, orderType, dateRange, retailBinThreshold);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _excelExporter.ExportToXls();
        }

        private void GroupByParamValues(GridView view)
        {
            // Clear any existing grouping
            view.ClearGrouping();

            // Group by CustomerCode column
            GridColumn colCustomerCode = view.Columns["CustomerCode"];
            if (colCustomerCode != null)
            {
                colCustomerCode.GroupIndex = 0;  // This will group by CustomerCode first
            }

            // Group by AccountingRef column
            GridColumn colAccountingRef = view.Columns["AccountingRef"];
            if (colAccountingRef != null)
            {
                colAccountingRef.GroupIndex = 1;  // This will group by AccountingRef next
            }
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
                    var pickAndPackForm = new PickandPackForm(_configuration, _context ?? throw new ArgumentNullException(nameof(_context)));

                    // Set the search text and click the search button
                    pickAndPackForm.SetSearchTextAndClickButton(SKU);

                    // Show the PickandPack form
                    pickAndPackForm.Show();

                    // Set e.Handled to true to prevent the link from being opened in a browser
                    e.Handled = true;
                }
            };

            // Assuming "SKU" is the name of your grid column where you want to put the hyperlink
            gridView1.Columns["ProductCode"].ColumnEdit = repositoryItemHyperLinkEdit1;
        }

    }
}