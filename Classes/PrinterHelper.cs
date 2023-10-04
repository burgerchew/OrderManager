using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using Microsoft.Extensions.Configuration;

namespace OrderManagerEF.Classes
{
    public class PrinterHelper
    {
        public static string GetDefaultPrinter(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("RubiesConnectionString");
            string printerName = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT TOP 1 PrinterName FROM DefaultPrinter", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            printerName = reader.GetString(0);
                        }
                    }
                }
            }

            return printerName;
        }

        public static bool PopulatePrinterComboBox(ComboBoxEdit comboBox)
        {
            bool hasPrinters = false;
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                comboBox.Properties.Items.Add(printerName);
                hasPrinters = true;
            }
            return hasPrinters;
        }

        public static void SaveDefaultPrinter(IConfiguration configuration, string printerName)
        {
            string connectionString = configuration.GetConnectionString("RubiesConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UPDATE DefaultPrinter SET PrinterName = @PrinterName", connection))
                {
                    command.Parameters.AddWithValue("@PrinterName", printerName);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
