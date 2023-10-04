using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        public void ExecuteDefaultPrinter(string defaultPrinterName)
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo(_programPath);


            // startInfo.Arguments = string.Format("-src \"C:\\test\\pickslips\\*.*\" -prn \"{0}\" -options sort:date_d -postproc passed:move passed_dir:\"C:\\test\\pickslips\\done\"", defaultPrinterName);
            //startInfo.Arguments = string.Format("-src \"\\\\dbserv\\pickslips\\*.*\" -prn \"{0}\" -options sort:date -postproc passed:move passed_dir:\"\\\\dbserv\\pickslips\\archive\"", defaultPrinterName);

            // Build the arguments string using the configurable values from appsettings

            // Add some logging or debugging code to check the values
            //XtraMessageBox.Show($"SourceCommand: {_sourceCommand}");
            //XtraMessageBox.Show($"SortCommand: {_sortCommand}");
            //XtraMessageBox.Show($"AfterCommand: {_afterCommand}");

            startInfo.Arguments = string.Format("-src \"{0}\" -prn \"{1}\" -options {2} -postproc {3}", _sourceCommand, defaultPrinterName, _sortCommand, _afterCommand);
            // Start the process
            Process.Start(startInfo);

        }
    }
}
