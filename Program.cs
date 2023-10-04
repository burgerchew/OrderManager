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

            // Registering OMDbContext
            serviceCollection.AddDbContext<OMDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RubiesConnectionString")));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var dbContext = serviceProvider.GetRequiredService<OMDbContext>();

            // Feedback about where the configuration was loaded from
            XtraMessageBox.Show($"The configuration has been loaded from: {Path.GetFullPath("appsettings.json")}");

            // Inject dbContext into TestForm
            EntryForm mainForm = new EntryForm(Configuration, dbContext);

            // Use the utility method to close the splash screen when the mainForm loads
            mainForm.Load += (s, e) => SplashScreenUtility.CloseSplashScreenIfNeeded();

            // Run the application with TestForm
            Application.Run(mainForm);
        }
    }

    
    }

