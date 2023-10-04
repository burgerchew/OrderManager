using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager.Classes
{
    public class DatabaseManager
    {
        public enum ConnectionType
        {
            Production,
            Test
        }

        //public static string GetConnectionString(ConnectionType connectionType)
        //{
        //    switch (connectionType)
        //    {
        //        case ConnectionType.Production:
        //            return ConfigurationManager
        //                .ConnectionStrings["OrderManager.Properties.Settings.RubiesConnectionString"]
        //                .ConnectionString;
        //        case ConnectionType.Test:
        //            return ConfigurationManager
        //                .ConnectionStrings["OrderManager.Properties.Settings.RubiesTestConnectionString"]
        //                .ConnectionString;
        //        default:
        //            throw new Exception("Invalid connection type");
        //    }
        //}

        public static string GetConnectionString(ConnectionType connectionType)
        {
            string configFileName = "App.config";
            string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\OM\";
            string configFile = Path.Combine(targetPath, configFileName);

            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            string connectionString;

            switch (connectionType)
            {
                case ConnectionType.Production:
                    connectionString = config.ConnectionStrings.ConnectionStrings["OrderManager.Properties.Settings.RubiesConnectionString"].ConnectionString;
                    break;
                case ConnectionType.Test:
                    connectionString = config.ConnectionStrings.ConnectionStrings["OrderManager.Properties.Settings.RubiesTestConnectionString"].ConnectionString;
                    break;
                default:
                    throw new Exception("Invalid connection type");
            }

            return connectionString;
        }



    }


}
