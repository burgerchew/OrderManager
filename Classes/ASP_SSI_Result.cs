using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Classes
{

    public class ASP_SSI_Result
    {
        public string CustomerCode { get; set; }
        public string CustomerTitle { get; set; }
        public DateTime EntryDateTime { get; set; }
        public DateTime DueDate { get; set; }
        public string AccountingRef { get; set; }
        public string TradingRef { get; set; }
        public string ZEmployeeGroup { get; set; }
        public decimal TotalWeight { get; set; }
        public string ZShippingMethod { get; set; }
        public string ZShipmentID { get; set; }
        public string LabelFile { get; set; }
        public string PickSlipFile { get; set; }

        public string ArchiveFile { get; set; }
        public int OrdersCount { get; set; }

        public string FileStatus { get; set; }
    }

}