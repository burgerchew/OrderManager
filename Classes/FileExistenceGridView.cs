using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Microsoft.Extensions.Configuration;
using DevExpress.Utils;

namespace OrderManagerEF.Classes;

public class FileExistenceGridView : GridView
{
    // Column name containing the file location
    public List<string> FileLocationColumnNames { get; set; } = new();
    private readonly IConfiguration _configuration;


    // Colors for file existence highlighting
    public Color FileExistsBackColor { get; set; } = Color.LightGreen;
    public Color FileNotExistsBackColor { get; set; } = Color.LightCoral;

    // Custom text for file existence indication
    public string FileExistsText { get; set; } = "File exists";
    public string FileNotExistsText { get; set; } = "File does not exist";

    public bool FilterFileExists { get; set; }


    public FileExistenceGridView(IConfiguration configuration)
    {
        CustomDrawCell += FileExistenceGridView_CustomDrawCell;
        CustomRowCellEdit += FileExistenceGridView_CustomRowCellEdit;
        CustomRowFilter += FileExistenceGridView_CustomRowFilter;
        CustomRowFilter += ZShipmentID_CustomRowFilter;
        _configuration = configuration; // Assign the injected configuration to the private field


        OptionsSelection.MultiSelect = true;
        OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
        DoubleClick += FileExistenceGridView_DoubleClick;
        RowCellStyle += FileExistenceGridView_RowCellStyle;
        CustomColumnDisplayText += FileExistenceGridView_CustomColumnDisplayText;
    }


    private void FileExistenceGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
    {
        if (FileLocationColumnNames.Contains(e.Column.FieldName))
        {
            var filePath = e.CellValue?.ToString();
            var fileExists = !string.IsNullOrEmpty(filePath) && File.Exists(filePath);

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
                var archiveFilePath = GetRowCellValue(e.RowHandle, Columns["ArchiveFile"])?.ToString();
                if (!string.IsNullOrEmpty(archiveFilePath) && File.Exists(archiveFilePath))
                {
                    e.DisplayText = "File processed";
                    e.Appearance.BackColor = FileExistsBackColor;
                }
            }

            // Check if the current column is 'PickSlipFile'
            if (e.Column.FieldName == "PickSlipFile")
            {
                var pickslipFilePath = GetRowCellValue(e.RowHandle, Columns["PickSlipFile"])?.ToString();
                if (!string.IsNullOrEmpty(pickslipFilePath) && File.Exists(pickslipFilePath))
                {
                    e.DisplayText = "Pick Slip Ready";
                    e.Appearance.BackColor = FileExistsBackColor;
                }
            }

            // Check if the current column is 'ArchiveFile'
            if (e.Column.FieldName == "ArchiveFile")
            {
                var labelFilePath = GetRowCellValue(e.RowHandle, Columns["LabelFile"])?.ToString();
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
                e.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                e.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);

                // Set the cell text to the tick or cross
                e.DisplayText = e.CellValue?.ToString() == "✓" ? "✓" : "✗";

                // Draw the cell
                e.Cache.DrawString(e.DisplayText, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), e.Bounds,
                    e.Appearance.GetStringFormat());
                e.Handled = true;
            }
        }
    }


    private void FileExistenceGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
    {
        if (FileLocationColumnNames.Contains(e.Column.FieldName))
        {
            var filePath = GetRowCellValue(e.RowHandle, e.Column)?.ToString();
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                var hyperlinkEdit = new RepositoryItemHyperLinkEdit
                {
                    Caption = FileExistsText,
                    LinkColor = Color.Blue, // Set the link color
                    SingleClick = true // Set the link to open on a single click
                };
                hyperlinkEdit.OpenLink += HyperlinkEdit_OpenLink;
                e.RepositoryItem = hyperlinkEdit;
            }
            else
            {
                var textEdit = new RepositoryItemTextEdit();
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

        var filePath = e.EditValue?.ToString();
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            try
            {
                // Open the file
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error printing file: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
    }

    private void FileExistenceGridView_CustomRowFilter(object sender, RowFilterEventArgs e)
    {
        if (FilterFileExists)
        {
            var anyFileExists = false;
            foreach (var columnName in FileLocationColumnNames)
            {
                var filePath = GetRowCellValue(e.ListSourceRow, Columns[columnName])?.ToString();
                var fileExists = !string.IsNullOrEmpty(filePath) && File.Exists(filePath);

                if (fileExists)
                {
                    e.Visible = true;
                    e.Handled = true;
                    break;
                }

                e.Visible = anyFileExists;
                e.Handled = true;
            }
        }
    }

    private bool _filterZShipmentID;

    public bool FilterZShipmentID
    {
        get => _filterZShipmentID;
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
            var zShipmentIDColumnIndex = 0;
            var zShipmentID = GetRowCellValue(e.ListSourceRow, Columns[zShipmentIDColumnIndex])?.ToString();
            var hasZShipmentID = !string.IsNullOrEmpty(zShipmentID);

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
            var rowHandle = gridView.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                // Get the value of the AccountingRef column
                var accountingRef = gridView.GetRowCellValue(rowHandle, "AccountingRef");

                // Get the value of the TotalWeight column
                var totalWeight = Convert.ToDecimal(gridView.GetRowCellValue(rowHandle, "TotalWeight"));

                if (accountingRef != null)
                {
                    if (totalWeight > 5)
                    {
                        XtraMessageBox.Show(
                            "Total weight cannot be more than 5. Please correct the values and try again.",
                            "Weight Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    // Show a confirmation message
                    var result =
                        XtraMessageBox.Show(
                            "Are you sure you want to create a pending shipment for " + accountingRef + "?",
                            "Confirm Execution", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Execute the stored procedure with the AccountingRef value
                        CreatePendingShipment(accountingRef);

                        // Show a success message
                        XtraMessageBox.Show("Successfully created Pending Shipment for " + accountingRef, "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        var storedProcedureName = "dbo.spInsertStarShipITOrder";

        // Use the connection string to create a new SqlConnection
        using (var connection = new SqlConnection(connectionString))
        {
            using (var command = new SqlCommand(storedProcedureName, connection))
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
    private void FileExistenceGridView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
    {
        if (e.Column.FieldName == "zEmployeeGroup" && e.Value?.ToString() == "NONE") e.DisplayText = "Pending";
    }


    private void FileExistenceGridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
    {
        var view = sender as GridView; // Cast the sender to the GridView

        if (view != null && e.Column.FieldName == "zEmployeeGroup")
        {
            var value = view.GetRowCellValue(e.RowHandle,
                "zEmployeeGroup"); // Use the view instance to call GetRowCellValue
            if (value?.ToString() == "NONE") e.Appearance.BackColor = Color.Orange; // Set background color to orange
        }
    }


    public void ApplyShippingMethodGrouping()
    {
        // Clear any existing grouping
        ClearGrouping();

        // Find the shipping method column (try common column names)
        GridColumn shippingMethodColumn = null;
        var possibleColumns = new[] { "ZShippingMethod" };

        foreach (var columnName in possibleColumns)
        {
            shippingMethodColumn = Columns.ColumnByFieldName(columnName);
            if (shippingMethodColumn != null)
                break;
        }

        if (shippingMethodColumn == null)
            // If no shipping method column found, try to find it by caption
            shippingMethodColumn = Columns
                .FirstOrDefault(c => c.Caption.ToLower().Contains("shipping"));

        if (shippingMethodColumn != null)
        {
            // Group by shipping method
            shippingMethodColumn.GroupIndex = 0;

            // Expand all groups by default
            ExpandAllGroups();

            // Apply custom cell styling for ONLY the ZShippingMethod column
            RowCellStyle += ShippingMethodCellStyle;
            CustomDrawGroupRow += ShippingMethodGroupRowStyle;
        }
        else
        {
            throw new InvalidOperationException(
                "Shipping method column not found. Expected columns: ZShippingMethod, ShippingMethod, or zShippingMethod");
        }
    }

    /// <summary>
    ///     Applies color coding to ONLY the ZShippingMethod cells based on shipping method
    /// </summary>
    private void ShippingMethodCellStyle(object sender, RowCellStyleEventArgs e)
    {
        // Only apply styling to the ZShippingMethod column
        if (e.Column.FieldName != "ZShippingMethod") return;

        if (e.RowHandle < 0) return; // Skip group rows

        var gridView = sender as GridView;
        if (gridView == null) return;

        // Get the shipping method value for this cell
        var shippingMethod = gridView.GetRowCellValue(e.RowHandle, e.Column)?.ToString();

        if (!string.IsNullOrEmpty(shippingMethod)) ApplyShippingMethodColors(shippingMethod.Trim(), e.Appearance);
    }

    /// <summary>
    ///     Applies color coding to group headers based on shipping method
    /// </summary>
    private void ShippingMethodGroupRowStyle(object sender, RowObjectCustomDrawEventArgs e)
    {
        var gridView = sender as GridView;
        if (gridView == null) return;

        var info = e.Info as GridGroupRowInfo;
        if (info == null) return;

        // Only apply styling if this is a ZShippingMethod group
        if (info.Column?.FieldName != "ZShippingMethod") return;

        // Extract shipping method from group text
        var groupText = info.GroupText;
        var shippingMethod = ExtractShippingMethodFromGroupText(groupText);

        if (!string.IsNullOrEmpty(shippingMethod)) ApplyShippingMethodColors(shippingMethod.Trim(), info.Appearance);
    }

    /// <summary>
    ///     Extracts shipping method from group row text
    /// </summary>
    private string ExtractShippingMethodFromGroupText(string groupText)
    {
        if (string.IsNullOrEmpty(groupText)) return null;

        // Group text format is usually like "ZShippingMethod: 7C55 (5 items)"
        var colonIndex = groupText.IndexOf(':');
        if (colonIndex >= 0 && colonIndex < groupText.Length - 1)
        {
            var afterColon = groupText.Substring(colonIndex + 1).Trim();
            var openParenIndex = afterColon.IndexOf('(');

            if (openParenIndex >= 0) return afterColon.Substring(0, openParenIndex).Trim();

            return afterColon;
        }

        return null;
    }

    /// <summary>
    ///     Applies the appropriate colors based on shipping method
    /// </summary>
    private void ApplyShippingMethodColors(string shippingMethod, AppearanceObject appearance)
    {
        switch (shippingMethod.ToUpper().Trim())
        {
            case "7C55":
                // AustPost - Red background with white text
                appearance.BackColor = Color.PaleVioletRed;
                appearance.ForeColor = Color.White;
                break;

            case "FPP":
                // StarTrack - Blue background with white text
                appearance.BackColor = Color.LightSkyBlue;
                appearance.ForeColor = Color.Black;
                break;

            default:
                // Default colors for other shipping methods
                appearance.BackColor = Color.LightGray;
                appearance.ForeColor = Color.Black;
                break;
        }
    }


    public bool ValidateShippingMethods(string[] restrictedMethods, string shippingMethodColumnName = "ZShippingMethod", string batchType = "this batch")
    {
        var invalidOrders = new List<string>();
        var selectedRowHandles = GetSelectedRows();

        if (selectedRowHandles.Length == 0)
        {
            XtraMessageBox.Show("Please select one or more rows first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // Check each selected row for restricted shipping methods
        foreach (var rowHandle in selectedRowHandles)
        {
            var shippingMethod = GetRowCellValue(rowHandle, shippingMethodColumnName)?.ToString();
            var accountingRef = GetRowCellValue(rowHandle, "AccountingRef")?.ToString();

            if (!string.IsNullOrEmpty(shippingMethod))
            {
                // Check if shipping method matches any restricted method (case insensitive)
                foreach (var restrictedMethod in restrictedMethods)
                {
                    if (shippingMethod.IndexOf(restrictedMethod, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        invalidOrders.Add($"{accountingRef} ({shippingMethod})");
                        break; // No need to check other restricted methods for this order
                    }
                }
            }
        }

        // If restricted shipping methods found, show warning and prevent processing
        if (invalidOrders.Any())
        {
            var restrictedMethodsList = string.Join(", ", restrictedMethods);
            var invalidOrdersList = string.Join("\n", invalidOrders);

            XtraMessageBox.Show(
                $"The following orders have restricted shipping methods ({restrictedMethodsList}) and cannot be processed in {batchType}:\n\n{invalidOrdersList}\n\n" +
                "Please use the correct batch for these orders or deselect them.",
                "Invalid Shipping Method",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        return true;
    }

    public void SetHeaderColors(Color backColor, Color foreColor)
    {
        // Set appearance for group row headers (the "ZShipping Method: 7C55" row)
        Appearance.GroupRow.BackColor = backColor;
        Appearance.GroupRow.ForeColor = foreColor;
        Appearance.GroupRow.Options.UseBackColor = true;
        Appearance.GroupRow.Options.UseForeColor = true;

        // Also set the font to bold for better visibility
        Appearance.GroupRow.Font = new Font(Appearance.GroupRow.Font, FontStyle.Bold);
        Appearance.GroupRow.Options.UseFont = true;

        // Refresh the grid to apply changes
        RefreshData();
    }


    /// <summary>
    /// Sets group row colors based on shipping method for dynamic coloring
    /// This will automatically color group rows based on their shipping method value
    /// </summary>
    public void EnableDynamicGroupRowColoring()
    {
        CustomDrawGroupRow += (sender, e) =>
        {
            GridView view = sender as GridView;
            if (view != null)
            {
                // Get the group row info
                GridGroupRowInfo info = e.Info as GridGroupRowInfo;
                if (info != null && info.Column.FieldName == "ZShippingMethod")
                {
                    // Extract the shipping method value from the group row text
                    string groupText = view.GetGroupRowDisplayText(e.RowHandle);

                    // Parse shipping method from text like "ZShipping Method: 7C55 (5 items)"
                    if (groupText.Contains(":"))
                    {
                        string[] parts = groupText.Split(':');
                        if (parts.Length > 1)
                        {
                            string methodPart = parts[1].Trim();
                            // Remove item count if present - get just the method
                            if (methodPart.Contains("("))
                            {
                                methodPart = methodPart.Substring(0, methodPart.IndexOf("(")).Trim();
                            }

                            // Apply colors based on shipping method
                            switch (methodPart.ToUpper().Trim())
                            {
                                case "7C55":
                                    // AustPost - Red background with white text
                                    e.Appearance.BackColor = Color.PaleVioletRed;
                                    e.Appearance.ForeColor = Color.White;
                                    break;
                                case "FPP":
                                    // StarTrack - Blue background with white text
                                    e.Appearance.BackColor = Color.LightSkyBlue;
                                    e.Appearance.ForeColor = Color.Black;
                                    break;
                                default:
                                    // Default colors for other shipping methods
                                    e.Appearance.BackColor = Color.LightGray;
                                    e.Appearance.ForeColor = Color.Black;
                                    break;
                            }

                            // Make the font bold for better visibility
                            e.Appearance.Font = new Font(e.Appearance.Font ?? SystemFonts.DefaultFont, FontStyle.Bold);
                            e.Appearance.Options.UseFont = true;
                            e.Appearance.Options.UseBackColor = true;
                            e.Appearance.Options.UseForeColor = true;
                        }
                    }
                }
            }
        };
    }

    /// <summary>
    /// Validates shipping methods specifically for AustPost batches (excludes FPP)
    /// </summary>
    /// <returns>True if validation passes, false if FPP methods are found</returns>
    public bool ValidateForAustPostBatch()
    {
        return ValidateShippingMethods(new[] { "FPP" }, "ZShippingMethod", "AustPost batch");
    }

    /// <summary>
    /// Validates shipping methods specifically for StarTrack batches (excludes 7C55)
    /// </summary>
    /// <returns>True if validation passes, false if 7C55 methods are found</returns>
    public bool ValidateForStarTrackBatch()
    {
        return ValidateShippingMethods(new[] { "7C55" }, "ZShippingMethod", "StarTrack batch");
    }

    /// <summary>
    /// Applies color coding to shipping method cells for visual identification
    /// </summary>
    /// <param name="shippingMethod">The shipping method value</param>
    /// <param name="appearance">The appearance object to modify</param>


    /// <summary>
    /// Enables automatic color coding for shipping method column
    /// Call this method after setting up your grid to automatically color shipping method cells
    /// </summary>
    /// <param name="shippingMethodColumnName">Name of the column containing shipping method data</param>
    public void EnableShippingMethodColoring(string shippingMethodColumnName = "ZShippingMethod")
    {
        CustomDrawCell += (sender, e) =>
        {
            if (e.Column.FieldName == shippingMethodColumnName)
            {
                var shippingMethod = e.CellValue?.ToString();
                ApplyShippingMethodColors(shippingMethod, e.Appearance);
            }
        };
    }

    /// <summary>
    ///     Removes shipping method grouping and restores default view
    /// </summary>
    public void RemoveShippingMethodGrouping()
    {
        ClearGrouping();
        RowCellStyle -= ShippingMethodCellStyle;
        CustomDrawGroupRow -= ShippingMethodGroupRowStyle;
    }

    /// <summary>
    /// Removes shipping method grouping and restores default view
    /// </summary>
}

// Extension method to easily call from your forms
public static class GridViewExtensions
{
    /// <summary>
    ///     Extension method to apply shipping method grouping to any FileExistenceGridView
    /// </summary>
    public static void GroupByShippingMethod(this FileExistenceGridView gridView)
    {
        gridView.ApplyShippingMethodGrouping();
    }
}