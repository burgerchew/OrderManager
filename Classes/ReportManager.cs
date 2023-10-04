using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace OrderManagerEF.Classes
{
    public class ReportManager
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ReportManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("RubiesConnectionString");
        }

        public ReportSetting GetReportSetting()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 * FROM ReportSettings";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new ReportSetting
                        {
                            Id = (int)reader["Id"],
                            LabelPath = reader["LabelPath"].ToString(),
                            PickSlipPath = reader["PickslipPath"].ToString(),
                            ErrorPath = reader["ErrorPath"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public bool IsPickSlipPathEmpty()
        {
            ReportSetting reportSetting = GetReportSetting();
            return reportSetting == null || IsDirectoryEmpty(reportSetting.PickSlipPath);
        }

        public void ShowWarningIfPathNotEmpty()
        {
            ReportSetting reportSetting = GetReportSetting();
            if (reportSetting != null)
            {
                int fileCount = Directory.GetFiles(reportSetting.PickSlipPath).Length;

                // Only proceed if there are files in the directory
                if (fileCount > 0)
                {
                    string archivePath = Path.Combine(reportSetting.PickSlipPath, "Archive");

                    string message = $"There are {fileCount} files in the {reportSetting.PickSlipPath} path.\n\n" +
                                     "Would you like to move them to the archive folder and suffix them with '_duplicate'?\n\n" +
                                     "This was probably some error which resulted in files not being archived properly.\n\n" +
                                     "If you choose 'Yes', the files will be moved and renamed. If you choose 'No', no action will be taken.";



                    if (XtraMessageBox.Show(message, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        MoveFilesToArchive(reportSetting.PickSlipPath, archivePath);
                        XtraMessageBox.Show($"Files have been moved to the archive folder and renamed with '_duplicate' suffix. You can find them at {archivePath}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void MoveFilesToArchive(string sourcePath, string archivePath)
        {
            if (!Directory.Exists(archivePath))
            {
                Directory.CreateDirectory(archivePath);
            }

            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);
                string destinationPath = Path.Combine(archivePath, fileName + "_duplicate" + fileExtension);

                // Check if the destination file already exists
                if (File.Exists(destinationPath))
                {
                    // If it does, delete it
                    File.Delete(destinationPath);
                }

                // Move the file, it will now overwrite if the destination file exists
                File.Move(filePath, destinationPath);
            }
        }




        private bool IsDirectoryEmpty(string path)
        {
            return Directory.Exists(path) && !Directory.EnumerateFileSystemEntries(path).Any();
        }


        public int SaveReportSetting(ReportSetting reportSetting)
        {
            int existingId = GetExistingReportSettingId();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                if (existingId > 0)
                {
                    // Update the existing row instead of inserting a new one
                    string updateQuery = "UPDATE ReportSettings SET LabelPath = @LabelPath, PickslipPath = @PickslipPath, ErrorPath = @ErrorPath  WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", existingId);
                        command.Parameters.AddWithValue("@LabelPath", reportSetting.LabelPath);
                        command.Parameters.AddWithValue("@PickslipPath", reportSetting.PickSlipPath);
                        command.Parameters.AddWithValue("@ErrorPath", reportSetting.ErrorPath);
                        command.ExecuteNonQuery();
                    }
                    return existingId;
                }
                else
                {
                    // Insert a new row
                    string insertQuery = "INSERT INTO ReportSettings (LabelPath, PickslipPath, ErrorPath) OUTPUT INSERTED.Id VALUES (@LabelPath, @PickslipPath, @ErrorPath)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@LabelPath", reportSetting.LabelPath);
                        command.Parameters.AddWithValue("@PickslipPath", reportSetting.PickSlipPath);
                        command.Parameters.AddWithValue("@ErrorPath", reportSetting.ErrorPath);
                        return (int)command.ExecuteScalar();
                    }
                }
            }
        }

        private int GetExistingReportSettingId()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 Id FROM ReportSettings";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    return result == null ? 0 : (int)result;
                }
            }
        }

        public void UpdateReportSetting(ReportSetting reportSetting)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE ReportSettings SET LabelPath = @LabelPath, PickSlipPath = @PickSlipPath, ErrorPath = @ErrorPath WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", reportSetting.Id);
                    command.Parameters.AddWithValue("@LabelPath", reportSetting.LabelPath);
                    command.Parameters.AddWithValue("@PickSlipPath", reportSetting.PickSlipPath);
                    command.Parameters.AddWithValue("@ErrorPath", reportSetting.ErrorPath);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
