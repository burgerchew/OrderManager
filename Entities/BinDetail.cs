using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    public class BinDetail
    {
        public string BinNumber { get; set; }
        public int BinID { get; set; }

        public decimal? ActualQuantity { get; set; }
    }
}
