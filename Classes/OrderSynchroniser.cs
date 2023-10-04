using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;

namespace OrderManagerEF.Classes
{
    public class OrderSynchroniser
    {
        private readonly HttpClient client;
        private readonly IConfiguration _configuration;
        private readonly int _limit = 250;
        private readonly int _daysBack = -7;
        private readonly string _apiEndpoint = "https://api.starshipit.com/api/orders/unshipped";

        public OrderSynchroniser(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            this._configuration = configuration;
        }

        public async Task SyncAndUpdateOrders(int page = 1)
        {
            string sinceOrderDate =
                Uri.EscapeDataString(DateTime.UtcNow.AddDays(_daysBack).ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'"));
            ApiRequestHelper apiRequestHelper = new ApiRequestHelper();

            while (true)
            {
                try
                {
                    Func<HttpRequestMessage> createRequest = () =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get,
                            $"{_apiEndpoint}?limit={_limit}&page={page}&since_order_date={sinceOrderDate}");
                        return request;
                    };

                    var response = await apiRequestHelper.SendRequestWithExponentialBackoff(client, createRequest);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var orders = JObject.Parse(responseString)["orders"];

                    using (SqlConnection connection =
                           new SqlConnection(_configuration.GetConnectionString("RubiesConnectionString")))
                    {
                        connection.Open();
                        foreach (var order in orders)
                        {
                            if (order["order_id"] != null && order["order_number"] != null)
                            {
                                string orderId = (string)order["order_id"];
                                string orderNumber = (string)order["order_number"];
                                using (SqlCommand cmdTransHeader = new SqlCommand("ASP_ShipmentIDSync", connection))
                                {
                                    cmdTransHeader.CommandType = CommandType.StoredProcedure;
                                    cmdTransHeader.Parameters.AddWithValue("@OrderID", orderId);
                                    cmdTransHeader.Parameters.AddWithValue("@OrderNumber", orderNumber);
                                    cmdTransHeader.ExecuteNonQuery();
                                }
                            }
                        }

                        // Check if orders are still available for the next page
                        if (orders.Count() < _limit)
                            break;

                        page++;
                    }
                }
                catch (Exception ex)
                {
                    // Error handling (existing code)
                    break;
                }
            }
        }
        }

    }

