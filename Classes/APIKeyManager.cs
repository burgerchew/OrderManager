
using System.Data.SqlClient;

namespace OrderManager.Classes
{


    public class ApiKeyManager
    {
        private string _connectionString;

        public ApiKeyManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public (string StarshipItApiKey, string OcpApimSubscriptionKey) GetApiKeysByLocation(string location)
        {
            string starshipItApiKey = null;
            string ocpApimSubscriptionKey = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT StarShipIT_Api_Key, Ocp_Apim_Subscription_Key FROM StarShipITAPIKeyManager WHERE Location = @Location", connection))
                {
                    command.Parameters.AddWithValue("@Location", location);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            starshipItApiKey = reader["StarShipIT_Api_Key"].ToString();
                            ocpApimSubscriptionKey = reader["Ocp_Apim_Subscription_Key"].ToString();
                        }
                    }
                }
            }

            return (starshipItApiKey, ocpApimSubscriptionKey);
        }
    }

}
