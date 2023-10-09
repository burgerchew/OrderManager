using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderManagerEF.Data;
using Microsoft.Extensions.Configuration;

namespace OrderManagerEF.Forms
{
    public partial class BatchForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public BatchForm(IConfiguration configuration, OMDbContext context)
        {

            InitializeComponent();
            _configuration = configuration;
            _context = context;
            this.WindowState = FormWindowState.Maximized;

            var batches = _context.PendingBatches.ToList();

            // Bind the fetched data to the grid or whatever control you are using
            gridControl1.DataSource = batches; // replace 'yourGridControl' with the actual control's name
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
            barButtonItem2.ItemClick += barButtonItem2_ItemClick;
        }


        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = XtraMessageBox.Show(
                "All data in the tables will be purged. Are you sure you want to continue?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {

                string connectionString = _configuration.GetConnectionString("RubiesConnectionString");
                string[] tablesToTruncate =
                {
                    "LabelstoPrintDS", "LabelstoPrintRUB", "LabelstoPrintCSC", "LabelstoPrint","LabelstoPrintNZ"
                }; // Add the names of the tables you want to truncate

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (string tableName in tablesToTruncate)
                            {
                                using (SqlCommand command = new SqlCommand($"TRUNCATE TABLE {tableName}", connection,
                                           transaction))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            XtraMessageBox.Show($"Error truncating tables: {ex.Message}");
                        }
                    }
                }
            }

            RefreshGridView();
        }

        private void RefreshGridView()
        {
            // Fetch the updated data from the vPendingBatches view using EF Core
            var batches = _context.PendingBatches.ToList();

            // Bind the fetched data to the grid
            gridControl1.DataSource = batches;

            // Refresh the grid data
            gridView1.RefreshData();
        }


        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.GetSelectedRows().Length == 0)
            {
                XtraMessageBox.Show("Please select at least one row to delete.");
                return;
            }

            DialogResult result = XtraMessageBox.Show(
                "Are you sure you want to delete the selected rows?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DeleteSelectedRows();
                RefreshGridView(); // Assuming you have a RefreshGridView method as shown in the previous answers
            }
        }

        private void DeleteSelectedRows()
        {
            string connectionString = _configuration.GetConnectionString("RubiesConnectionString");
            string deleteCommandTemplate = "DELETE FROM {0} WHERE SalesOrder = @AccountingRef";



            // Mapping between StarShipITAccount values and table names
            Dictionary<string, string> tableMapping = new Dictionary<string, string>
            {
                { "CSC", "LabelstoPrintCSC" },
                { "RUB", "LabelstoPrintRUB" },
                { "DS", "LabelstoPrintDS" },
                { "BSA", "LabelstoPrint" },
                { "NZ", "LabelstoPrintNZ" }
            };


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int[] selectedRows = gridView1.GetSelectedRows();
                foreach (int rowHandle in selectedRows)
                {
                    DataRowView rowView = (DataRowView)gridView1.GetRow(rowHandle);
                    string starShipITAccount =
                        rowView["StarShipITAccount"]
                            .ToString(); // Replace "StarShipITAccount" with the actual column name containing the account code
                    string
                        accountingRef =
                            rowView["SalesOrder"]
                                .ToString(); // Replace "AccountingRef" with the actual column name containing the primary key

                    if (tableMapping.TryGetValue(starShipITAccount, out string tableName))
                    {
                        string deleteCommandText = string.Format(deleteCommandTemplate, tableName);

                        using (SqlCommand command = new SqlCommand(deleteCommandText, connection))
                        {
                            command.Parameters.AddWithValue("@AccountingRef", accountingRef);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show($"Error: Unknown StarShipITAccount value '{starShipITAccount}'.");
                    }
                }
            }

        }
    }
}