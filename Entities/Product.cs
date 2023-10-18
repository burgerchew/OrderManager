using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    [Table("PRODUCTS")]
    public class Product
    {
        [Key]
        [Column("UniqueID")]
        [Required]
        [MaxLength(17)]
        public string UniqueID { get; set; }

        [Column("ProductCode")]
        [Required]
        [MaxLength(25)]
        public string ProductCode { get; set; }

        [Column("ProductTitle")]
        [MaxLength(100)]
        public string ProductTitle { get; set; }
    }
}
