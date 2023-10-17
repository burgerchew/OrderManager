using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.DTOs
{

        public class ReplenHeader
        {
            public int Id { get; set; }
            public DateTime? ReplenDate { get; set; }
            public string TradingRef { get; set; }
            public int WarehouseId { get; set; }
            
            public virtual ICollection<ReplenDetail> ReplenDetails { get; set; } = new HashSet<ReplenDetail>();
        }

        public class ReplenDetail
    {
            public int Id { get; set; }
            public int ReplenId { get; set; }
            public string ProductCode { get; set; }
            public int Qty { get; set; }

            public string FromLocation { get; set; }

            public string ToLocation { get; set; }

            public virtual ReplenHeader ReplenHeader { get; set; }
    }

    }

