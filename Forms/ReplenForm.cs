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
using OrderManagerEF.DTOs;

namespace OrderManagerEF.Forms
{
    public partial class ReplenForm : XtraForm
    {
        // Set up a flag to keep track of whether data has been loaded yet.
        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

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
            barEditItem4.EditValue = "DROPSHIP";      // Default value for OrderType
            barEditItem1.EditValue = 40;               // Default value for DateRange
            barEditItem3.EditValue = 0;               // Default value for retailBinThreshold

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
        }



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int sourceLocationNo = Convert.ToInt32(barEditItem2.EditValue);
            string orderType = (string)barEditItem4.EditValue;
            int dateRange = Convert.ToInt32(barEditItem1.EditValue);
            int retailBinThreshold = Convert.ToInt32(barEditItem3.EditValue);

            LoadData(sourceLocationNo, orderType, dateRange, retailBinThreshold);
        }

    }
}