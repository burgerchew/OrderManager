using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager.Classes
{
    public class BINContentsLocn1
    {
        public string ProductCode { get; set; }
        public string ProductTitle { get; set; }
        public int ActualQuantity { get; set; }
        public int BaseQuantity { get; set; }
        public string BinNumber { get; set; }
        public string PickType { get; set; }
        public decimal FreeStock { get; set; }
        public string Barcode { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal CustomerOrders { get; set; }
        public decimal SupplierOrders { get; set; }
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
        public decimal FreeStock { get; set; }
        public string Barcode { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal CustomerOrders { get; set; }
        public decimal SupplierOrders { get; set; }
        public int TotalAllocatedInBins { get; set; }
    }

}
