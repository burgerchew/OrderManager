using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class LabelPrintQueue
    {
        public string OrderNo { get; set; }
        public string SalesOrder { get; set; }
        public string UserID { get; set; }
        public DateTime OrderApprovalDateTime { get; set; }
        public bool Printed { get; set; }
    }

}
