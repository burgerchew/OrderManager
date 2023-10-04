using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager.Classes
{
    public class AddressPart
    {
        public DateTime EntryDateTime { get; set; }
        public string AccountingRef { get; set; }
        public string TradingRef { get; set; }
        public string ExtraText { get; set; }
        public string CustomerCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
    }

}
