using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class ScanPackReportLookup
    {
        public string CustomerCode { get; set; }
        public string OrderNumber { get; set; }
        public string SKU { get; set; }
        public string APN { get; set; }
        public int PickQty { get; set; }
        public int DeliverQty { get; set; }
        public int ShipQty { get; set; }
        public string FulfillmentStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime InvDateAvG { get; set; }
        public DateTime DeliveryDateAvG { get; set; }
        public DateTime ShipDate { get; set; }
        public string ConNoteNo { get; set; }
        public string Carrier { get; set; }
        public string InvRef { get; set; }
        public string DeliveryRef { get; set; }
        public string OrderRef { get; set; }
        public string CompletionFlag { get; set; }
    }

}
