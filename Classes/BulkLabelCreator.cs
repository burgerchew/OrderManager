using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrderManagerEF.Classes;

public class BulkLabelCreator
{
    private readonly GridView _gridView;
    private readonly int[] rowHandles;
    private readonly string _starshipItApiKey;
    private readonly string _ocpApimSubscriptionKey;
    private readonly ProgressBarControl _progressBarControl;
    private readonly Form _form;
    private ReportManager _reportManager;
    private readonly ApiRequestHelper _apiRequestHelper;
    private readonly IConfiguration _configuration;

    public BulkLabelCreator(IConfiguration configuration, GridView gridView, string starshipItApiKey, string ocpApimSubscriptionKey, int[] rowHandles,
        ProgressBarControl progressBarControl, Form form)
    {
        _gridView = gridView;
        _starshipItApiKey = starshipItApiKey;
        _ocpApimSubscriptionKey = ocpApimSubscriptionKey;
        this.rowHandles = rowHandles;
        _progressBarControl = progressBarControl;
        _form = form;
        _configuration=configuration;
        _apiRequestHelper = new ApiRequestHelper();
    }

    public async Task BulkLabelSendApiRequestAsync()
    {
        _progressBarControl.Properties.Maximum = rowHandles.Length;
        _progressBarControl.EditValue = 0;
        _progressBarControl.Invoke(new Action(() => _progressBarControl.Visible = true));

        _form.Invoke(new Action(() => _form.Refresh()));
        // Get the focused row from the GridView
        foreach (var rowHandle in rowHandles)
        {
            var order_id = _gridView.GetRowCellValue(rowHandle, "order_id");
            var reprint = _gridView.GetRowCellValue(rowHandle, "reprint");
            var weight = _gridView.GetRowCellValue(rowHandle, "weight");
            var height = _gridView.GetRowCellValue(rowHandle, "height");
            var width = _gridView.GetRowCellValue(rowHandle, "width");
            var length = _gridView.GetRowCellValue(rowHandle, "length");
            var order_number = _gridView.GetRowCellValue(rowHandle, "order_number");

            var data = new
            {
                order_id,
                reprint,
                packages = new[]
                {
                    new
                    {
                        weight,
                        height,
                        width,
                        length
                    }
                }
            };

            var apiUrl = "https://api.starshipit.com/api/orders/shipment";
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", _starshipItApiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _ocpApimSubscriptionKey);

                response = await client.PostAsJsonAsync(apiUrl, data);
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();


                try
                {
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var base64EncodedPdfsArray = jsonDocument.RootElement.GetProperty("labels");

                    var base64EncodedPdf = base64EncodedPdfsArray[0].GetString();
                    var pdfBytes = Convert.FromBase64String(base64EncodedPdf);

                    var pdfFileName = $"{order_number}.pdf";

                    // Retrieve the report setting

                
                    var reportManager = new ReportManager(_configuration);

                    var setting = reportManager.GetReportSetting();
                    if (setting == null) throw new Exception("LabelPath setting not found");

                    // Use the LabelPath from the report setting
                    var pdfFilePath = Path.Combine(setting.LabelPath, pdfFileName);

                    File.WriteAllBytes(pdfFilePath, pdfBytes);

              
                    _progressBarControl.EditValue = (int)_progressBarControl.EditValue + 1;
                    _form.Invoke(new Action(() => _form.Refresh()));
                }
                catch (Exception ex)
                {
                    // Display a custom MessageBox if the "labels" property cannot be parsed
                    XtraMessageBox.Show($"Error parsing JSON response: {ex.Message}", "JSON Parsing Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Read the JSON response and display it in a MessageBox
                var jsonResponse = await response.Content.ReadAsStringAsync();
                XtraMessageBox.Show($"Unexpected response: {jsonResponse}", "Unexpected Response", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        _progressBarControl.Invoke(new Action(() => _progressBarControl.Visible = false));
    }

    public async Task BulkLabelSendApiRequestAsyncLimit()
    {
        _progressBarControl.Properties.Maximum = rowHandles.Length;
        _progressBarControl.EditValue = 0;
        _progressBarControl.Invoke(new Action(() => _progressBarControl.Visible = true));

        _form.Invoke(new Action(() => _form.Refresh()));

        foreach (var rowHandle in rowHandles)
        {
            var order_id = _gridView.GetRowCellValue(rowHandle, "order_id");
            var reprint = _gridView.GetRowCellValue(rowHandle, "reprint");
            var weight = _gridView.GetRowCellValue(rowHandle, "weight");
            var height = _gridView.GetRowCellValue(rowHandle, "height");
            var width = _gridView.GetRowCellValue(rowHandle, "width");
            var length = _gridView.GetRowCellValue(rowHandle, "length");
            var order_number = _gridView.GetRowCellValue(rowHandle, "order_number");

            var data = new
            {
                order_id,
                reprint,
                packages = new[]
                {
                new
                {
                    weight,
                    height,
                    width,
                    length
                }
            }
            };

            var apiUrl = "https://api.starshipit.com/api/orders/shipment";
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", _starshipItApiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _ocpApimSubscriptionKey);

                Func<HttpRequestMessage> createRequest = () =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    return request;
                };

                response = await _apiRequestHelper.SendRequestWithExponentialBackoff(client, createRequest);
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    var jsonDocument = JsonDocument.Parse(jsonResponse);
                    var base64EncodedPdfsArray = jsonDocument.RootElement.GetProperty("labels");

                    var base64EncodedPdf = base64EncodedPdfsArray[0].GetString();
                    var pdfBytes = Convert.FromBase64String(base64EncodedPdf);

                    var pdfFileName = $"{order_number}.pdf";

                    var reportManager = new ReportManager(_configuration);

                    var setting = reportManager.GetReportSetting();
                    if (setting == null) throw new Exception("LabelPath setting not found");

                    var pdfFilePath = Path.Combine(setting.LabelPath, pdfFileName);

                    File.WriteAllBytes(pdfFilePath, pdfBytes);

                    XtraMessageBox.Show($"PDF for Order Number {order_number} saved successfully at {pdfFilePath}");

                    _progressBarControl.Invoke(new Action(() =>
                    {
                        _progressBarControl.EditValue = (int)_progressBarControl.EditValue + 1;
                    }));

                    _form.Invoke(new Action(() => _form.Refresh()));
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Error parsing JSON response: {ex.Message}", "JSON Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _progressBarControl.Invoke(new Action(() => _progressBarControl.Visible = false));


        }
    }
    public async Task BulkLabelSendApiRequestAsyncLimitProgress(IProgress<int> progress)
    {
        _form.Invoke(new Action(() => _form.Refresh()));

        for (int i = 0; i < rowHandles.Length; i++)
        {
            try
            {

                var rowHandle = rowHandles[i];

                var order_id = _gridView.GetRowCellValue(rowHandle, "order_id");
                var reprint = _gridView.GetRowCellValue(rowHandle, "reprint");
                var weight = _gridView.GetRowCellValue(rowHandle, "weight");
                var height = _gridView.GetRowCellValue(rowHandle, "height");
                var width = _gridView.GetRowCellValue(rowHandle, "width");
                var length = _gridView.GetRowCellValue(rowHandle, "length");
                var order_number = _gridView.GetRowCellValue(rowHandle, "order_number");

                var data = new
                {
                    order_id,
                    reprint,
                    packages = new[]
                    {
                        new
                        {
                            weight,
                            height,
                            width,
                            length
                        }
                    }
                };

                var apiUrl = "https://api.starshipit.com/api/orders/shipment";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", _starshipItApiKey);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _ocpApimSubscriptionKey);

                    HttpResponseMessage response = await _apiRequestHelper.SendRequestWithExponentialBackoff(client,
                        () =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                                "application/json");
                            return request;
                        });

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        try
                        {
                            var jsonDocument = JsonDocument.Parse(jsonResponse);
                            var base64EncodedPdfsArray = jsonDocument.RootElement.GetProperty("labels");

                            var base64EncodedPdf = base64EncodedPdfsArray[0].GetString();
                            var pdfBytes = Convert.FromBase64String(base64EncodedPdf);

                            var pdfFileName = $"{order_number}.pdf";

                  

                            var reportManager = new ReportManager(_configuration);

                            var setting = reportManager.GetReportSetting();
                            if (setting == null) throw new Exception("LabelPath setting not found");

                            var pdfFilePath = Path.Combine(setting.LabelPath, pdfFileName);

                            File.WriteAllBytes(pdfFilePath, pdfBytes);
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show($"Error parsing JSON response: {ex.Message}", "JSON Parsing Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        XtraMessageBox.Show($"Unexpected response: {jsonResponse}", "Unexpected Response",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Report progress after each request.
        progress.Report((i + 1) * 100 / rowHandles.Length);
        }
        // Calculate the final progress percentage.
        int finalProgress = (rowHandles.Length * 100) / rowHandles.Length;

        // Show a message box to notify the user that all labels have been saved successfully ONLY if the final progress is 100%.
        if (finalProgress == 100)
        {
            XtraMessageBox.Show("Labels for this account saved successfully");
        }
    }


}