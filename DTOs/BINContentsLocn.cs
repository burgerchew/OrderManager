using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class BINContentsLocn1
    {
        public string ProductCode { get; set; }
        public string ProductTitle { get; set; }
        public int ActualQuantity { get; set; }
        public int BaseQuantity { get; set; }
        public string BinNumber { get; set; }
        public string PickType { get; set; }
        public int FreeStock { get; set; }
        public string Barcode { get; set; }
        public int QuantityOnHand { get; set; }
        public int CustomerOrders { get; set; }
        public int SupplierOrders { get; set; }
        public int TotalAllocatedInBins { get; set; }
    }

    public class BINContentsLocn11
    {
        public string ProductCode { get; set; }
        public string ProductTitle { get; set; }
        public int ActualQuantity { get; set; }
        public int BaseQuantity { get; set; }
        public string BinNumber { get; set; }
        public string PickType { get; set; }
        public int FreeStock { get; set; }
        public string Barcode { get; set; }
        public int QuantityOnHand { get; set; }
        public int CustomerOrders { get; set; }
        public int SupplierOrders { get; set; }
        public int TotalAllocatedInBins { get; set; }
    }

}
