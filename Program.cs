using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Data;
using OrderManager.Classes;
using OrderManagerEF.Classes;
using OrderManagerEF.Forms;
using OrderManagerEF.Entities;
using DevExpress.XtraReports.Diagnostics;

namespace OrderManagerEF
{
    internal static class Program
    {

        public static string targetDirectory { get; private set; } // Static property to hold the target directory
        public static IConfigurationRoot Configuration { get; set; }

        [STAThread]
        static void Main()
        {
            // Set up application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // Define the target directory
            targetDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OM");


            // Create the target directory if it doesn't exist
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Define the source files
            string[] sourceFiles = { "appsettings.json" };

            foreach (string sourceFile in sourceFiles)
            {
                // Ensure the source file exists
                if (File.Exists(sourceFile))
                {
                    string targetFile = Path.Combine(targetDirectory, sourceFile);
                    // Copy the source file to the target directory
                    File.Copy(sourceFile, targetFile, true); // the 'true' parameter allows the file to be overwritten if it already exists
                }
            }



            //// Set up configuration sources directly from appsettings.json
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //    .AddJsonFile("appsettings.json");

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(targetDirectory) // Note that we are now loading configuration from the target directory
                .AddJsonFile("appsettings.json");


            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();

            // Registering OMDbContext and UserSession
            serviceCollection.AddDbContext<OMDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RubiesConnectionString")));
            serviceCollection.AddSingleton<UserSession>();  // <-- Register the UserSession service

            var path = Path.Combine(targetDirectory, "appsettings.json");
            XtraMessageBox.Show($"The configuration has been loaded from: {Path.GetFullPath(path)}");

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var dbContext = serviceProvider.GetRequiredService<OMDbContext>();
            var userSession = serviceProvider.GetRequiredService<UserSession>();  // <-- Retrieve the UserSession instance


            //Load Report Settings and Error Log Path
            ReportManager reportManager = new ReportManager(Configuration);

            ReportSetting setting = reportManager.GetReportSetting();

            string errorPath = setting?.ErrorPath ?? Configuration["ErrorLogPath"];
            string errorLogDirectoryName = Path.GetDirectoryName(errorPath);

            if (!Directory.Exists(errorLogDirectoryName))
            {
                Directory.CreateDirectory(errorLogDirectoryName);
            }

            ExceptionHandler.Initialize(errorPath);


            // Create an instance of LoginForm and show it
            LoginForm loginForm = new LoginForm(Configuration, dbContext, userSession);  // <-- Inject UserSession

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Feedback about where the configuration was loaded from
                XtraMessageBox.Show($"The configuration has been loaded from: {Path.GetFullPath("appsettings.json")}");

                // Inject dbContext and UserSession into EntryForm
                EntryForm mainForm = new EntryForm(Configuration, dbContext, userSession);  // <-- Inject UserSession

                // Use the utility method to close the splash screen when the mainForm loads
                mainForm.Load += (s, e) => SplashScreenUtility.CloseSplashScreenIfNeeded();

                // Run the application with EntryForm
                Application.Run(mainForm);
            }


            ////Bypass Login for testing
            //EntryForm mainForm = new EntryForm(Configuration, dbContext, userSession);
            //Application.Run(mainForm);
        }


    }


}

