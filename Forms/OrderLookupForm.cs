﻿using DevExpress.XtraEditors;
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
using DevExpress.XtraScheduler.Reporting;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace OrderManagerEF.Forms
{
    public partial class OrderLookupForm : XtraForm
    {

        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public OrderLookupForm(IConfiguration configuration, OMDbContext context, string OrderRef)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            this.WindowState = FormWindowState.Maximized;


            var data = _context.ExecuteOrderLookupResult(OrderRef);
            gridControl1.DataSource = data;
            InitializeHyperLink();
            LoadParamValues();
            gridView1.CustomDrawGroupRow += gridView_CustomDrawGroupRow;

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
            gridView1.Columns["SKU"].ColumnEdit = repositoryItemHyperLinkEdit1;
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


        private void GroupByParamValues(GridView view)
        {
            // Clear any existing grouping
            view.ClearGrouping();

            GridColumn colDefaultBinQtyCheck = view.Columns["DefaultBinQtyCheck"];
            if (colDefaultBinQtyCheck != null)
            {
                colDefaultBinQtyCheck.GroupIndex = 0;  
            }

        }

        private void gridView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (info != null && info.Column.FieldName == "DefaultBinQtyCheck")
            {
                string groupValue = view.GetGroupRowDisplayText(e.RowHandle);

                if (groupValue.Contains("EnoughQtytoInvoice"))
                {
                    info.Appearance.BackColor = Color.Green;
                    info.Appearance.ForeColor = Color.White;
                }
                else if (groupValue.Contains("TransferRequired"))
                {
                    info.Appearance.BackColor = Color.Red;
                    info.Appearance.ForeColor = Color.White;
                }
            }
        }

    }
}