using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF.Forms
{
    public partial class BinLookupChoiceForm : DevExpress.XtraEditors.XtraForm
    {
        public string SelectedColumnName { get; private set; }
        public string SelectedBinNumber { get; private set; }
       // public int SelectedBinID { get; private set; }  // Adding property to hold the selected BinID

        public decimal SelectedActualQuantity { get; private set; }  // Adding property to hold the selected BinID

        private readonly GridView _detailView;
        private readonly int _rowHandle;


        public BinLookupChoiceForm(GridView detailView, int rowHandle, string binNumber, decimal actualQuantity) // Adjust constructor to accept BinID
        {
            _detailView = detailView;
            _rowHandle = rowHandle;
            SelectedBinNumber = binNumber;
           // SelectedBinID = binID;
            SelectedActualQuantity = actualQuantity;

            InitializeComponent();
            ButtonLoad();
        }

        private void ButtonLoad()
        {
            // Set the size of the buttons
            var fromBinNumberButton = new SimpleButton { Text = $"FromBinNumber <-- ({SelectedBinNumber})", Size = new Size(200, 50) };
            var toBinNumberButton = new SimpleButton { Text = $"ToBinNumber --> ({SelectedBinNumber})", Size = new Size(200, 50) };

            // Set the location of the buttons to center them on the form
            fromBinNumberButton.Location = new Point((this.ClientSize.Width - fromBinNumberButton.Width) / 2, this.ClientSize.Height / 2 - fromBinNumberButton.Height - 20);
            toBinNumberButton.Location = new Point((this.ClientSize.Width - toBinNumberButton.Width) / 2, this.ClientSize.Height / 2 + 20);

            //// Assign Click event handlers
            //toBinNumberButton.Click += (s, e) => UpdateGrid("ToBinNumber");
            //fromBinNumberButton.Click += (s, e) => UpdateGrid("FromBinNumber");

            //// Add the buttons to the form's controls
            //Controls.Add(toBinNumberButton);
            //Controls.Add(fromBinNumberButton);


            // Assign Click event handlers
            toBinNumberButton.Click += (s, e) => UpdateGrid("ToLocation");
            fromBinNumberButton.Click += (s, e) =>
            {
                if (SelectedActualQuantity == 0)
                {
                    XtraMessageBox.Show("You need to select a bin with quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                UpdateGrid("FromLocation");
            };

            // Add the buttons to the form's controls
            Controls.Add(toBinNumberButton);
            Controls.Add(fromBinNumberButton);


        }

        private void UpdateGrid(string columnName)
        {
            SelectedColumnName = columnName;

            this.DialogResult = DialogResult.OK; // Set dialog result to OK to signal that a choice was made
            this.Close();
        }
    }
}