using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    [Table("PBINCONTENTS")]
    public class PBinContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BinContentsID { get; set; }

        [Required]
        public int BinID { get; set; }

        [Required]
        [StringLength(17)]
        public string ProductID { get; set; }

        public int? UnitNumber { get; set; }

        public decimal? ActualQuantity { get; set; }

        public decimal? BaseQuantity { get; set; }
    }
}
