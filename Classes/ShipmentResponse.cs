using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager.Classes
{
    public class ShipmentResponse
    {
        public OrderResponse Order { get; set; }
        public bool Success { get; set; }

        public string ExtraData { get; set; }
        public string order_number { get; set; }

    }

    public class OrderResponse
    {
        public int order_id { get; set; }
        public string order_number { get; set; }
        public string Reference { get; set; }
        public string OrderDate { get; set; }
        public List<Package> Packages { get; set; }
        public List<Metadata> Metadatas { get; set; }
    }

    public class Package
    {
        public double Weight { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
    }

    public class Metadata
    {
        public string metafield_key { get; set; }
        public string Value { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }

}
