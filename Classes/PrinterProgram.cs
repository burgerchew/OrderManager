using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;

namespace OrderManagerEF.Classes
{
    public class PrinterProgram
    {
        private readonly string _programPath;
        private readonly string _sourceCommand;
        private readonly string _sortCommand;
        private readonly string _afterCommand;


        public PrinterProgram(string programPath, IConfiguration configuration)
        {
            _programPath = programPath;
            _sourceCommand = configuration["PrinterProgram:SourceCommand"];
            _sortCommand = configuration["PrinterProgram:SortCommand"];
            _afterCommand = configuration["PrinterProgram:AfterCommand"];

            // Add some logging or debugging code to check the values
    
        }

        public void ExecuteDefaultPrinterLog(string defaultPrinterName)
        {
            // Static path for testing
            string logDirectoryPath = @"C:\Logs\printlog";

            // Ensure the log directory exists
            Directory.CreateDirectory(logDirectoryPath);

            // Generate a timestamp for the filename
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string logFileName = Path.Combine(logDirectoryPath, $"PrinterLog_{timestamp}.txt");

            // Create the printer command
            string printerCommand = string.Format("-src \"{0}\" -prn \"{1}\" -options {2} -postproc {3}", _sourceCommand, defaultPrinterName, _sortCommand, _afterCommand);

            // Create a new process
            Process process = new Process();
            process.StartInfo.FileName = _programPath;
            process.StartInfo.Arguments = printerCommand;

            // Set up the process to redirect its output
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            // Start the process
            process.Start();

            // Read the output and write it to the log file
            string output = process.StandardOutput.ReadToEnd();
            File.WriteAllText(logFileName, output);

            // Wait for the process to finish
            process.WaitForExit();
        }







        public void ExecuteDefaultPrinter(string defaultPrinterName)
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo(_programPath);


            startInfo.Arguments = string.Format("-src \"{0}\" -prn \"{1}\" -options {2} -postproc {3} ", _sourceCommand, defaultPrinterName, _sortCommand, _afterCommand);
            // Start the process
            Process.Start(startInfo);

        }

        public void ExecuteDefaultPrinterQuickPrint(string defaultPrinterName, string pickSlipPath, string salesOrderRef)
        {
            // This method executes the default printer program with only the source and printer name parameters.

            // Create the full source path by concatenating the pickSlipPath and salesOrderRef
            string fullSourcePath = string.Format("{0}\\archive\\{1}.pdf", pickSlipPath.TrimEnd('\\'), salesOrderRef);


            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo(_programPath);

            // Include -src and -prn in the arguments
            startInfo.Arguments = string.Format("-src \"{0}\" -prn \"{1}\"", fullSourcePath, defaultPrinterName);

            // Start the process
            Process.Start(startInfo);
        }

    }



}

