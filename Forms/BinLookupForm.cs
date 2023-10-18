using DevExpress.XtraEditors;
using DevExpress.XtraMap;
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
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Forms
{
    public partial class BinLookupForm : DevExpress.XtraEditors.XtraForm
    {
        private int warehouseLocationID;
        private string productCode;
        private int transferDetailID;
        private readonly IConfiguration  _configuration;
        private readonly OMDbContext _context;


        public delegate void BinNumberSelectedHandler(string selectedColumnName, string selectedBinNumber);
        public event BinNumberSelectedHandler BinNumberSelected;


        public BinLookupForm(IConfiguration configuration, OMDbContext context, int warehouseLocationID, string productCode, int transferDetailID)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            this.productCode = productCode;
            this.warehouseLocationID = warehouseLocationID;
            this.transferDetailID = transferDetailID;
            Load += BinTransferForm_Load;
            gridView1.DoubleClick += GridView1_DoubleClick;
            gridView1.RowCellStyle += gridView1_RowCellStyle;

        }



        private void BinTransferForm_Load(object sender, EventArgs e)
        {
            SetBinLookups(warehouseLocationID, productCode);
        }

        private void SetBinLookups(int warehouseLocationID, string productCode)
        {

            {
                // Fetching bin details based on the selected productCode
                var binDetails = (
                    from p in _context.Products  // Start with the Products table
                    join pbc in _context.PBinContents on p.UniqueID equals pbc.ProductID  // Join with PBinContents on the ProductID field
                    join b in _context.PBins on pbc.BinID equals b.BinID  // Join with PBins on the BinID field
                    where b.Location == warehouseLocationID  // Filter by the warehouse location ID
                          && b.Type != "A"  // Exclude bins of Type "A"
                          && p.ProductCode == productCode  // Filter by the product code
                    select new BinDetail  // Project the result into a new BinDetail object
                    {
                        BinNumber = b.BinNumber,
                        BinID = b.BinID,
                        ActualQuantity = pbc.ActualQuantity
                    }
                ).ToList();  // Execute the query and convert the result to a List<BinDetail>

                // Check if any records were returned
                if (!binDetails.Any())
                {
                    XtraMessageBox.Show("No bin details found for the selected warehouse location ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Assuming gridControl1 is the GridControl associated with your GridView
                gridControl1.DataSource = binDetails;
            }
        }


        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            var view = (GridView)sender;
            var point = view.GridControl.PointToClient(Control.MousePosition);
            var rowHandle = view.CalcHitInfo(point).RowHandle;


            if (rowHandle >= 0) // Ensure a valid row is double-clicked
            {
                var binNumber = view.GetRowCellValue(rowHandle, "BinNumber").ToString(); // Adjust the field name as necessary
            
                var actualQuantity = (decimal)view.GetRowCellValue(rowHandle, "ActualQuantity"); 

                using (var choiceForm =
                       new BinLookupChoiceForm(view, rowHandle, binNumber, actualQuantity)) // Pass BinID to the choice form
                {
                    var dialogResult = choiceForm.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        var selectedColumnName = choiceForm.SelectedColumnName;
                        var selectedBinNumber = choiceForm.SelectedBinNumber;
                        //var selectedBinID = choiceForm.SelectedBinID; // Get the selected BinID from the choice form
                        var selectedActualQuantity = choiceForm.SelectedActualQuantity; // Get the selected BinID from the choice form

                        // Raise event to carry both the selected bin number and BinID back to WarehouseTransferForm2
                        BinNumberSelected?.Invoke(selectedColumnName, selectedBinNumber);
                    }
                }
            }


        }

        // This method gets invoked for every cell in GridView, allowing you to change cell styles conditionally
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "ActualQuantity") // Change the FieldName based on your actual setup
            {
                // Get the value of ActualQuantity in the current row
                int actualQuantity = Convert.ToInt32(gridView1.GetRowCellValue(e.RowHandle, "ActualQuantity"));

                // Set the cell style based on the value of ActualQuantity
                if (actualQuantity >= 20)
                {
                    e.Appearance.BackColor = Color.Green;
                    e.Appearance.ForeColor = Color.White;
                }
                else if (actualQuantity > 0 && actualQuantity < 20)
                {
                    e.Appearance.BackColor = Color.Orange;
                    e.Appearance.ForeColor = Color.White;
                }
                else // For ActualQuantity <= 0
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.ForeColor = Color.White;
                }
            }
        }

    }
}