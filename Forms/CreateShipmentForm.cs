using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.DTOs;
using Label = OrderManagerEF.Entities.Label;

namespace OrderManagerEF;

public partial class CreateShipmentForm : RibbonForm
{
    // Declare your DataSet and TableAdapters.
    private readonly IConfiguration _configuration;
    private readonly OMDbContext _context;


    public CreateShipmentForm(IConfiguration configuration, OMDbContext context)
    {
        InitializeComponent();

        _configuration = configuration;
        _context = context;
        Load += CreateShipmentForm_Load;
    }


    private void BarButtonClick()
    {
        //AddressMode
        barButtonItem2.ItemClick += barButtonItem2_ItemClick;
        //Create Shipments
        barButtonItem3.ItemClick += barButtonItem3_ItemClick;
        //Delete Shipments
        barButtonItem4.ItemClick += barButtonItem4_ItemClick;
    }


    private void CreateShipmentForm_Load(object sender, EventArgs e)
    {
        ConfigureMasterDetailGrid();

        // Refresh the grid with latest data from the database
        RefreshGridView();

        ApplyShipmentIdFilter();
        PopulateExtraDataLookup(gridControl1.MainView as GridView);
    }

    private void ConfigureMasterDetailGrid()
    {
        // Load data with details included
        var orders = _context.StarShipITOrders.Include(s => s.StarShipITOrderDetails).ToList();

        // Bind master data (orders) to grid
        gridControl1.DataSource = orders;

        // Configure master GridView
        var masterView = gridControl1.MainView as GridView;
        masterView.OptionsDetail.EnableMasterViewMode = true;

        masterView.MasterRowExpanded += MasterView_MasterRowExpanded;

        // Handle the master-detail relationship
        masterView.MasterRowGetChildList += (sender, e) =>
        {
            var view = sender as GridView;
            var order = view.GetRow(e.RowHandle) as StarShipITOrder;
            e.ChildList = order.StarShipITOrderDetails.ToList(); // Cast to list
        };

        masterView.MasterRowGetRelationName += (sender, e) => { e.RelationName = "StarShipITOrderDetails"; };

        masterView.MasterRowGetRelationCount += (sender, e) => { e.RelationCount = 1; };

        // Create and configure detail GridView
        var detailView = new GridView(gridControl1);
        gridControl1.LevelTree.Nodes.Add("StarShipITOrderDetails", detailView);
        detailView.PopulateColumns();
        detailView.OptionsView.ShowGroupPanel = false;


        BarButtonClick();
        gridView1.CustomDrawCell += gridView1_CustomDrawCell;

        gridView1.RowUpdated += gridView1_RowUpdated;
        gridView1.RowDeleted += gridView1_RowDeleted;
        detailView.RowUpdated += detailView_RowUpdated;
        gridView1.KeyDown += gridView1_KeyDown;
    }


    private void MasterView_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
    {
        var masterView = sender as GridView;
        var detailView = masterView.GetDetailView(e.RowHandle, e.RelationIndex) as GridView;

        if (detailView != null)
        {
            detailView.OptionsBehavior.Editable = true;
            // Subscribe to the RowUpdated event for the detail view
            detailView.RowUpdated += GridView1_RowUpdated;
        }
    }

    private void GridView1_RowUpdated(object sender, RowObjectEventArgs e)
    {
        // Commit any changes to the row to ensure that they are updated in the data source.
        var view = sender as GridView;
        view.CloseEditor();
        view.UpdateCurrentRow();

        try
        {
            // Check the type of the updated row and update it in the database
            if (e.Row is StarShipITOrder header)
                _context.StarShipITOrders.Update(header);
            else if (e.Row is StarShipITOrderDetail detail) _context.StarShipITOrderDetails.Update(detail);

            // Save changes to database
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"An error occurred while saving: {ex.Message}", "Save Failed", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }


    private void PopulateExtraDataLookup(GridView gridView)
    {
        // Fetch the distinct ExtraData values directly from the database
        var extraDataValues = _context.StarShipITOrders
            .Select(s => s.ExtraData)
            .Distinct()
            .ToList();

        // Add default values, if they don't exist in the database
        var defaultValues = new[] { "CSC", "BSA", "DS", "RUB" };
        foreach (var defaultValue in defaultValues)
            if (!extraDataValues.Contains(defaultValue))
                extraDataValues.Add(defaultValue);

        var lookupTable = new DataTable();
        lookupTable.Columns.Add("ID", typeof(string));
        lookupTable.Columns.Add("Value", typeof(string));

        foreach (var value in extraDataValues)
        {
            var newRow = lookupTable.NewRow();
            newRow["ID"] = value;
            newRow["Value"] = value;
            lookupTable.Rows.Add(newRow);
        }

        var repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
        repositoryItemLookUpEdit.DataSource = lookupTable;
        repositoryItemLookUpEdit.ValueMember = "ID";
        repositoryItemLookUpEdit.DisplayMember = "Value";

        gridView.Columns["ExtraData"].ColumnEdit = repositoryItemLookUpEdit;
    }


    private void ApplyShipmentIdFilter()
    {
        // Apply a filter to show only the rows where ShipmentID is null or blank
        gridView1.ActiveFilterString = "[ShipmentID] IS NULL OR [ShipmentID] = ''";
    }


    private void SaveChanges()
    {
        // Ensure any UI-based edits are committed in the grid views
        gridView1.CloseEditor();
        gridView1.UpdateCurrentRow();

        var detailView = gridView1.GetDetailView(gridView1.FocusedRowHandle, 0) as GridView;
        if (detailView != null)
        {
            detailView.CloseEditor();
            detailView.UpdateCurrentRow();
        }

        // Commit changes to the database
        _context.SaveChanges();
    }

    private void SaveAllChanges()
    {
        // Update the current row to ensure all changes are committed to the data source.
        var masterView = gridControl1.MainView as GridView;
        masterView.CloseEditor();
        masterView.UpdateCurrentRow();

        // Do the same for all detail views
        for (var i = 0; i < masterView.RowCount; i++)
            if (masterView.IsMasterRow(i))
            {
                var detailView = masterView.GetDetailView(i, 0) as GridView;
                if (detailView != null)
                {
                    detailView.CloseEditor();
                    detailView.UpdateCurrentRow();
                }
            }

        try
        {
            // Save changes to the database
            _context.SaveChanges();

            XtraMessageBox.Show("All changes saved successfully!", "Save Successful", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"An error occurred while saving: {ex.Message}", "Save Failed", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }


    private void gridView1_RowUpdated(object sender, RowObjectEventArgs e)
    {
        SaveChanges();
    }

    private void detailView_RowUpdated(object sender, RowObjectEventArgs e)
    {
        SaveChanges();
    }

    private void gridView1_RowDeleted(object sender, RowDeletedEventArgs e)
    {
        // Save changes to the database
        SaveChanges();
    }


    private (string StarshipItApiKey, string OcpApimSubscriptionKey) GetApiKeysFromTableAdapter(string location)
    {
        var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
        string starshipItApiKey = null;
        string ocpApimSubscriptionKey = null;

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (var command =
                   new SqlCommand(
                       "SELECT [StarShipIT_Api_Key], Ocp_Apim_Subscription_Key FROM StarShipITAPIKeyManager WHERE Location = @Location",
                       connection))
            {
                command.Parameters.AddWithValue("@Location", location);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        starshipItApiKey = reader["StarShipIT_Api_Key"].ToString();
                        ocpApimSubscriptionKey = reader["Ocp_Apim_Subscription_Key"].ToString();
                    }
                }
            }
        }

        return (starshipItApiKey, ocpApimSubscriptionKey);
    }


    public void UpdateTransHeadersWithShipmentId(int orderId, string orderNumber)
    {
        var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (var command =
                   new SqlCommand(
                       "UPDATE TransHeaders SET ZShipmentID = @orderId WHERE TradingRef = @orderNumber and outstanding =1 and transactiontype='CO'",
                       connection))
            {
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateOrdersWithShipmentId(int orderId, string orderNumber, string reference)
    {
        var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (var command =
                   new SqlCommand(
                       "UPDATE StarShipITOrders SET ShipmentID = @orderId WHERE OrderNumber = @orderNumber and reference = @reference",
                       connection))
            {
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@orderNumber", orderNumber);
                command.Parameters.AddWithValue("@reference", reference);
                command.ExecuteNonQuery();
            }
        }
    }


    private async void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
    {
        var selectedShipments = _context.StarShipITOrders
            .Include(s => s.StarShipITOrderDetails) // To load related data
            .Where(s => (bool)s.Selected && s.ShipmentID == null)
            .ToList();

        if (!selectedShipments.Any())
        {
            XtraMessageBox.Show("Please select at least one row before proceeding.", "No Rows Selected",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var shipmentResponses = await ProcessShipments(selectedShipments);

        var successfulShipmentsCount = ProcessResponses(shipmentResponses);

        // Inform the user about the successful shipments
        XtraMessageBox.Show(
            $"Total {successfulShipmentsCount} shipments were successful. Please check the label tab in a moment.",
            "Shipments Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);

        RefreshGridView();
    }


    private async Task<List<ShipmentResponse>> ProcessShipments(List<StarShipITOrder> orders)
    {
        var shipmentResponses = new List<ShipmentResponse>();
        var ordersGroupedByLocation = orders.GroupBy(o => o.ExtraData);
        var mapper = new StarShipOrderMapper();

        SplashScreenManager.ShowForm(typeof(ProgressForm));
        var totalRows = orders.Count;

        var progress = new Progress<int>(value =>
        {
            if (SplashScreenManager.Default != null)
                SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetProgress, value);
        });

        foreach (var ordersGroup in ordersGroupedByLocation)
        {
            var extraData = ordersGroup.Key;
            var (starshipItApiKey, ocpApimSubscriptionKey) = GetApiKeysFromTableAdapter(extraData);
            var shipmentManager = new ShipmentManager(starshipItApiKey, ocpApimSubscriptionKey);

            var mappedOrders = ordersGroup.Select(mapper.MapToOrder).ToList();

            // Here's the change: For each StarShipITOrder, map its details to a list of OrderDetail
            var mappedOrderDetailsGrouped = ordersGroup
                .Select(o => mapper.MapToOrderDetails(o.StarShipITOrderDetails.ToList()).ToList()).ToList();

            var singleGroupResponses =
                await shipmentManager.CreateShipments(mappedOrders, mappedOrderDetailsGrouped, progress, totalRows);

            foreach (var resp in singleGroupResponses)
            {
                resp.ExtraData = extraData;
                resp.Location = extraData;
            }


            shipmentResponses.AddRange(singleGroupResponses);
        }

        SplashScreenManager.CloseForm();
        return shipmentResponses;

    }


    private int ProcessResponses(List<ShipmentResponse> responses)
    {
        var successfulShipmentsCount = 0;

        foreach (var response in responses)
            if (response.Success)
            {
                // Note: Here I'm assuming that the Order object in your ShipmentResponse closely matches your Shipment EF entity
                // This will need adjusting based on your actual model and what the CreateShipments() method returns.
                var orderResponse = response.Order;
                if (orderResponse != null)
                {
                    successfulShipmentsCount++;

                    var addressValidatedValue = orderResponse.Metadatas
                        ?.FirstOrDefault(m => m.metafield_key == "ADDRESSVALIDATED")?.Value;

                    if (orderResponse.Packages != null)
                        foreach (var package in orderResponse.Packages)
                        {
                            // Replace your TableAdapter insert method with an EF Add method
                            var newLabel = new Label
                            {
                                OrderId = orderResponse.order_id,
                                OrderDate = orderResponse.OrderDate,
                                Reference = orderResponse.Reference,
                                OrderNumber = orderResponse.order_number,
                                AddressValidatedKey = "ADDRESSVALIDATED",
                                AddressValidatedValue = addressValidatedValue,
                                Weight = (decimal?)package.Height,
                                Height = (decimal?)package.Height,
                                Length = (decimal?)package.Length,
                                Width = (decimal?)package.Width,
                                Location = response.Location,
                                Selected = true,
                                ExtraData = response.ExtraData,
                                Exported = false
                            };
                            _context.Labels.Add(newLabel);

                            UpdateTransHeadersWithShipmentId(orderResponse.order_id, orderResponse.order_number);
                            UpdateOrdersWithShipmentId(orderResponse.order_id, orderResponse.order_number,
                                orderResponse.Reference);

                            if (addressValidatedValue == "true") DeleteOrderAndOrderDetails(orderResponse.order_id);
                        }

                    _context.SaveChanges();
                }
            }
            else
            {
                XtraMessageBox.Show(
                    $"Shipment for order {response.Order?.order_number ?? "unknown"} was unsuccessful. Please check the order exists and try again.",
                    "Shipment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        return successfulShipmentsCount;
    }

    // Attach this event handler to the KeyDown event of the GridView
    private void gridView1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            var view = sender as GridView;
            if (view != null)
            {
                // Show a message box asking the user if they want to delete the row
                var result = XtraMessageBox.Show(
                    "Are you sure you want to delete the selected row and its details?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // If the user clicks 'No', exit the event handler
                if (result == DialogResult.No) return;

                var focusedRow = view.GetRow(view.FocusedRowHandle);

                // Check if it's a master row
                if (focusedRow is StarShipITOrder)
                {
                    var masterRow = focusedRow as StarShipITOrder;

                    // Remove child details
                    foreach (var detail in masterRow.StarShipITOrderDetails)
                        _context.StarShipITOrderDetails.Remove(detail);

                    // Delete the master row
                    _context.StarShipITOrders.Remove(masterRow);
                }
                // Check if it's a detail row
                else if (focusedRow is StarShipITOrderDetail)
                {
                    var detailRow = focusedRow as StarShipITOrderDetail;
                    _context.StarShipITOrderDetails.Remove(detailRow);
                }

                // Save changes to the database
                _context.SaveChanges();

                // Refresh the grid
                RefreshGridView(); // Assume RefreshGridView() is your method to refresh the grid
            }
        }
    }


    public void DeleteOrderAndOrderDetails(int orderId)
    {
        var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Delete the associated records from the OrderDetails table
            using (var command =
                   new SqlCommand("DELETE FROM StarShipITOrderDetails WHERE OrderId = @orderId", connection))
            {
                command.Parameters.AddWithValue("@orderId", orderId);

                command.ExecuteNonQuery();
            }

            // Delete the record from the Orders table
            using (var command = new SqlCommand("DELETE FROM StarShipITOrders WHERE Id = @orderId", connection))
            {
                command.Parameters.AddWithValue("@orderId", orderId);

                command.ExecuteNonQuery();
            }
        }
    }


    private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
    {
        if (e.Column.FieldName == "ExtraData")
        {
            var value = e.CellValue.ToString();
            switch (value)
            {
                case "CSC":
                    e.Appearance.BackColor = Color.LightBlue;
                    break; // Change as needed
                case "RUB":
                    e.Appearance.BackColor = Color.LightGray;
                    break;
                case "BSA":
                    e.Appearance.BackColor = Color.LightGreen;
                    break;
                case "DS":
                    e.Appearance.BackColor = Color.LightYellow;
                    break;
                // Add more cases if needed
            }
        }

        // List of column names to check for missing values
        var columnNames = new List<string>
        {
            "OrderNumber", "DestinationName", "DestinationStreet", "DestinationSuburb", "DestinationState",
            "DestinationPostCode", "DestinationCountry"
        };

        // Dictionary of allowed states and their postcode ranges
        var stateRanges = new Dictionary<string, Tuple<int, int>>(StringComparer.OrdinalIgnoreCase)
        {
            { "vic", new Tuple<int, int>(3000, 3999) },
            { "Victoria", new Tuple<int, int>(3000, 3999) },
            { "qld", new Tuple<int, int>(4000, 4999) },
            { "Queensland", new Tuple<int, int>(4000, 4999) },
            { "nsw", new Tuple<int, int>(2000, 2999) },
            { "New South Wales", new Tuple<int, int>(2000, 2999) },
            { "act", new Tuple<int, int>(0200, 0299) },
            { "Australian Capital Territory", new Tuple<int, int>(0200, 0299) },
            { "tas", new Tuple<int, int>(7000, 7999) },
            { "Tasmania", new Tuple<int, int>(7000, 7999) },
            { "sa", new Tuple<int, int>(5000, 5999) },
            { "South Australia", new Tuple<int, int>(5000, 5999) },
            { "wa", new Tuple<int, int>(6000, 6999) },
            { "Western Australia", new Tuple<int, int>(6000, 6999) },
            { "nt", new Tuple<int, int>(0800, 0899) },
            { "Northern Territory", new Tuple<int, int>(0800, 0899) }
        };


        // Check if the current column should be checked for missing values
        if (columnNames.Contains(e.Column.FieldName))
        {
            // Get the cell value
            var cellValue = e.CellValue;

            // Check if the cell value is missing (null or empty)
            if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
            {
                // Set the cell background color to highlight missing values
                e.Appearance.BackColor = Color.Yellow;

                // Set the cell text color
                e.Appearance.ForeColor = Color.Black;
            }

            // Check if the DestinationState column contains an allowed state
            else if (e.Column.FieldName == "DestinationState" && !stateRanges.ContainsKey(cellValue.ToString()))
            {
                // Set the cell background color to highlight invalid states
                e.Appearance.BackColor = Color.Pink;

                // Set the cell text color
                e.Appearance.ForeColor = Color.Black;
            }

            // Check if the DestinationPostCode column contains a 4-digit number
            else if (e.Column.FieldName == "DestinationPostCode" && !Regex.IsMatch(cellValue.ToString(), @"^\d{4}$"))
            {
                // Set the cell background color to highlight invalid postcodes
                e.Appearance.BackColor = Color.Pink;

                // Set the cell text color
                e.Appearance.ForeColor = Color.Black;
            }

            // Check if the DestinationState and DestinationPostCode match
            else if (e.Column.FieldName == "DestinationPostCode" && gridView1.GetRowCellValue(e.RowHandle,
                                                                     "DestinationState") is string state
                                                                 && stateRanges.TryGetValue(state, out var range)
                                                                 && int.TryParse(cellValue.ToString(),
                                                                     out var postcode) && (postcode < range.Item1 ||
                                                                     postcode > range.Item2))
            {
                // Set the cell background color to highlight mismatched postcodes
                e.Appearance.BackColor = Color.Orange;

                // Set the cell text color
                e.Appearance.ForeColor = Color.Black;
            }
        }
    }


    private readonly List<GridColumn> removedColumns = new();

    private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (gridView1.Columns.Count > 0)
        {
            // Store the state of removed columns.
            foreach (var columnName in new[]
                     {
                         "SignatureRequired", "Carrier", "ShippingMethod", "ReturnOrder", "DeliveryInstructions",
                         "DestinationPhone"
                     }) // replace with your column names
            {
                var columnToRemove = gridView1.Columns[columnName];
                if (columnToRemove != null)
                {
                    removedColumns.Add(columnToRemove);
                    gridView1.Columns.Remove(columnToRemove);
                }
            }
        }
        else
        {
            // Restore removed columns.
            foreach (var column in removedColumns) gridView1.Columns.Add(column);

            removedColumns.Clear();
        }
    }

    private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
    {
        // Confirm the user wants to delete all rows
        var result = XtraMessageBox.Show("Are you sure you want to delete all rows?", "Confirmation",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("[sp_delete_starshipit_order_tables]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }
                }

                // Refresh the grid view
                RefreshGridView();

                // Notify the user that the deletion was successful
                XtraMessageBox.Show("All rows were successfully deleted.", "Success", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                // Log the exception and notify the user of the error
                // This is a basic example, consider logging the exception details in a real application
                Console.WriteLine(ex.ToString());
                XtraMessageBox.Show("An error occurred while trying to delete all rows.", "Error",
                    MessageBoxButtons.OK);
            }
        }
    }


    public void RefreshGridView()
    {
        // Fetch updated data from the database using EF.
        var updatedOrders = _context.StarShipITOrders
            .Include(s => s.StarShipITOrderDetails)
            .Where(s => s.ShipmentID == null)
            .ToList();

        // Clear and update the data source for the grid control
        gridControl1.DataSource = null;
        gridControl1.DataSource = new BindingList<StarShipITOrder>(updatedOrders);

        // Refresh the main and detail views
        var mainView = gridControl1.MainView as GridView;
        mainView.RefreshData();

        // Loop through all rows of the master view
        for (var i = 0; i < mainView.RowCount; i++)
        {
            var rowHandle =
                mainView.GetRowHandle(i); // Get the row handle, which may be different from the visual index i
            // Get the detail view for the current master row; assume only one detail grid, so relationIndex is 0
            var detailView = mainView.GetDetailView(rowHandle, 0) as GridView;

            // Refresh the detail view
            if (detailView != null) detailView.RefreshData();
        }
    }


    private void barCheckItem1_CheckedChanged_1(object sender, ItemClickEventArgs e)
    {
        var item = (BarCheckItem)e.Item;
        gridView1.BeginUpdate(); // Suspend layout updates

        for (var i = 0; i < gridView1.RowCount; i++)
        {
            var row = gridView1.GetRow(i) as StarShipITOrder;
            if (row != null)
            {
                row.Selected = item.Checked;
                _context.StarShipITOrders.Update(row); // Update the entity
            }

            gridView1.SetRowCellValue(i, "Selected", item.Checked); // Update the grid cell
        }

        _context.SaveChanges(); // Commit changes to the database

        gridView1.EndUpdate(); // Resume layout updates
    }


    private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
    {
        SaveAllChanges();
    }
}