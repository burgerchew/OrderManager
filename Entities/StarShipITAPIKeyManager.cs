using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagerEF.Entities
{
    [Table("StarShipITAPIKeyManager", Schema = "dbo")]
    public class StarShipITAPIKeyManager
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("StarShipIT_Api_Key")]
        [MaxLength(255)]
        public string StarShipITApiKey { get; set; }

        [Column("Ocp_Apim_Subscription_Key")]
        [MaxLength(255)]
        public string OcpApimSubscriptionKey { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }
    }
}
