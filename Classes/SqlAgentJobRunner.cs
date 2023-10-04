using System.Data.SqlClient;

namespace OrderManagerEF.Classes
{
    public class SqlAgentJobRunner
    {
        private readonly string _serverName;
        private readonly string _databaseName;
        private readonly string _jobName;

        public SqlAgentJobRunner(string serverName, string databaseName, string jobName)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _jobName = jobName;
        }

        public void RunJob()
        {
            // Connect to the SQL Server instance and open a connection to the msdb database
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = _serverName;
            builder.InitialCatalog = _databaseName;
            builder.IntegratedSecurity = true;
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            // Start the SQL Server Agent Job using sp_start_job
            SqlCommand cmd = new SqlCommand("sp_start_job", connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@job_name", _jobName);
            cmd.ExecuteNonQuery();

            // Close the connection
            connection.Close();
        }
    }
}