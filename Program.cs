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

namespace OrderManagerEF
{
    internal static class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        [STAThread]
        static void Main()
        {
            // Set up application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set up configuration sources directly from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();

            // Registering OMDbContext and UserSession
            serviceCollection.AddDbContext<OMDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RubiesConnectionString")));
            serviceCollection.AddSingleton<UserSession>();  // <-- Register the UserSession service

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var dbContext = serviceProvider.GetRequiredService<OMDbContext>();
            var userSession = serviceProvider.GetRequiredService<UserSession>();  // <-- Retrieve the UserSession instance

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
        }


    }


}

