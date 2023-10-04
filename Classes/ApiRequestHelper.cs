using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF.Classes
{
    public class ApiRequestHelper
    {
        private readonly int _maxRetries;
        private readonly int _initialDelay;

        public ApiRequestHelper(int maxRetries = 5, int initialDelay = 200)
        {
            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
        }

        public async Task<HttpResponseMessage> SendRequestWithExponentialBackoff(HttpClient client, Func<HttpRequestMessage> createRequest)
        {
            HttpResponseMessage response;
            int retryCount = 0;
            int delay = _initialDelay;

            while (retryCount < _maxRetries)
            {
                HttpRequestMessage request = createRequest();
                response = await client.SendAsync(request);

                if ((int)response.StatusCode == 429) // Use integer value 429 for TooManyRequests
                {
                    if (++retryCount == _maxRetries)
                    {
                        XtraMessageBox.Show("Max retries reached. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw new HttpRequestException($"Request failed after {_maxRetries} retries due to too many requests.");
                    }

                    await Task.Delay(delay);
                    delay *= 2; // Exponential backoff
                }
                else if (!response.IsSuccessStatusCode)
                {
                    XtraMessageBox.Show($"Error: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

}
