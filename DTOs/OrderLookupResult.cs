using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class OrderLookupResult
    {
        public string CustomerCode { get; set; }
        public string OrderNumber { get; set; }
        public string SKU { get; set; }
        public string APN { get; set; }
        public decimal PickQty { get; set; }
        public DateTime OrderDate { get; set; }
        public string ConNoteNo { get; set; }
        public string Carrier { get; set; }
        public int Ver { get; set; }
        public string AccountingRef { get; set; }
        public string DefaultPickBin { get; set; }
        public string BinNumber { get; set; }
        public int ActualQty { get; set; }

        public string DefaultBinQtyCheck { get; set; }
        public string DefaultBinCheck { get; set; }

        public string PickType { get; set; }
        public int LocationNo { get; set; }
    }

}
