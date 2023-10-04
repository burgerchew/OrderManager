using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Classes
{
    public class StarShipITAccount
    {
        public string AccountName { get; set; }

        public StarShipITAccount(string accountName)
        {
            AccountName = accountName;
        }
    }
}
