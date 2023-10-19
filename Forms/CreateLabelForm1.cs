using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Label = OrderManagerEF.Entities.Label;

namespace OrderManagerEF
{
    public partial class CreateLabelForm1 : RibbonForm
    {
        private readonly IConfiguration _configuration;
        private BindingSource bindingSource;
        private readonly SemaphoreSlim semaphore;
        private OMDbContext _context;



        public CreateLabelForm1(IConfiguration configuration, OMDbContext context)
        {
            _configuration = configuration;
            _context = context;
            InitializeComponent();
            BindTableAdapterToBindingSource();

            //gridView1.RowClick += gridView_RowClick;
            semaphore = new SemaphoreSlim(1, 1);
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            gridControl1.EmbeddedNavigator.ButtonClick += EmbeddedNavigator_ButtonClick;


            progressBarControl1 = new ProgressBarControl();
            progressBarControl1.Properties.Minimum = 0;
            progressBarControl1.Visible = false;

            Controls.Add(progressBarControl1);

            UpdateProgressBarLocation(); // Call this after the progress bar has been added to the form
            Resize += (sender, e) => UpdateProgressBarLocation();
            gridView1.RowCellStyle += gridView1_RowCellStyle;
            Load += CreateLabel1_Load;
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
            barButtonItem3.ItemClick += barButtonItem3_ItemClick;
        }


        private void UpdateProgressBarLocation()
        {
            progressBarControl1.Location = new Point(
                (ClientSize.Width - progressBarControl1.Width) / 2,
                (ClientSize.Height - progressBarControl1.Height) / 2
            );
        }

        private void YourForm_Resize(object sender, EventArgs e)
        {
            UpdateProgressBarLocation();
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "AddressValidatedValue")
            {
                // Get the value of the addressvalidated_value field for the current row
                bool addressValidated =
                    Convert.ToBoolean(gridView1.GetRowCellValue(e.RowHandle, "AddressValidatedValue"));

                // Set the cell background color based on the addressvalidated_value
                if (addressValidated)
                {
                    e.Appearance.BackColor = Color.Green; // Set the cell background color for True
                    e.Appearance.ForeColor = Color.White;
                }
                else
                {
                    e.Appearance.BackColor = Color.Red; // Set the cell background color for False
                    e.Appearance.ForeColor = Color.White;
                }
            }
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


        private void BindTableAdapterToBindingSource()
        {
            // Fetch labels using Entity Framework Core
            var labels = _context.Labels.ToList();

            // Bind the result to the grid control
            gridControl1.DataSource = labels;
        }



        private void CreateLabel1_Load(object sender, EventArgs e)
        {
            gridView1.CustomRowCellEdit += gridView1_CustomRowCellEdit;
            gridView1.CellValueChanged += gridView1_CellValueChanged;


        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "addressvalidated_value")
            {
                RepositoryItemComboBox comboBox = new RepositoryItemComboBox();
                comboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                comboBox.Items.AddRange(new object[] { true, false });
                e.RepositoryItem = comboBox;
            }
        }

        private async void gridView1_CellValueChanged(object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AddressValidated_Value")
            {
                gridView1.PostEditor();
                gridView1.UpdateCurrentRow();

                // Get the changed label entity from the grid view row
                Label changedLabel = gridView1.GetRow(e.RowHandle) as Label;

                if (changedLabel != null)
                {
                    // Mark the entity as modified
                    _context.Entry(changedLabel).State = EntityState.Modified;

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }

                gridView1.RefreshData();
            }
        }



        private async void EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            var gridView = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;

            if (e.Button.ButtonType == NavigatorButtonType.EndEdit)
            {
                gridView1.PostEditor(); // Post any pending changes
                gridView1.UpdateCurrentRow(); // Update the current row

                try
                {
                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    e.Handled = true; // Indicate that the event has been handled
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else if (e.Button.ButtonType == NavigatorButtonType.Remove)
            {
                // Get the selected row handles
                var selectedRowHandles = gridView.GetSelectedRows();

                // Sort the row handles in descending order to avoid index conflicts when deleting rows
                Array.Sort(selectedRowHandles, (x, y) => y.CompareTo(x));

                // Start a batch update to prevent excessive layout updates
                gridView.BeginUpdate();

                // Loop through the selected row handles and delete the corresponding rows
                foreach (var rowHandle in selectedRowHandles)
                {
                    if (rowHandle >= 0)
                    {
                        Label labelToRemove = gridView.GetRow(rowHandle) as Label;
                        if (labelToRemove != null)
                        {
                            _context.Labels.Remove(labelToRemove); // Remove the entity from the context
                            gridView.DeleteRow(rowHandle); // Remove the row from the grid view
                        }
                    }
                }

                // End the batch update
                gridView.EndUpdate();

                // Update the underlying data source after deleting rows from the grid view
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                // Prevent the default behavior of the "Delete" button by setting e.Handled to true
                e.Handled = true;
            }
        }


        private async void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }






        private async void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the selected rows
            int[] selectedRowHandles = gridView1.GetSelectedRows();

            try
            {
                // Loop through the selected rows, add them to the archive table, and remove them from the main table
                for (int i = selectedRowHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedRowHandles[i];
                    Label sourceLabel = gridView1.GetRow(rowHandle) as Label; // Assuming gridView1 is bound to a list of Labels

                    if (sourceLabel == null)
                        continue;

                    // Create a new LabelArchive object and copy properties from the source label
                    LabelsArchive archiveLabel = new LabelsArchive
                    {
                        OrderId = sourceLabel.OrderId,
                        OrderDate = sourceLabel.OrderDate,
                        Reference = sourceLabel.Reference,
                        OrderNumber = sourceLabel.OrderNumber,
                        AddressValidatedKey = sourceLabel.AddressValidatedKey,
                        AddressValidatedValue = sourceLabel.AddressValidatedValue,
                        Weight = sourceLabel.Weight,
                        Width = sourceLabel.Width,
                        Height = sourceLabel.Height,
                        Length = sourceLabel.Length,
                        Reprint = sourceLabel.Reprint,
                        Location = sourceLabel.Location,
                        Selected = sourceLabel.Selected,

                    };

                    // Add to LabelsArchive and remove from Labels
                    _context.LabelArchives.Add(archiveLabel);
                    _context.Labels.Remove(sourceLabel);
                }

                // Commit changes
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., show an error message)
                XtraMessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Refresh the Labels data
            BindTableAdapterToBindingSource();  // This is the method you previously created for binding data
        }


        public async Task<bool> DeleteShipmentId(int orderId, string location)
        {
            // Get the connection string from configuration
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            // Create ApiKeyManager instance with the retrieved connection string
            var apiKeyManager = new ApiKeyManager(connectionString);


            // Get API keys for the location
            var apiKeys = apiKeyManager.GetApiKeysByLocation(location);

            string apiUrl = $"https://api.starshipit.com/api/orders/delete?order_id={orderId}";

            // Prepare HttpClient
            using (var client = new HttpClient())
            {
                // Setup headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", apiKeys.StarshipItApiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKeys.OcpApimSubscriptionKey);

                // Send a DELETE request
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    XtraMessageBox.Show($"Order with ID {orderId} was deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    // Read the response body
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Decode the response body from JSON
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBody);

                    // Show an error message if the request was not successful
                    XtraMessageBox.Show($"Failed to delete the order with ID {orderId}. Server response: {errorResponse.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }
        }



        private async void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Check if a row is selected
            if (gridView1.GetSelectedRows().Length == 0)
            {
                XtraMessageBox.Show("Please select a row to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Assuming the GridView is called gridView1 and it's connected to your database.
                // Get the connection string
                string connectionString = _configuration.GetConnectionString("RubiesConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Begin a transaction to ensure data consistency
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Get the selected row in the GridView
                            int selectedRowHandle = gridView1.GetSelectedRows()[0];

                            // Get the order id and location for the row
                            int orderId = Convert.ToInt32(gridView1.GetRowCellValue(selectedRowHandle, "OrderId"));
                            string location = gridView1.GetRowCellValue(selectedRowHandle, "Location").ToString();

                            // Use the order id and location when deleting the record from the API
                            bool isDeletedFromAPI = await DeleteShipmentId(orderId, location);

                            if (!isDeletedFromAPI)
                            {
                                // If deletion from API failed, don't delete from the database
                                return;
                            }

                            // If deletion from API succeeded, update the transheaders table
                            string updateQuery = "UPDATE transheaders SET ZShipmentID = '' WHERE ZShipmentID = @order_id AND TransactionType ='co' AND outstanding = 1";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction)) // Update transheaders
                            {
                                updateCommand.Parameters.AddWithValue("@order_id", orderId);
                                await updateCommand.ExecuteNonQueryAsync();
                            }

                            // If deletion from API succeeded, delete from the database
                            string deleteQuery = "DELETE FROM Labels WHERE OrderId = @order_id";
                            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction)) // Delete from Labels
                            {
                                deleteCommand.Parameters.AddWithValue("@order_id", orderId);
                                await deleteCommand.ExecuteNonQueryAsync();
                            }

                            // Commit the transaction
                            transaction.Commit();

                            // Remove the row from the GridView
                            gridView1.DeleteRow(selectedRowHandle);

                            // Refresh the GridView after deletion
                            gridView1.RefreshData();
                        }
                        catch
                        {
                            // An error occurred, roll back the transaction
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                XtraMessageBox.Show("Please select a row to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void barButtonItem1_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            if (semaphore.CurrentCount == 0) return;

            await semaphore.WaitAsync();

            try
            {
                var checkedRowHandles = new Dictionary<string, List<int>>();

                for (var i = 0; i < gridView1.RowCount; i++)
                {
                    var isChecked = Convert.ToBoolean(gridView1.GetRowCellValue(i, "Selected"));

                    if (isChecked)
                    {
                        // Check if the addressvalidated_value field is true for the current row
                        bool addressValidated =
                            Convert.ToBoolean(gridView1.GetRowCellValue(i, "AddressValidatedValue"));

                        if (addressValidated)
                        {
                            var extraData = gridView1.GetRowCellValue(i, "Location").ToString();

                            if (!checkedRowHandles.ContainsKey(extraData))
                                checkedRowHandles[extraData] = new List<int>();

                            checkedRowHandles[extraData].Add(i);
                        }
                        else
                        {
                            // Optionally, show a message to the user informing them that the submission is not allowed
                            XtraMessageBox.Show(
                                "Submission not allowed. The address has not been validated. Please unselect the row",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                // Ensure the splash screen is closed
                SplashScreenUtility.CloseSplashScreenIfNeeded();

                // Show the custom splash screen
                SplashScreenManager.ShowForm(typeof(ProgressForm));

                // Create a Progress<int> instance that will update the splash screen progress
                Progress<int> progress = new Progress<int>(value =>
                {
                    if (SplashScreenManager.Default != null)
                    {
                        SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetProgress, value);
                    }
                });

                foreach (var extraData in checkedRowHandles.Keys)
                {
                    // Retrieve API keys for the desired location.
                    var (starshipItApiKey, ocpApimSubscriptionKey) = GetApiKeysFromTableAdapter(extraData);

                    var apiRequestSender = new BulkLabelCreator(_configuration, gridView1, starshipItApiKey, ocpApimSubscriptionKey,
                        checkedRowHandles[extraData].ToArray(), progressBarControl1, this);
                    await apiRequestSender.BulkLabelSendApiRequestAsyncLimitProgress(progress);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                semaphore.Release();

                // Close the splash screen
                if (SplashScreenManager.Default != null)
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }
    }
}