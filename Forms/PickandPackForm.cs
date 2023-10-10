using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF
{
    public partial class PickandPackForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public PickandPackForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;

            this.WindowState = FormWindowState.Maximized;
            this.VisibleChanged += new EventHandler(this.PickandPack_VisibleChanged);
            gridView1.CustomDrawGroupRow += gridView_CustomDrawGroupRow;

        }

        private void PickandPack_VisibleChanged(Object sender, EventArgs e)
        {
            if (this.Visible && !_dataLoaded)
            {
                LoadData();
                LoadParamValues();
                ConfigureGridView();
                _dataLoaded = true;
            }
        }

        public void ConfigureGridView()
        {
            GridView gridView = (GridView)gridControl1.MainView;
            gridView.OptionsFind.FindFilterColumns = "ProductCode;ProductTitle";

        }




        private void LoadParamValues()
        {

            var gridView = gridControl1.FocusedView as GridView;  // Assuming you're working with a GridView
            if (gridView != null)
            {
                GroupByProductCodeAndPickType();
                gridView.ExpandAllGroups();
            }


        }


        private void LoadData()
        {
            var data = _context.BINContentsLocn1s.ToList();

            // Assuming gridControl or some other control is being populated
            gridControl1.DataSource = data;

    
        }



        // Method to group by 'ProductCode' and 'PickType' and set summaries
        private void GroupByProductCodeAndPickType()
        {
            GridView gridView = (GridView)gridControl1.MainView;

            // Clear any existing grouping
            gridView.ClearGrouping();

            // Group by 'ProductCode'
            GridColumn colProductCode = gridView.Columns["ProductCode"];
            if (colProductCode != null)
            {
                colProductCode.GroupIndex = 0;
            }

            // Group by 'PickType'
            GridColumn colPickType = gridView.Columns["PickType"];
            if (colPickType != null)
            {
                colPickType.GroupIndex = 1;
            }

            // Clear existing group summaries
            gridView.GroupSummary.Clear();

            // Add a new group summary for 'ActualQuantity' grouped by 'ProductCode'
            GridGroupSummaryItem summaryItemProductCode = new GridGroupSummaryItem()
            {
                FieldName = "ActualQuantity",
                SummaryType = DevExpress.Data.SummaryItemType.Sum,
                DisplayFormat = "Sum = {0}",
                ShowInGroupColumnFooter = gridView.Columns["ActualQuantity"]
            };
            gridView.GroupSummary.Add(summaryItemProductCode);

        }


        // Method to add the grid sum helper
        private void AddGridSumHelper()
        {
            GridView gridView = (GridView)gridControl1.MainView;
            GridViewSumHelper gridViewSumHelper = new GridViewSumHelper(gridView);

            // Sum 'ActualQuantity' for the grouped 'ProductCode'
            gridViewSumHelper.AddSumToGroupedColumn("ActualQuantity", "ProductCode");

            // Optionally, if you want to sum 'ActualQuantity' for the grouped 'PickType' as well:
            gridViewSumHelper.AddSumToGroupedColumn("ActualQuantity", "PickType");
        }



        public void SetSearchTextAndClickButton(string SKU)
        {
            // Assuming your GridControl is named gridControl1
            this.gridControl1.ForceInitialize(); // Ensures that all visual elements of the grid control are created

            // Cast MainView to GridView (which is derived from ColumnView)
            GridView view = this.gridControl1.MainView as GridView;
            if (view != null)
            {
                // Set the search string for the Find Panel
                view.FindFilterText = SKU;
            }
        }



        private void gridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.RowHandle >= 0)
            {
                int actualQuantity = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, "ActualQuantity"));

                if (actualQuantity == 0)
                {
                    e.Appearance.BackColor = Color.LightPink;
                    e.Appearance.ForeColor = Color.Black;
                }
            }
        }

       

        private void PickandPackForm_Load(object sender, EventArgs e)
        {

        }

        private void gridView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (info != null && info.Column.FieldName == "PickType")
            {
                string groupValue = view.GetGroupRowDisplayText(e.RowHandle);

                if (groupValue.Contains("PickFace"))
                {
                    info.Appearance.BackColor = Color.Green;
                    info.Appearance.ForeColor = Color.White;
                }
                else if (groupValue.Contains("Bulk"))
                {
                    info.Appearance.BackColor = Color.MediumPurple;
                    info.Appearance.ForeColor = Color.White;
                }
                else if (groupValue.Contains("Receive"))
                {
                    info.Appearance.BackColor = Color.LightBlue;
                    info.Appearance.ForeColor = Color.White;
                }
                else if (groupValue.Contains("Consolidation"))  
                {
                    info.Appearance.BackColor = Color.Orange;
                    info.Appearance.ForeColor = Color.Black;
                }
            }
        }
    }
}