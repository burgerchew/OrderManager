using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DevExpress.CodeParser;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Windows.Forms;
using DevExpress.Utils.TouchHelpers;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OrderManager.Classes
{


    public class FileExistenceGridView : GridView
    {
        // Column name containing the file location
        public List<string> FileLocationColumnNames { get; set; } = new List<string>();
        private IConfiguration _configuration;


        // Colors for file existence highlighting
        public Color FileExistsBackColor { get; set; } = Color.LightGreen;
        public Color FileNotExistsBackColor { get; set; } = Color.LightCoral;

        // Custom text for file existence indication
        public string FileExistsText { get; set; } = "File exists";
        public string FileNotExistsText { get; set; } = "File does not exist";

        public bool FilterFileExists { get; set; } = false;


        public FileExistenceGridView(IConfiguration configuration) : base()
        {
            CustomDrawCell += FileExistenceGridView_CustomDrawCell;
            CustomRowCellEdit += FileExistenceGridView_CustomRowCellEdit;
            CustomRowFilter += FileExistenceGridView_CustomRowFilter;
            CustomRowFilter += ZShipmentID_CustomRowFilter;
            _configuration = configuration; // Assign the injected configuration to the private field



            this.OptionsSelection.MultiSelect = true;
            this.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            DoubleClick += FileExistenceGridView_DoubleClick;
            RowCellStyle += FileExistenceGridView_RowCellStyle;
            CustomColumnDisplayText += FileExistenceGridView_CustomColumnDisplayText;

        }

   
        private void FileExistenceGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {




            if (FileLocationColumnNames.Contains(e.Column.FieldName))
            {
                string filePath = e.CellValue?.ToString();
                bool fileExists = !string.IsNullOrEmpty(filePath) && File.Exists(filePath);

                // Change text to "Complete" for ArchiveFile column when the file exists
                if (fileExists && e.Column.FieldName == "ArchiveFile")
                {
                    e.Appearance.BackColor = FileExistsBackColor;
                    e.Appearance.ForeColor = Color.Black;
                    e.DisplayText = "Complete";
                }

                else if (fileExists)
                {
                    e.Appearance.BackColor = FileExistsBackColor;
                    e.Appearance.ForeColor = Color.Black;
                    e.DisplayText = FileExistsText;
                }
                else
                {
                    e.Appearance.BackColor = FileNotExistsBackColor;
                    e.Appearance.ForeColor = Color.Black;
                    e.DisplayText = FileNotExistsText;
                }

                // Check if the current column is 'LabelFile'
                if (e.Column.FieldName == "LabelFile")
                {
                    string archiveFilePath = GetRowCellValue(e.RowHandle, Columns["ArchiveFile"])?.ToString();
                    if (!string.IsNullOrEmpty(archiveFilePath) && File.Exists(archiveFilePath))
                    {
                        e.DisplayText = "File processed";
                        e.Appearance.BackColor = FileExistsBackColor;
                    }
                }

                // Check if the current column is 'PickSlipFile'
                if (e.Column.FieldName == "PickSlipFile")
                {
                    string pickslipFilePath = GetRowCellValue(e.RowHandle, Columns["PickSlipFile"])?.ToString();
                    if (!string.IsNullOrEmpty(pickslipFilePath) && File.Exists(pickslipFilePath))
                    {
                        e.DisplayText = "Pick Slip Ready";
                        e.Appearance.BackColor = FileExistsBackColor;
                    }
                }

                // Check if the current column is 'ArchiveFile'
                if (e.Column.FieldName == "ArchiveFile")
                {
                    string labelFilePath = GetRowCellValue(e.RowHandle, Columns["LabelFile"])?.ToString();
                    if (!string.IsNullOrEmpty(labelFilePath) && File.Exists(labelFilePath))
                    {
                        e.DisplayText = "Pending";
                        e.Appearance.BackColor = Color.Orange;
                    }
                }

                // Add the custom drawing for the TickCrossColumn here
                if (e.Column.FieldName == "TickCrossColumn")
                {
                    // Set the cell appearance
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);

                    // Set the cell text to the tick or cross
                    e.DisplayText = (e.CellValue?.ToString() == "✓") ? "✓" : "✗";

                    // Draw the cell
                    e.Cache.DrawString(e.DisplayText, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), e.Bounds, e.Appearance.GetStringFormat());
                    e.Handled = true;
                }
            }
        }


        private void FileExistenceGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (FileLocationColumnNames.Contains(e.Column.FieldName))
            {
                string filePath = GetRowCellValue(e.RowHandle, e.Column)?.ToString();
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    RepositoryItemHyperLinkEdit hyperlinkEdit = new RepositoryItemHyperLinkEdit
                    {
                        Caption = FileExistsText,
                        LinkColor = Color.Blue, // Set the link color
                        SingleClick = true, // Set the link to open on a single click
                    };
                    hyperlinkEdit.OpenLink += HyperlinkEdit_OpenLink;
                    e.RepositoryItem = hyperlinkEdit;
                }
                else
                {
                    RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
                    e.RepositoryItem = textEdit;
                }
            }
        }




        public void ToggleFileExistenceFilter()
        {
            // Toggle the filter state
            FilterFileExists = !FilterFileExists;

            // Refresh the view
            RefreshData();
        }


        private void HyperlinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
        {
            e.Handled = true; // Mark the event as handled to prevent the default behavior

            string filePath = e.EditValue?.ToString();
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    // Open the file
                    System.Diagnostics.Process process = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true,
                     
                        }
                    };
                    process.Start();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Error printing file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FileExistenceGridView_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            if (FilterFileExists)
            {
                bool anyFileExists = false;
                foreach (string columnName in FileLocationColumnNames)
                {
                    string filePath = GetRowCellValue(e.ListSourceRow, Columns[columnName])?.ToString();
                    bool fileExists = !string.IsNullOrEmpty(filePath) && File.Exists(filePath);

                    if (fileExists)
                    {
                        e.Visible = true;
                        e.Handled = true;
                        break;
                    }
                    else
                    {
                        e.Visible = anyFileExists;
                        e.Handled = true;
                    }
                }
            }


        }

        private bool _filterZShipmentID = false;

        public bool FilterZShipmentID
        {
            get { return _filterZShipmentID; }
            set
            {
                if (_filterZShipmentID != value)
                {
                    _filterZShipmentID = value;
                    RefreshData();
                }
            }
        }

        public void ToggleZShipmentIDFilter()
        {
            // Toggle the filter state
            FilterZShipmentID = !FilterZShipmentID;

            // Refresh the view
            RefreshData();
        }

        private void ZShipmentID_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            if (FilterZShipmentID)
            {
                // Assuming ZShipmentID is in the first cell, change 0 to the correct cell index if needed
                int zShipmentIDColumnIndex = 0;
                string zShipmentID = GetRowCellValue(e.ListSourceRow, Columns[zShipmentIDColumnIndex])?.ToString();
                bool hasZShipmentID = !string.IsNullOrEmpty(zShipmentID);

                e.Visible = hasZShipmentID;
                e.Handled = true;
            }
        }

        private void FileExistenceGridView_DoubleClick(object sender, EventArgs e)
        {
            // Get the GridView instance from the sender object
            var gridView = sender as GridView;

            if (gridView != null)
            {
                // Get the handle of the clicked row
                int rowHandle = gridView.FocusedRowHandle;

                if (rowHandle >= 0)
                {
                    // Get the value of the AccountingRef column
                    object accountingRef = gridView.GetRowCellValue(rowHandle, "AccountingRef");

                    // Get the value of the TotalWeight column
                    decimal totalWeight = Convert.ToDecimal(gridView.GetRowCellValue(rowHandle, "TotalWeight"));

                    if (accountingRef != null)
                    {

                        if (totalWeight > 5)
                        {
                            XtraMessageBox.Show("Total weight cannot be more than 5. Please correct the values and try again.", "Weight Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }


                        // Show a confirmation message
                        DialogResult result = XtraMessageBox.Show("Are you sure you want to create a pending shipment for " + accountingRef.ToString() + "?", "Confirm Execution", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Execute the stored procedure with the AccountingRef value
                            CreatePendingShipment(accountingRef);

                            // Show a success message
                            XtraMessageBox.Show("Successfully created Pending Shipment for " + accountingRef.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }


        // This method has been modified to accept an IConfiguration parameter.
        // The IConfiguration instance is used to provide the necessary configuration data
        // for the current environment.
        // This method accepts the accountingRef object and utilizes the _configuration instance
        // from the class to fetch the connection string and perform necessary database operations.
        private void CreatePendingShipment(object accountingRef)
        {
            // Get the connection string from the _configuration object using the specified key
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            string storedProcedureName = "dbo.CopyTransHeadersToOrdersNoCursor";

            // Use the connection string to create a new SqlConnection
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the AccountingRef parameter to the SqlCommand object
                    command.Parameters.AddWithValue("@AccountingRef", accountingRef);
                    // Add additional parameters if needed

                    // Open the connection and execute the stored procedure
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // Event handler to change display text
        private void FileExistenceGridView_CustomColumnDisplayText(object sender,CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "zEmployeeGroup" && e.Value?.ToString() == "NONE")
            {
                e.DisplayText = "Pending";
            }
        }


        private void FileExistenceGridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView; // Cast the sender to the GridView

            if (view != null && e.Column.FieldName == "zEmployeeGroup")
            {
                object value = view.GetRowCellValue(e.RowHandle, "zEmployeeGroup"); // Use the view instance to call GetRowCellValue
                if (value?.ToString() == "NONE")
                {
                    e.Appearance.BackColor = Color.Orange; // Set background color to orange
                }
            }
        }


    }

}
