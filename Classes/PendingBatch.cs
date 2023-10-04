using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Classes
{
    public class PendingBatch
    {
        public int SalesOrder { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerCode { get; set; }
        public string StarShipITAccount { get; set; }
        public DateTime Date { get; set; }
    }
}
