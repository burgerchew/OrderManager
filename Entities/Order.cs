using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }
        public string ExtraData { get; set; }
        public string ShippingMethod { get; set; }
        public bool SignatureRequired { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string DeliveryInstructions { get; set; }

       // public virtual ICollection<OrderDetail> OrderDetails { get; set; }


    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public long ItemId { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public int QuantityToShip { get; set; }
        public double Weight { get; set; }
        public decimal Value { get; set; }

        //public virtual Order Order { get; set; }
    }
}

public class Destination
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Street { get; set; }
    public string Suburb { get; set; }
    public string State { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
    public string DeliveryInstructions { get; set; }
}

public class Item
{
    public long ItemId { get; set; }
    public string Description { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public int QuantityToShip { get; set; }
    public double Weight { get; set; }
    public decimal Value { get; set; }
}

public class Package
{
    public long PackageId { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Length { get; set; }
}

public class OrderResponse
{
    public long OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderNumber { get; set; }
    public string Reference { get; set; }
    public string Carrier { get; set; }
    public string CarrierName { get; set; }
    public string CarrierServiceCode { get; set; }
    public string ShippingMethod { get; set; }
    public bool SignatureRequired { get; set; }
    public bool DangerousGoods { get; set; }
    public Destination Destination { get; set; }
    public List<Item> Items { get; set; }
    public List<Package> Packages { get; set; }
    public decimal DeclaredValue { get; set; }
}

public class ApiResponse
{
    public OrderResponse Order { get; set; }
    public bool Success { get; set; }
}

