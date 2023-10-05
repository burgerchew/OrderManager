using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
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
using System.Data.SqlClient;

namespace OrderManagerEF
{
    public partial class Import1Form : RibbonForm
    {
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public Import1Form(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;

            var addressParts = _context.AddressParts.ToList();
            gridControl1.DataSource = addressParts;


            // Set up the MemoEdit column after the data is loaded
            ReadOnlyColumns();
            SetupMemoEditColumn("ExtraText");
            SetupEditFormMemoEdit("ExtraText");
            // Save changes and refresh
            UpdateAndRefresh();

            // Add and validate data
            ValidateAndSetDisplayText();
        }


        private void SetupMemoEditColumn(string columnName)
        {
            var memoEdit = new RepositoryItemMemoEdit();

            // Adjust properties as necessary
            memoEdit.WordWrap = true;
            memoEdit.ScrollBars = ScrollBars.Vertical;

            gridView1.Columns[columnName].ColumnEdit = memoEdit;

            // Set row auto height
            gridView1.OptionsView.RowAutoHeight = true;
        }

        private void SetupEditFormMemoEdit(string columnName)
        {
            gridView1.EditFormPrepared += (sender, e) =>
            {
                var view = sender as GridView;
                if (view != null)
                {
                    var memo = e.BindableControls[columnName] as MemoEdit;
                    if (memo != null)
                    {
                        memo.Properties.ScrollBars = ScrollBars.Vertical;
                        memo.Properties.WordWrap = true;
                        memo.Height = 150; // Adjust the height as necessary
                    }
                }
            };

            gridView1.RowUpdated += (sender, e) =>
            {
                // Save changes and refresh the GridView after the row is updated
                UpdateAndRefresh();
            };
        }


        private void UpdateAndRefresh()
        {
            try
            {
                // Commit changes in the GridView
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

                // Re-fetch the data from the database using EF Core
                var addressParts = _context.AddressParts.ToList();

                // Bind the refreshed data to the grid
                gridControl1.DataSource = addressParts;

                // Refresh the data in the GridView
                gridView1.RefreshData();
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                XtraMessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void ReadOnlyColumns()
        {
            // Make all columns read only
            foreach (GridColumn col in gridView1.Columns) col.OptionsColumn.ReadOnly = true;

            // Enable editing on specific columns
            gridView1.Columns["ExtraText"].OptionsColumn.ReadOnly = false;
            gridView1.Columns["TradingRef"].OptionsColumn.ReadOnly = false;
        }


        private void barButtonItem1_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            var gridView = gridControl1.MainView as GridView;

            // Get the selected rows in the GridView
            var selectedRows = gridView.GetSelectedRows();

            var rowsInserted = 0;

            // Iterate through the selected rows
            foreach (var rowHandle in selectedRows)
            {
                // Get the accountingref value from the current row
                var accountingref = gridView.GetRowCellValue(rowHandle, "AccountingRef").ToString();

                var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.CopyTransHeadersToOrdersNoCursor", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AccountingRef", accountingref);
                        connection.Open();
                        try
                        {
                            var result = command.ExecuteNonQuery();
                            if (result >= 0)
                                rowsInserted++; // Increment rowsInserted by 1 for each successful execution
                            else
                                XtraMessageBox.Show(
                                    $"There was something wrong with the order with AccountingRef: {accountingref}, and it cannot be inserted into the database",
                                    "Database Insertion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (SqlException ex)
                        {
                            // Handle or throw other exceptions
                            XtraMessageBox.Show($"{ex}", "Database Insertion Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }

            // Show a message box with the number of rows inserted
            XtraMessageBox.Show(rowsInserted + " row(s) inserted successfully.", "Insert Rows", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private bool IsValidRow(GridView view, int rowHandle)
        {
            bool isValid = true;

            // Columns to validate
            string[] columnsToValidate = new string[] { "AddressLine1", "AddressLine2", "AddressLine3", "State", "Postcode" };

            foreach (string columnName in columnsToValidate)
            {
                object value = view.GetRowCellValue(rowHandle, columnName);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        private void ValidateAndSetDisplayText()
        {
            gridView1.BeginDataUpdate();

            // Add a "Status" column if it doesn't exist
            if (gridView1.Columns["Status"] == null)
            {
                GridColumn statusColumn = gridView1.Columns.AddVisible("Status");
                statusColumn.OptionsColumn.ReadOnly = true;  // Make it read-only
            }

            // Custom column display text for the "Status" column
            gridView1.CustomColumnDisplayText += (sender, e) =>
            {
                if (e.Column.FieldName == "Status")
                {
                    // Get the validity of the row
                    bool rowIsValid = IsValidRow(gridView1, e.ListSourceRowIndex);

                    // Set the cell text to the tick or cross
                    e.DisplayText = rowIsValid ? "Valid" : "Error";
                }
            };

            gridView1.RowCellStyle += (sender, e) =>
            {
                if (e.Column.FieldName == "Status")
                {
                    // Get the validity of the row
                    bool rowIsValid = IsValidRow(gridView1, e.RowHandle);

                    // Set the cell background color based on its validity
                    if (rowIsValid)
                    {
                        e.Appearance.BackColor = Color.Green;
                        e.Appearance.ForeColor = Color.White;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.ForeColor = Color.White;
                    }
                }
            };


            gridView1.EndDataUpdate();
        }




    }
}