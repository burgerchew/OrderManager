using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class ReplenishmentResult
    {
        public string CustomerCode { get; set; }
        public string OrderType { get; set; }
        public string ProductCode { get; set; }
        public string BulkBin { get; set; }
        public string RetailBin { get; set; }
        public decimal? BulkQty { get; set; }
        public decimal? RetailQty { get; set; }
        public string AccountingRef { get; set; }
        public DateTime? AccountingDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int LocationNo { get; set; }
    }

}
