using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;

namespace OrderManager.Classes
{
    using System;
    using System.IO;
    using System.Net.Mail;
    using System.Text;
    using System.Windows.Forms;

    public class ExceptionHandler
    {
        private static string logFilePath;

        public static void Initialize(string logFile)
        {
            logFilePath = logFile;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledExceptions;
            Application.ThreadException += HandleThreadExceptions;
        }

        private static void HandleUnhandledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, "UnhandledException");
        }

        private static void HandleThreadExceptions(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "ThreadException");
        }

        private static void HandleException(Exception ex, string exceptionType)
        {
            if (ex == null) return;

            string exceptionMessage = $"Exception Type: {exceptionType}\r\n" +
                                      $"Message: {ex.Message}\r\n" +
                                      $"Source: {ex.Source}\r\n" +
                                      $"StackTrace: {ex.StackTrace}\r\n" +
                                      $"Date/Time: {DateTime.Now}\r\n";

            LogToFile(exceptionMessage);
            SendEmail(exceptionMessage);
            // Show message box
            XtraMessageBox.Show(exceptionMessage, "An error has occurred with OM app. A log file and email has been generated. ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void LogToFile(string exceptionMessage)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(exceptionMessage);
                    sw.WriteLine("--------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Unable to log exception to file: {ex.Message}");
            }
        }

        private static void SendEmail(string exceptionMessage)
        {
            try
            {
                // Set up email client and message
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com");

                mail.From = new MailAddress("app@rubies.com.au");
                mail.To.Add("daniel@rubiesdeerfield.com.au");
                mail.Subject = "Error Report - OrderManager App";
                mail.Body = exceptionMessage;

                // Set up SMTP client
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("app@rubies.com.au", "gnylxwvdjdfmrsvn");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Unable to send email notification: {ex.Message}");
            }
        }
    }

}
