using DevExpress.XtraEditors;
using Microsoft.EntityFrameworkCore;
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
using DevExpress.XtraGrid.Views.Grid;
using OrderManagerEF.Classes;
using OrderManagerEF.DTOs;
using OrderManagerEF.Entities;

namespace OrderManagerEF
{
    public partial class ReplenWizardForm : DevExpress.XtraEditors.XtraForm
    {

        private IConfiguration _configuration;
        private OMDbContext _context;
        private UserSession _userSession;
        private ReplenService _replenService;

        public ReplenWizardForm(IConfiguration configuration, OMDbContext context, UserSession userSession, ReplenService replenService)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            _userSession = userSession;
            _replenService = replenService;
            ConfigureMasterDetailGrid();
        }

        private void ConfigureMasterDetailGrid()
        {
            // Load data with details included
            var orders = _context.ReplenHeaders.Include(s => s.ReplenDetails).ToList();

            // Bind master data (orders) to grid
            gridControl1.DataSource = orders;

            // Configure master GridView
            GridView masterView = gridControl1.MainView as GridView;
            masterView.OptionsDetail.EnableMasterViewMode = true;

            masterView.MasterRowExpanded += MasterView_MasterRowExpanded;

            // Handle the master-detail relationship
            masterView.MasterRowGetChildList += (sender, e) =>
            {
                GridView view = sender as GridView;
                ReplenHeader order = view.GetRow(e.RowHandle) as ReplenHeader;
                e.ChildList = order.ReplenDetails.ToList(); // Cast to list
            };

            masterView.MasterRowGetRelationName += (sender, e) => { e.RelationName = "ReplenDetails"; };

            masterView.MasterRowGetRelationCount += (sender, e) => { e.RelationCount = 1; };

            // Create and configure detail GridView
            GridView detailView = new GridView(gridControl1);
            gridControl1.LevelTree.Nodes.Add("ReplenDetails", detailView);
            detailView.PopulateColumns();
            detailView.OptionsView.ShowGroupPanel = false;
        }

        private void MasterView_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            GridView detailView = masterView.GetDetailView(e.RowHandle, e.RelationIndex) as GridView;

            if (detailView != null)
            {
                detailView.OptionsBehavior.Editable = true;
                // Subscribe to the RowUpdated event for the detail view
                detailView.RowUpdated += GridView1_RowUpdated;
            }
        }

        private void GridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            // Check the type of the updated row and update it in the database
            if (e.Row is ReplenHeader header)
            {
                _context.ReplenHeaders.Update(header);
            }
            else if (e.Row is ReplenDetail detail)
            {
                _context.ReplenDetails.Update(detail);
            }

            // Save changes to database
            _context.SaveChanges();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Get the GridView attached to the GridControl
                GridView gridView = gridControl1.MainView as GridView;

                // Count the rows to be processed
                int rowCount = gridView.RowCount;

                // Loop through rows in the GridView to delete ReplenHeader and ReplenDetail data
                for (int i = rowCount - 1; i >= 0; i--)  // loop backwards to safely remove rows
                {
                    object row = gridView.GetRow(i);

                    if (row is ReplenHeader header)
                    {
                        _context.ReplenHeaders.Remove(header);
                    }
                    else if (row is ReplenDetail detail)
                    {
                        _context.ReplenDetails.Remove(detail);
                    }
                }

                // Save changes to the database
                _context.SaveChanges();

                // Refresh the grid view data source to reflect changes
                gridView.RefreshData();

                // Show a success message
                XtraMessageBox.Show($"Successfully deleted {rowCount} rows.");
            }
            catch (Exception ex)
            {
                // Show an error message
                XtraMessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

    }
}