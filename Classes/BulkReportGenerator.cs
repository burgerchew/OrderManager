using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Pdf.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraWaitForm;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraSplashScreen;
using Microsoft.Extensions.Configuration;
using OrderManager;
using OrderManagerEF.Reports;


namespace OrderManagerEF.Classes
{
    public class BulkReportGenerator
    {
       
        private ReportManager _reportManager;
        private readonly IConfiguration _configuration;

        public BulkReportGenerator(IConfiguration configuration)
        {
         
            _configuration = configuration;
            _reportManager = new ReportManager(_configuration);
        }
       

        public void GenerateAndSaveReportsProgressPath(List<string> salesOrderReferences, Action<int> progressCallback, Action<string> errorCallback)
        {
            int totalReports = salesOrderReferences.Count;
            int currentReport = 0;

            // Retrieve the report setting from the database
            ReportSetting reportSetting = _reportManager.GetReportSetting();

            if (reportSetting != null)
            {
                int successfulReports = 0;

                foreach (string salesOrderRef in salesOrderReferences)
                {
                    try
                    {
                        // Pass the errorCallback to GenerateReportPortrait
                        XtraReport report = GenerateReportPortrait(salesOrderRef, errorCallback);

                        if (report != null) // Check if report is not null (i.e., successful)
                        {
                            // Call the SaveReport method with the pickslipPath from the reportSetting object
                            SaveReportPath(report, salesOrderRef, reportSetting.PickSlipPath);

                            // Update the transheaders table
                            UpdateTransHeaders(salesOrderRef);

                            successfulReports++;
                            string successMessage = $"{successfulReports} out of {totalReports} pickslips generated successfully.";
                            SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetMessage, successMessage);
                        }
                    }
                    catch (Exception ex) // Handle exceptions
                    {
                        string errorMessage = $"An error occurred for {salesOrderRef}. Details: {ex.Message}. Skipping...";
                        SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetMessage, errorMessage);
                    }

                    // Update the progress
                    currentReport++;
                    int progress = (int)((double)currentReport / totalReports * 100);
                    progressCallback(progress);
                }
            }

            else
            {
                // Handle the case when the report setting is not available
                XtraMessageBox.Show("Report settings are not available. Please configure the settings first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GenerateAndSavePreorderReportsProgressPath(List<string> salesOrderReferences, Action<int> progressCallback, Action<string> errorCallback)
        {
            int totalReports = salesOrderReferences.Count;
            int currentReport = 0;

            // Retrieve the report setting from the database
            ReportSetting reportSetting = _reportManager.GetReportSetting();

            if (reportSetting != null)
            {
                int successfulReports = 0;

                foreach (string salesOrderRef in salesOrderReferences)
                {
                    try
                    {
                        // Pass the errorCallback to GenerateReportPortrait
                        XtraReport report = GenerateReportPreorderPortrait(salesOrderRef, errorCallback);

                        if (report != null) // Check if report is not null (i.e., successful)
                        {
                            // Call the SaveReport method with the pickslipPath from the reportSetting object
                            SaveReportPath(report, salesOrderRef, reportSetting.PickSlipPath);

                            // Update the transheaders table
                            UpdateTransHeaders(salesOrderRef);

                            successfulReports++;
                            string successMessage = $"{successfulReports} out of {totalReports} pickslips generated successfully.";
                            SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetMessage, successMessage);
                        }
                    }
                    catch (Exception ex) // Handle exceptions
                    {
                        string errorMessage = $"An error occurred for {salesOrderRef}. Details: {ex.Message}. Skipping...";
                        SplashScreenManager.Default.SendCommand(ProgressForm.SplashScreenCommand.SetMessage, errorMessage);
                    }

                    // Update the progress
                    currentReport++;
                    int progress = (int)((double)currentReport / totalReports * 100);
                    progressCallback(progress);
                }
            }

            else
            {
                // Handle the case when the report setting is not available
                XtraMessageBox.Show("Report settings are not available. Please configure the settings first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        //Older Version
        public XtraReport GenerateReportPortrait(string salesOrderReference, Action<string> errorCallback)
        {


            if (!IsDataPresentForSalesOrder(salesOrderReference, errorCallback))
            {
                return null;
            }

            // Create a new report instance
            PickSlipReportPortrait report = new PickSlipReportPortrait();

            // Set the sales order reference parameter
            report.Parameters["SalesOrderReferenceParam"].Value = salesOrderReference;

            // Apply the parameter (set the second argument to 'true')
            report.Parameters["SalesOrderReferenceParam"].Visible = false;

            // Make sure the data is filtered by the parameter
            report.FilterString = "[AccountingRef] = ?SalesOrderReferenceParam";

            return report;
        }


        public XtraReport GenerateReportPreorderPortrait(string salesOrderReference, Action<string> errorCallback)
        {


            if (!IsDataPresentForSalesOrder(salesOrderReference, errorCallback))
            {
                return null;
            }

            // Create a new report instance
            PreorderPickslipReport report = new PreorderPickslipReport();

            // Set the sales order reference parameter
            report.Parameters["SalesOrderReferenceParam"].Value = salesOrderReference;

            // Apply the parameter (set the second argument to 'true')
            report.Parameters["SalesOrderReferenceParam"].Visible = false;

            // Make sure the data is filtered by the parameter
            report.FilterString = "[AccountingRef] = ?SalesOrderReferenceParam";

            return report;
        }


        public bool IsDataPresentForSalesOrder(string salesOrderReference, Action<string> errorCallback)
        {
            // Retrieve all data using the stored procedure
            DataTable result = ExecutePickSlipProcedure();

            // Check if the data contains any rows matching the sales order reference
            DataRow[] filteredRows = result.Select("AccountingRef = '" + salesOrderReference + "'");
            if (filteredRows.Length == 0)
            {
                // Call the errorCallback and pass the error message
                errorCallback($"No data found for the provided sales order reference {salesOrderReference}.");
                return false;
            }

            return true;
        }

        private DataTable ExecutePickSlipProcedure()
        {
            DataTable dataTable = new DataTable();
            string connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("ASP_PickSlip", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }


        private void SaveReportPath(XtraReport report, string salesOrderNumber, string pickslipPath)
        {
            if (report == null || salesOrderNumber == null || pickslipPath == null)
            {
                // Optionally log or show a warning that a parameter is null
                return; // Skip the record if any parameter is null
            }

            // Set the path where you want to save the reports using the pickslipPath
            string uncPath = pickslipPath;

            // Set the filename using the sales order number
            string filename = $"{salesOrderNumber}.pdf";

            // Combine the UNC path and filename
            string fullPath = Path.Combine(uncPath, filename);

            PdfExportOptions pdfOptions = new PdfExportOptions
            {
                Compressed = true,
                ImageQuality = PdfJpegImageQuality.Lowest
            };

            // Save the report as a PDF file
            report.ExportToPdf(fullPath, pdfOptions);
        }



        public void UpdateTransHeaders(string salesOrder)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UPDATE transheaders SET ZEmployeeGroup = C.CustomerGroup FROM transheaders as t " +
                                                           "INNER JOIN CUSTOMERS as c ON c.UniqueId = t.accountid WHERE AccountingRef = @SalesOrder", connection))
                {
                    command.Parameters.AddWithValue("@SalesOrder", salesOrder);
               

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
