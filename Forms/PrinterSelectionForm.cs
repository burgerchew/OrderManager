using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Forms
{
    public partial class PrinterSelectionForm : XtraForm
    {
        private string _selectedPrinter;
        private readonly IConfiguration _configuration;
        private int _previousSelectedIndex = -1;
        private readonly OMDbContext _context;
        private readonly UserSession _userSession;
        public PrinterSelectionForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            _userSession = userSession;
            Load += PrinterSelectionForm_Load;
            comboBoxEdit1.EditValueChanged += comboBoxEdit1_EditValueChanged;

        }


        private void PrinterSelectionForm_Load(object sender, EventArgs e)
        {
            if (PrinterHelperEF.PopulatePrinterComboBox(comboBoxEdit1) && comboBoxEdit1.Properties.Items.Count > 0)
            {
                // Get the default printer name
                string userPrinterName = PrinterHelperEF.GetUserPrinter(_context, _userSession.CurrentUser.Id);

                // Set the selected printer in the combobox
                if (string.IsNullOrEmpty(userPrinterName))
                {
                    comboBoxEdit1.SelectedIndex = 0;
                }
                else
                {
                    int index = comboBoxEdit1.Properties.Items.IndexOf(userPrinterName);
                    if (index >= 0)
                    {
                        comboBoxEdit1.SelectedIndex = index;
                    }
                    else
                    {
                        comboBoxEdit1.SelectedIndex = 0;
                    }
                }
            }
        }

        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.SelectedIndex != _previousSelectedIndex)
            {
                _previousSelectedIndex = comboBoxEdit1.SelectedIndex;
                _selectedPrinter = comboBoxEdit1.EditValue?.ToString();
            }
        }



        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedPrinter))
            {
                PrinterHelperEF.SaveUserPrinter(_context, _userSession.CurrentUser.Id, _selectedPrinter);
                XtraMessageBox.Show("Printer settings saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XtraMessageBox.Show("Please select a printer before saving.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}