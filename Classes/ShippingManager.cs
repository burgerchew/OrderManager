using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using OrderManagerEF.Entities;

namespace OrderManager.Classes;

public class ShipmentManager
{
    private readonly string _starshipItApiKey;
    private readonly string _ocpApimSubscriptionKey;

    public ShipmentManager(string starshipItApiKey, string ocpApimSubscriptionKey)
    {
        _starshipItApiKey = starshipItApiKey;
        _ocpApimSubscriptionKey = ocpApimSubscriptionKey;
    }

    public async Task<List<ShipmentResponse>> CreateShipments(List<Order> orders, List<List<OrderDetail>> ordersDetails,
        IProgress<int> progress, int totalRows)
    {
        var shipmentResponses = new List<ShipmentResponse>();

        //Progress Bar Update
        var ordersProcessed = 0;

        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var orderDetails = ordersDetails[i];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.starshipit.com");
                client.DefaultRequestHeaders.Add("StarShipIT-Api-Key", _starshipItApiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _ocpApimSubscriptionKey);

                var payload = new
                {
                    order = new
                    {
                        id = order.Id,
                        order_date = order.OrderDate,
                        order_number = order.OrderNumber,
                        reference = order.Reference,
                        shipping_method = order.ShippingMethod,
                        signature_required = order.SignatureRequired,
                        destination = new
                        {
                            name = order.Name,
                            phone = order.Phone,
                            street = order.Street,
                            suburb = order.Suburb,
                            state = order.State,
                            post_code = order.PostCode,
                            country = order.Country,
                            delivery_instructions = order.DeliveryInstructions
                        },

                        items = orderDetails.Select(od => new
                        {
                            item_id = od.ItemId,
                            description = od.Description,
                            sku = od.SKU,
                            quantity = od.Quantity,
                            quantity_to_ship = od.QuantityToShip,
                            weight = od.Weight,
                            value = od.Value
                        }).ToList()
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);

                var createRequest = () =>
                {
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    return new HttpRequestMessage(HttpMethod.Post, "/api/orders") { Content = content };
                };

                // Call SendRequestWithExponentialBackoff method here
                var response = await SendRequestWithExponentialBackoff(client, createRequest);
                // ...

                try
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var shipmentResponse = JsonConvert.DeserializeObject<ShipmentResponse>(jsonResponse);
                    shipmentResponses.Add(shipmentResponse);
                    shipmentResponse.ExtraData = order.ExtraData;
                    //XtraMessageBox.Show($"API request sent successfully.", "Order Created", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Update the progress after processing the order
                    ordersProcessed++;
                    var currentProgress = (int)((double)ordersProcessed / totalRows * 100);
                    progress.Report(currentProgress);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"An error occurred while decoding the response: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        return shipmentResponses;
    }



    public async Task<HttpResponseMessage> SendRequestWithExponentialBackoff(HttpClient client,
        Func<HttpRequestMessage> createRequest)
    {
        HttpResponseMessage response;
        var maxRetries = 5;
        var retryCount = 0;
        var delay = 200; // Initial delay in milliseconds (200ms will ensure a max of 5 requests per second)

        while (retryCount < maxRetries)
        {
            var request = createRequest();
            response = await client.SendAsync(request);

            if ((int)response.StatusCode == 429) // Use integer value 429 for TooManyRequests
            {
                if (++retryCount == maxRetries)
                {
                    XtraMessageBox.Show("Max retries reached. Please try again later.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    throw new HttpRequestException(
                        $"Request failed after {maxRetries} retries due to too many requests.");
                }

                await Task.Delay(delay);
                delay *= 2; // Exponential backoff
            }
            else if (!response.IsSuccessStatusCode)
            {
                XtraMessageBox.Show($"Error: {response.StatusCode}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
            }
            else
            {
                return response;
            }
        }

        throw new InvalidOperationException("Request failed after retries.");
    }
}