using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class ScanPackReportOverview
    {
        public string CustomerCode { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string? ConNoteNo { get; set; }
        public string? Carrier { get; set; }
        public int Ver { get; set; } // Assuming `CurrentVersionNo` is of type int in your DB
        public string AccountingRef { get; set; }
        public string CompleteFlag { get; set; }
        public int ItemCount { get; set; }
    }

}
