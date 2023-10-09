using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class CustomerResult
    {
        public string CustomerCode { get; set; }
        public string CustomerTitle { get; set; }
        public string CustomerGroup { get; set; }
        public int TotalOrders { get; set; }
        public int ExistingOrders { get; set; }
        public int NewOrders { get; set; }
    }

}
