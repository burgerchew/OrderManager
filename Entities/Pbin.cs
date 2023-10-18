using DevExpress.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    [Table("PBINS", Schema = "dbo")]
    public class PBin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BinID { get; set; }

        [Required]
        [MaxLength(25)]
        public string BinNumber { get; set; }

        public int? Location { get; set; }

        [MaxLength(1)]
        public string Type { get; set; }

        public decimal? WeightCapacity { get; set; }

        public bool? Consignment { get; set; }

        public bool? BondedGoods { get; set; }

        public bool? Unavailable { get; set; }

        [MaxLength(17)]
        public string CustomerID { get; set; }

        public int? ParentID { get; set; }
    }
}
