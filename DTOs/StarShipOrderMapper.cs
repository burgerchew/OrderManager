using OrderManagerEF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{
    public class StarShipOrderMapper
    {

        public Order MapToOrder(StarShipITOrder starShipOrder)
        {
            return new Order
            {
                Id = starShipOrder.Id,
                OrderDate = (DateTime)starShipOrder.OrderDate,
                OrderNumber = starShipOrder.OrderNumber,
                Reference = starShipOrder.Reference,
                ShippingMethod = starShipOrder.ShippingMethod,
                SignatureRequired = (bool)starShipOrder.SignatureRequired,
                Name = starShipOrder.DestinationName,
                Phone = starShipOrder.DestinationPhone,
                Street = starShipOrder.DestinationStreet,
                Suburb = starShipOrder.DestinationSuburb,
                State = starShipOrder.DestinationState,
                PostCode = starShipOrder.DestinationPostCode,
                Country = starShipOrder.DestinationCountry,
                DeliveryInstructions = starShipOrder.DeliveryInstructions
            };
        }

        public OrderDetail MapToOrderDetail(StarShipITOrderDetail starShipOrderDetail)
        {
            return new OrderDetail
            {
                Id = starShipOrderDetail.Id,
                OrderId = starShipOrderDetail.OrderId,
                ItemId = (long)starShipOrderDetail.ItemId,
                Description = starShipOrderDetail.Description,
                SKU = starShipOrderDetail.SKU,
                Quantity = (int)starShipOrderDetail.Quantity,
                QuantityToShip = (int)starShipOrderDetail.QuantityToShip,
                Weight = (double)starShipOrderDetail.Weight,
                Value = (decimal)starShipOrderDetail.Value
            };
        }

        public List<Order> MapToOrders(List<StarShipITOrder> starShipITOrders)
        {
            return starShipITOrders.Select(MapToOrder).ToList();
        }

        public List<OrderDetail> MapToOrderDetails(List<StarShipITOrderDetail> starShipITOrderDetails)
        {
            return starShipITOrderDetails.Select(MapToOrderDetail).ToList();
        }

    }
}
