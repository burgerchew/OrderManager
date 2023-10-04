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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace OrderManagerEF
{
    public partial class ReportSettingForm : XtraForm
    {
        private ReportManager _reportManager;
        private readonly IConfiguration _configuration;

        public ReportSettingForm(IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;

            _reportManager = new ReportManager(_configuration);
            LoadReportSetting();
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Assuming you have textEditLabelPath and textEditSalesOrderNumber as your text edit fields in the form
            string labelPath = textEdit1.Text;
            string pickslipPath = textEdit2.Text;
            string errorPath = textEdit3.Text;

            if (string.IsNullOrEmpty(labelPath))
            {
                XtraMessageBox.Show("Label path cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(pickslipPath))
            {
                XtraMessageBox.Show("Pickslip path cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(errorPath))
            {
                XtraMessageBox.Show("Error path cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a ReportSetting object with the updated values
            ReportSetting reportSetting = new ReportSetting
            {
                LabelPath = labelPath,
                PickSlipPath = pickslipPath,
                ErrorPath = errorPath
            };

            // Save or update the report setting in the database
            // You can decide whether to save or update based on the existence of an Id value
            if (reportSetting.Id == 0)
            {
                int newId = _reportManager.SaveReportSetting(reportSetting);
                reportSetting.Id = newId; // Update the ReportSetting object with the new Id
                XtraMessageBox.Show("Report setting has been saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _reportManager.UpdateReportSetting(reportSetting);
                XtraMessageBox.Show("Report setting has been updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void LoadReportSetting()
        {
            ReportSetting reportSetting = _reportManager.GetReportSetting();

            if (reportSetting != null)
            {
                textEdit1.Text = reportSetting.LabelPath;
                textEdit2.Text = reportSetting.PickSlipPath;
                textEdit3.Text = reportSetting.ErrorPath;
            }
        }

    }
}