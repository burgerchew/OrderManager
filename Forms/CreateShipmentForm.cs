using DevExpress.Data;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
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
using OrderManagerEF.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Label = OrderManagerEF.Entities.Label;

namespace OrderManagerEF
{
    public partial class CreateShipmentForm : RibbonForm
    {
        // Declare your DataSet and TableAdapters.
        private IConfiguration _configuration;
        private OMDbContext _context;


        public CreateShipmentForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();

            _configuration = configuration;
            _context = context;
            //InitializeData();
            //AddRadioCheckBoxColumn();
            Load += CreateShipmentForm_Load;
   

            BarButtonClick();
        }


        private void BarButtonClick()
        {
            //Toggle Shipments
            barCheckItem1.CheckedChanged += barCheckItem1_CheckedChanged;
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

            ApplyShipmentIdFilter();
            PopulateExtraDataLookup(gridControl1.MainView as GridView);
        }

        private void ConfigureMasterDetailGrid()
        {
            // Load data with details included
            var orders =  _context.StarShipITOrders.Include(s => s.StarShipITOrderDetails).ToList();

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
                StarShipITOrder order = view.GetRow(e.RowHandle) as StarShipITOrder;
                e.ChildList = order.StarShipITOrderDetails.ToList(); // Cast to list
            };

            masterView.MasterRowGetRelationName += (sender, e) => { e.RelationName = "StarShipITOrderDetails"; };

            masterView.MasterRowGetRelationCount += (sender, e) => { e.RelationCount = 1; };

            // Create and configure detail GridView
            GridView detailView = new GridView(gridControl1);
            gridControl1.LevelTree.Nodes.Add("StarShipITOrderDetails", detailView);
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
            if (e.Row is StarShipITOrder header)
            {
                _context.StarShipITOrders.Update(header);
            }
            else if (e.Row is StarShipITOrderDetail detail)
            {
                _context.StarShipITOrderDetails.Update(detail);
            }

            // Save changes to database
            _context.SaveChanges();
        }

        private void InitializeData()
        {
            try
            {
                // Fetch data using EF and DbContext
                var shipments = _context.StarShipITOrders.Include(s => s.StarShipITOrderDetails).ToList();


                // Set up the main view's data source with the fetched shipments
                var masterBindingSource = new BindingSource { DataSource = shipments };

                gridControl1.DataSource = masterBindingSource;

                // Set up the master-detail relationship
                var detailView = new GridView(gridControl1);
                gridControl1.LevelTree.Nodes.Add("StarShipITOrderDetails", detailView);
                detailView.PopulateColumns();
                detailView.ViewCaption = "Shipment Details";

                //// Attach event handlers

                detailView.MasterRowExpanded += gridView1_MasterRowExpanded;
                detailView.RowUpdated += gridView1_RowUpdated;
                detailView.RowDeleted += gridView1_RowDeleted;
                detailView.InitNewRow += gridView1_InitNewRow;
                detailView.RowDeleting += gridView1_RowDeleting;
                gridControl1.EmbeddedNavigator.ButtonClick += EmbeddedNavigator_ButtonClick;
                detailView.CustomDrawCell += gridView_CustomDrawCell;

                SubscribeToDetailGridViewInitNewRow();
                SubscribeToDetailGridViewCellValueChanged();
                SubscribeToDetailGridViewRowDeleted();

                // Assuming the methods ApplyShipmentIdFilter and PopulateExtraDataLookup are still relevant,
                // let's call them as they are. If they need to be refactored too, please provide their content.
                ApplyShipmentIdFilter();
                PopulateExtraDataLookup(gridControl1.MainView as GridView);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Failed to initialize data. Error: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            {
                if (!extraDataValues.Contains(defaultValue))
                    extraDataValues.Add(defaultValue);
            }

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

            RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
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



        private void SubscribeToDetailGridViewInitNewRow()
        {
            // Get the detail GridView pattern view from the LevelTree
            var detailViewPattern = gridControl1.LevelTree.Nodes[0].LevelTemplate as GridView;

            if (detailViewPattern != null)
                // Subscribe to the InitNewRow event of the detail GridView pattern view
                detailViewPattern.InitNewRow += detailViewPattern_InitNewRow;
        }

        private void OrderTableAdapter_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
        {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert)
            {
                var newOrderID = (int)e.Row["Id", DataRowVersion.Current];
                AddDefaultOrderDetail(newOrderID);
            }
        }

        private void DeleteOrderDetails(int orderId)
        {
            // Fetch related order details directly from the database
            var relatedOrderDetails = _context.StarShipITOrderDetails
                .Where(sd => sd.OrderId == orderId)
                .ToList();

            // Remove the related order details from the context
            _context.StarShipITOrderDetails.RemoveRange(relatedOrderDetails);

            // Commit changes to the database
            _context.SaveChanges();
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


        private void detailView_RowDeleted(object sender, RowDeletedEventArgs e)
        {
            SaveChanges();
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

        private void detailViewPattern_RowDeleted(object sender, RowDeletedEventArgs e)
        {
            // Save changes to the database
            SaveChanges();
        }

        private void SubscribeToDetailGridViewCellValueChanged()
        {
            // Get the detail GridView pattern view from the LevelTree
            var detailViewPattern = gridControl1.LevelTree.Nodes[0].LevelTemplate as GridView;

            if (detailViewPattern != null)
                // Subscribe to the CellValueChanged event of the detail GridView pattern view
                detailViewPattern.CellValueChanged += detailViewPattern_CellValueChanged;
        }

        private void detailViewPattern_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var view = sender as GridView;

            if (view != null)
            {
                view.SetRowCellValue(e.RowHandle, view.Columns["Description"], "Costume");
                view.SetRowCellValue(e.RowHandle, view.Columns["SKU"], "COSTUME");
                view.SetRowCellValue(e.RowHandle, view.Columns["Quantity"], 1);
                view.SetRowCellValue(e.RowHandle, view.Columns["Weight"], 0.65);
                view.SetRowCellValue(e.RowHandle, view.Columns["Value"], 40);
                // Set other default values as required.
            }
        }

        private void AddDefaultOrderDetail(int orderID)
        {
            var newDetail = new StarShipITOrderDetail
            {
                OrderId = orderID,
                Description = "Mobile Phone",
                SKU = "MOBILE",
                Quantity = 1,
                Weight = 0.23M, // Assuming weight is of type decimal and 023 was a typo
                Value = 125M
            };

            _context.StarShipITOrderDetails.Add(newDetail);
            _context.SaveChanges();
        }


        private void detailViewPattern_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var view = sender as GridView;

            if (view != null)
            {
                view.PostEditor();  // Save the edited cell value
                view.UpdateCurrentRow();  // Update the underlying data source

                // Assuming you have mapped OrderDetails as ShipmentDetails
                var detailRow = view.GetRow(e.RowHandle) as StarShipITOrderDetail;

                if (detailRow != null)
                {
                    // Attach the entity and mark it as modified
                    _context.Entry(detailRow).State = EntityState.Modified;

                    // Save changes to the database
                    _context.SaveChanges();
                }
            }
        }


        private void gridView1_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            var detailView = gridView1.GetDetailView(e.RowHandle, 0) as GridView;
            if (detailView != null)
            {
                detailView.InitNewRow += (s, ea) =>
                {
                    var orderID = (int)gridView1.GetRowCellValue(e.RowHandle, "Id");
                    detailView.SetRowCellValue(ea.RowHandle, "OrderID", orderID);
                };

                detailView.RowUpdated += (s, ea) => SaveChanges();
                detailView.RowDeleted += (s, ea) => SaveChanges();
            }
        }


        private void SubscribeToDetailGridViewRowDeleted()
        {
            // Get the detail GridView pattern view from the LevelTree
            var detailViewPattern = gridControl1.LevelTree.Nodes[0].LevelTemplate as GridView;

            if (detailViewPattern != null)
                // Subscribe to the RowDeleted event of the detail GridView pattern view
                detailViewPattern.RowDeleted += detailViewPattern_RowDeleted;
        }

        private void gridView1_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newOrderID = GenerateNewOrderID();
            gridView1.SetRowCellValue(e.RowHandle, "Id", newOrderID);
            gridView1.SetRowCellValue(e.RowHandle, "OrderDate", DateTime.Now);
            gridView1.SetRowCellValue(e.RowHandle, "Selected", true); // Set the Selected column value to true

            var detailView = gridView1.GetDetailView(e.RowHandle, 0) as GridView;
            if (detailView != null)
                detailView.InitNewRow += (s, ea) =>
                {
                    detailView.SetRowCellValue(ea.RowHandle, "OrderID", newOrderID);
                };
        }

        private int GenerateNewOrderID()
        {
            // Using Entity Framework to get the maximum OrderID value
            var maxOrderID = _context.StarShipITOrders.Any() ? _context.StarShipITOrders.Max(o => o.Id) : 0;

            // Increment the maximum OrderID value by 1 or set a default starting value if the table is empty
            var newOrderID = maxOrderID > 0 ? maxOrderID + 1 : 1;

            return newOrderID;
        }

        private void gridView1_RowDeleting(object sender, RowDeletingEventArgs e)
        {
            var rowView = e.Row as DataRowView;
            var orderId = (int)rowView["Id"];

            // Using Entity Framework to retrieve the related entity
            var orderToDelete = _context.StarShipITOrders.Include(s => s.StarShipITOrderDetails).FirstOrDefault(o => o.Id == orderId);

            if (orderToDelete != null)
            {
                // Delete the related order details
                _context.StarShipITOrderDetails.RemoveRange(orderToDelete.StarShipITOrderDetails);

                // Delete the order
                _context.StarShipITOrders.Remove(orderToDelete);

                // Save the changes
                _context.SaveChanges();
            }
        }


        private void EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button.ButtonType == NavigatorButtonType.Remove)
            {
                e.Handled = true;

                // Check if a row is selected in the master GridView
                if (gridView1.FocusedRowHandle < 0)
                {
                    XtraMessageBox.Show("Please select an order to delete.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Ask for confirmation before deleting the selected order
                var result = XtraMessageBox.Show("Are you sure you want to delete the selected order?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) gridView1.DeleteSelectedRows();
            }
        }


        private void AddRadioCheckBoxColumn()
        {
            // Create a new GridColumn for the "Selected" field
            var selectedColumn = new GridColumn
            {
                FieldName = "Selected",
                Caption = "Selected",
                Visible = true,
                VisibleIndex = gridView1.Columns.Count // Set the visible index to the last column
            };

            // Create a RepositoryItemCheckEdit with the RadioGroupIndex property set to 0
            var checkEdit = new RepositoryItemCheckEdit
            {
                RadioGroupIndex = 0
            };

            // Set the ColumnEdit property of the new column to the RepositoryItemCheckEdit
            selectedColumn.ColumnEdit = checkEdit;

            // Add the new column to the gridView1.Columns collection
            gridView1.Columns.Add(selectedColumn);
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

        public int InsertLabelAndMetadata(int orderId, string orderDate, string reference, string orderNumber,
            string metafieldKey, string value, decimal weight, decimal width, decimal height, decimal length, bool selected, string extraData)
        {
            // Create a new label entity
            var newLabel = new Label
            {
                OrderId = orderId,
                OrderDate = DateTime.ParseExact(orderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                Reference = reference,
                OrderNumber = orderNumber,
                AddressValidatedKey = metafieldKey,
                AddressValidatedValue = value,
                Weight = weight,
                Width = width,
                Height = height,
                Length = length,
                Location =extraData,
                Selected = selected,
                ExtraData = extraData
            };

            // Add the new label to the DbContext
            _context.Labels.Add(newLabel);

            // Save the changes to the database and return the number of affected rows
            return _context.SaveChanges();
        }


        public void UpdateTransHeadersWithShipmentId(int orderId, string orderNumber)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("UPDATE TransHeaders SET ZShipmentID = @orderId WHERE TradingRef = @orderNumber and outstanding =1 and transactiontype='CO'", connection))
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

                using (var command = new SqlCommand("UPDATE StarShipITOrders SET ShipmentID = @orderId WHERE OrderNumber = @orderNumber and reference = @reference", connection))
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
                                           .Where(s => (bool)s.Selected)
                                           .ToList();

            if (!selectedShipments.Any())
            {
                XtraMessageBox.Show("Please select at least one row before proceeding.", "No Rows Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var shipmentResponses = await ProcessShipments(selectedShipments);

            int successfulShipmentsCount = ProcessResponses(shipmentResponses);

            // Inform the user about the successful shipments
            XtraMessageBox.Show($"Total {successfulShipmentsCount} shipments were successful. Please check the label tab in a moment.", "Shipments Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshGridView();
        }


        private async Task<List<ShipmentResponse>> ProcessShipments(List<StarShipITOrder> orders)
        {
            var shipmentResponses = new List<ShipmentResponse>();
            var ordersGroupedByLocation = orders.GroupBy(o => o.ExtraData);
            var mapper = new StarShipOrderMapper();

            SplashScreenManager.ShowForm(typeof(ProgressForm));
            int totalRows = orders.Count;

            Progress<int> progress = new Progress<int>(value =>
            {
                if (SplashScreenManager.Default != null)
                {
                    SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetProgress, value);
                }
            });

            foreach (var ordersGroup in ordersGroupedByLocation)
            {
                var extraData = ordersGroup.Key;
                var (starshipItApiKey, ocpApimSubscriptionKey) = GetApiKeysFromTableAdapter(extraData);
                var shipmentManager = new ShipmentManager(starshipItApiKey, ocpApimSubscriptionKey);

                var mappedOrders = ordersGroup.Select(mapper.MapToOrder).ToList();

                // Here's the change: For each StarShipITOrder, map its details to a list of OrderDetail
                var mappedOrderDetailsGrouped = ordersGroup.Select(o => mapper.MapToOrderDetails(o.StarShipITOrderDetails.ToList()).ToList()).ToList();

                var singleGroupResponses = await shipmentManager.CreateShipments(mappedOrders, mappedOrderDetailsGrouped, progress, totalRows);

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
            int successfulShipmentsCount = 0;

            foreach (var response in responses)
            {
                if (response.Success)
                {
                    // Note: Here I'm assuming that the Order object in your ShipmentResponse closely matches your Shipment EF entity
                    // This will need adjusting based on your actual model and what the CreateShipments() method returns.
                    var orderResponse = response.Order;
                    if (orderResponse != null)
                    {
                        successfulShipmentsCount++;

                        var addressValidatedValue = orderResponse.Metadatas?.FirstOrDefault(m => m.metafield_key == "ADDRESSVALIDATED")?.Value;

                        if (orderResponse.Packages != null)
                        {
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
                                    ExtraData = response.ExtraData

                                };
                                _context.Labels.Add(newLabel);

                                UpdateTransHeadersWithShipmentId(orderResponse.order_id, orderResponse.order_number);
                                UpdateOrdersWithShipmentId(orderResponse.order_id, orderResponse.order_number, orderResponse.Reference);

                                if (addressValidatedValue == "true")
                                {
                                    DeleteOrderAndOrderDetails(orderResponse.order_id);
                                }
                            }
                        }

                        _context.SaveChanges();
                    }
                }
                else
                {
                    XtraMessageBox.Show($"Shipment for order {response.Order?.order_number ?? "unknown"} was unsuccessful. Please check the order exists and try again.", "Shipment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return successfulShipmentsCount;
        }





        private void gridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "ExtraData")
            {
                var value = e.CellValue.ToString();
                switch (value)
                {
                    case "CSC": e.Appearance.BackColor = Color.LightBlue; break; // Change as needed
                    case "RUB": e.Appearance.BackColor = Color.LightGray; break;
                    case "BSA": e.Appearance.BackColor = Color.LightGreen; break;
                    case "DS": e.Appearance.BackColor = Color.LightYellow; break;
                        // Add more cases if needed
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
                using (var command = new SqlCommand("DELETE FROM OrderDetails WHERE OrderId = @orderId", connection))
                {
                    command.Parameters.AddWithValue("@orderId", orderId);

                    command.ExecuteNonQuery();
                }

                // Delete the record from the Orders table
                using (var command = new SqlCommand("DELETE FROM Orders WHERE Id = @orderId", connection))
                {
                    command.Parameters.AddWithValue("@orderId", orderId);

                    command.ExecuteNonQuery();
                }
            }
        }


        private void barCheckItem1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            BarCheckItem item = (BarCheckItem)e.Item;
            gridView1.BeginUpdate(); // Suspend layout updates

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "Selected", item.Checked);
            }

            gridView1.EndUpdate(); // Resume layout updates
        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            // List of column names to check for missing values
            List<string> columnNames = new List<string> { "OrderNumber", "DestinationName", "DestinationStreet", "DestinationSuburb", "DestinationState", "DestinationPostCode", "DestinationCountry" };

            // Dictionary of allowed states and their postcode ranges
            Dictionary<string, Tuple<int, int>> stateRanges = new Dictionary<string, Tuple<int, int>>(StringComparer.OrdinalIgnoreCase)
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
                { "Northern Territory", new Tuple<int, int>(0800, 0899) },
            };


            // Check if the current column should be checked for missing values
            if (columnNames.Contains(e.Column.FieldName))
            {
                // Get the cell value
                object cellValue = e.CellValue;

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
                else if (e.Column.FieldName == "DestinationPostCode" && gridView1.GetRowCellValue(e.RowHandle, "DestinationState") is string state
                         && stateRanges.TryGetValue(state, out var range)
                         && (int.TryParse(cellValue.ToString(), out var postcode) && (postcode < range.Item1 || postcode > range.Item2)))
                {
                    // Set the cell background color to highlight mismatched postcodes
                    e.Appearance.BackColor = Color.Orange;

                    // Set the cell text color
                    e.Appearance.ForeColor = Color.Black;
                }
            }
        }



        private List<GridColumn> removedColumns = new List<GridColumn>();

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gridView1.Columns.Count > 0)
            {
                // Store the state of removed columns.
                foreach (var columnName in new string[] { "SignatureRequired", "Carrier", "ShippingMethod", "ReturnOrder", "DeliveryInstructions", "DestinationPhone" }) // replace with your column names
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
                foreach (var column in removedColumns)
                {
                    gridView1.Columns.Add(column);
                }

                removedColumns.Clear();
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Confirm the user wants to delete all rows
            DialogResult result = XtraMessageBox.Show("Are you sure you want to delete all rows?", "Confirmation", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                string connectionString = _configuration.GetConnectionString("RubiesConnectionString");
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand("[sp_delete_starshipit_order_tables]", connection))
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
                    XtraMessageBox.Show("An error occurred while trying to delete all rows.", "Error", MessageBoxButtons.OK);
                }
            }
        }



        public void RefreshGridView()
        {
            // Fetch updated data from the database using EF.
            var updatedOrders = _context.StarShipITOrders.Include(s => s.StarShipITOrderDetails).ToList();

            // Bind the fetched data to the gridControl.
            gridControl1.DataSource = new BindingList<StarShipITOrder>(updatedOrders);

            // Refresh the view
            gridView1.RefreshData();
        }


    }
}