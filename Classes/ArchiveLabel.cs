using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager.Classes
{
    public class LabelsArchive
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string OrderDate { get; set; }
        public string Reference { get; set; }
        public string OrderNumber { get; set; }
        public string AddressValidatedKey { get; set; }
        public string AddressValidatedValue { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public bool? Reprint { get; set; }
        public string Location { get; set; }
        public bool? Selected { get; set; }
    }

}
