using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class PendingBatch
    {
        public string SalesOrder { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string StarShipITAccount { get; set; }
        public DateTime Date { get; set; }
    }
}
