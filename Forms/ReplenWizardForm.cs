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
using OrderManagerEF.Forms;

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
            detailView.DoubleClick += DetailView_DoubleClick;
            detailView.KeyDown += DetailView_KeyDown;
        }


        // Method to handle KeyDown event on detailView
        private void DetailView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var detailView = sender as GridView;
                if (detailView != null)
                {
                    // Confirm deletion
                    DialogResult dialogResult = XtraMessageBox.Show("Are you sure you want to delete this row?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int rowHandle = detailView.FocusedRowHandle;
                        if (rowHandle >= 0)
                        {
                            // Delete the row from the context and update the database if needed
                            var detailRow = detailView.GetRow(rowHandle) as ReplenDetail; // Adjust the type
                            if (detailRow != null)
                            {
                                // Assuming _context is your DbContext
                                _context.ReplenDetails.Remove(detailRow);
                                _context.SaveChanges();

                                // Refresh the data source or remove the row from the grid
                                detailView.DeleteRow(rowHandle);
                            }
                        }
                    }
                }
            }
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
                RefreshGridData();
                // Show a success message
                XtraMessageBox.Show($"Successfully deleted {rowCount} rows.");
            }
            catch (Exception ex)
            {
                // Show an error message
                XtraMessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void DetailView_DoubleClick(object sender, EventArgs e)
        {
            var detailView = gridControl1.FocusedView as GridView;

            if (detailView != null)
            {
                var focusedRowHandle = detailView.FocusedRowHandle;
                if (focusedRowHandle >= 0) // Check if a row is focused
                {
                    // Get the parent row handle from the detail view
                    int parentRowHandle = detailView.SourceRowHandle;

                    // Get the master view
                    GridView masterView = detailView.ParentView as GridView;

                    // Fetch the WarehouseLocationID from the master row
                    var warehouseLocationID = (int)masterView.GetRowCellValue(parentRowHandle, "WarehouseId");

                    // Continue fetching detail row-specific information
                    var productCode = (string)detailView.GetRowCellValue(focusedRowHandle, "ProductCode");
                    var transferDetailID = (int)detailView.GetRowCellValue(focusedRowHandle, "Id");

                    // Load the new form here and set the properties
                    var binTransferForm = new BinLookupForm(_configuration, _context, warehouseLocationID, productCode, transferDetailID);
                    binTransferForm.BinNumberSelected += BinTransferForm_BinNumberSelected;
                    binTransferForm.Show();
                }
                else
                {
                    XtraMessageBox.Show("No row is currently focused.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Detail view could not be retrieved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BinTransferForm_BinNumberSelected(string selectedColumnName, string selectedBinNumber
            )
        {
            var detailView = gridControl1.FocusedView as GridView;
            if (detailView != null)
            {
                var focusedRowHandle = detailView.FocusedRowHandle;
                if (focusedRowHandle >= 0)
                {
                    detailView.SetRowCellValue(focusedRowHandle, selectedColumnName, selectedBinNumber);

                    // Determine the column name for the BinID based on the selected column name for the bin number
                    // var binIDColumnName = selectedColumnName.Replace("Number", "ID");

                    // Set the value in the BinID column
                    detailView.SetRowCellValue(focusedRowHandle, selectedColumnName, selectedBinNumber);
                }
            }
        }

        private void RefreshGridData()
        {
            // Load updated data from the database
            var orders = _context.ReplenHeaders.Include(s => s.ReplenDetails).ToList();

            // Bind the updated data to gridControl1
            gridControl1.DataSource = orders;

            // Refresh the MainView to update the UI
            (gridControl1.MainView as GridView).RefreshData();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraMessageBox.Show("This feature is not implemented yet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ValidateInventoryQty(GridView detailView)
        {
            bool allRowsValid = true;

            {



                List<string> errorMessages = new List<string>();

                var replenHeader = _context.ReplenHeaders.FirstOrDefault();
                int WarehouseId = replenHeader.WarehouseId;

                if (WarehouseId ==null)
                {
                    XtraMessageBox.Show("Warehouse ID is missing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                for (var i = 0; i < detailView.DataRowCount; i++)
                {
                    var row = detailView.GetRow(i);

                    if (row is ReplenDetail detail)
                    {
                        var selectedBinDetail = (from p in _context.Products  // Start with the Products table
                            join pbc in _context.PBinContents on p.UniqueID equals pbc.ProductID  // Join with PBinContents on the ProductID field
                            join b in _context.PBins on pbc.BinID equals b.BinID  // Join with PBins on the BinID field
                            where b.Location == WarehouseId  // Filter by the warehouse location ID
                                  && b.Type != "A"  // Exclude bins of Type "A"
                                  && p.ProductCode == detail.ProductCode  // Filter by the product code
                            select new BinDetail
                            {
                                BinID = pbc.BinID,
                                BinNumber = b.BinNumber,  // Assuming that BinNumber is a property of the PBins table
                                ActualQuantity = pbc.ActualQuantity
                            }).SingleOrDefault();



                        if (selectedBinDetail != null)
                        {
                            if (selectedBinDetail.ActualQuantity < detail.Qty)
                            {
                                // Adding error message to the list
                                errorMessages.Add($"Row {i + 1}: Transfer quantity exceeds the available quantity in bin. Available: {selectedBinDetail.ActualQuantity}, Requested: {detail.Qty}");
                                allRowsValid = false;
                            }
                        }
                        else
                        {
                         
                            errorMessages.Add($"Row {i + 1}: Bin details not found for the Product: {detail.ProductCode} and Bin Number: {detail.FromLocation}.");
                            allRowsValid = false;
                        }
                    }
                }

                // If there are any errors, show them in a message box
                if (errorMessages.Any())
                {
                    XtraMessageBox.Show(string.Join(Environment.NewLine, errorMessages), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    XtraMessageBox.Show("All rows are valid.", "Validation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return allRowsValid;
        }
    }
}