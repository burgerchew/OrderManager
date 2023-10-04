using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    public class StarShipITOrder
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }
        public string Carrier { get; set; }
        public string CarrierName { get; set; }
        public string ShippingMethod { get; set; }
        public bool? SignatureRequired { get; set; }
        public bool? ReturnOrder { get; set; }
        public string DestinationName { get; set; }
        public string DestinationPhone { get; set; }
        public string DestinationStreet { get; set; }
        public string DestinationSuburb { get; set; }
        public string DestinationState { get; set; }
        public string DestinationPostCode { get; set; }
        public string DestinationCountry { get; set; }
        public string DeliveryInstructions { get; set; }
        public string ExtraData { get; set; }
        public bool? Selected { get; set; }
        public long? ShipmentID { get; set; }

        public virtual ICollection<StarShipITOrderDetail> StarShipITOrderDetails { get; set; } = new HashSet<StarShipITOrderDetail>();
    }

    public class StarShipITOrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public long? ItemId { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int? Quantity { get; set; }
        public int? QuantityToShip { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Value { get; set; }

        public virtual StarShipITOrder StarShipITOrder { get; set; }
    }

}
