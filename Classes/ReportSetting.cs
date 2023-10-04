using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager.Classes
{
    public class ReportSetting
    {
        public int Id { get; set; }
        public string LabelPath { get; set; }
        public string PickSlipPath { get; set; }
        public string ErrorPath { get; set; }

    }
}
