using DevExpress.XtraEditors;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Classes
{
    public class PrinterHelperEF
    {
        public static string GetUserPrinter(OMDbContext context, int userId)
        {
            var user = context.Users.SingleOrDefault(u => u.Id == userId);
            return user?.PrinterName;
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

        public static void SaveUserPrinter(OMDbContext context, int userId, string printerName)
        {
            var user = context.Users.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.PrinterName = printerName;
                context.SaveChanges();
            }
        }
    }

}
