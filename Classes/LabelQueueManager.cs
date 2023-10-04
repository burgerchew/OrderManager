using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManager.Classes
{
    public class LabelQueueManager
    {
        private SqlConnection _connection;
        private string _tableName;
        private IConfiguration _configuration;
        public LabelQueueManager(string tableName, IConfiguration configuration)
        {
            _tableName = tableName;
            _configuration = configuration;
            ConnectToDatabase();
        }

        private void ConnectToDatabase()
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public void TruncateTable()
        {
            var truncateQuery = $"TRUNCATE TABLE {_tableName}";
            var truncateCommand = new SqlCommand(truncateQuery, _connection);
            truncateCommand.ExecuteNonQuery();
        }

        public void InsertData(FileExistenceGridView gridView, Dictionary<string, string> columnMappings, string[] parameterNames)
        {
            var selectedRowHandles = gridView.GetSelectedRows();

            foreach (var handle in selectedRowHandles)
            {
                var values = new string[columnMappings.Count];

                for (int i = 0; i < columnMappings.Count; i++)
                {
                    string gridColumnName = columnMappings.ElementAt(i).Key;
                    string tableColumnName = columnMappings.ElementAt(i).Value;

                    var cellValue = gridView.GetRowCellValue(handle, gridColumnName);

                    if (cellValue is DateTime dateTimeValue)
                    {
                        values[i] = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        values[i] = cellValue != null ? cellValue.ToString() : string.Empty;
                    }
                }

                var tableColumnNames = string.Join(",", columnMappings.Values);
                var insertQuery = $"INSERT INTO {_tableName} ({tableColumnNames}) VALUES ({string.Join(",", parameterNames)})";

                var command = new SqlCommand(insertQuery, _connection);

                for (int i = 0; i < parameterNames.Length; i++)
                {
                    command.Parameters.AddWithValue(parameterNames[i], values[i]);
                }

                command.ExecuteNonQuery();
            }
        }

        public void ShowRowCountMessage(int rowCount)
        {
            var message = string.Format("Inserted {0} rows.", rowCount);
            XtraMessageBox.Show(message, "Insert Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool ConfirmTruncate()
        {
            var result = XtraMessageBox.Show(
                $"The existing batch for {_tableName} will be removed from staging. Are you sure you want to continue?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return result == DialogResult.Yes;
        }
    }
}
