using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderManagerEF.Data;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Entities;
using DevExpress.XtraCharts;
using OrderManagerEF.Classes;
using Microsoft.EntityFrameworkCore;

namespace OrderManagerEF.Forms
{
    public partial class OrderChartForm : DevExpress.XtraEditors.XtraForm
    {
        IConfiguration _configuration;
        OMDbContext _context;
        UserSession _userSession;

        public OrderChartForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            _userSession = userSession;
            Load += OrderChartForm_Load;
        }

        private void OrderChartForm_Load(object sender, EventArgs e)
        {
            var data1 = _context.NzOrderDatas.ToList();
            BarChartHelper.SetupBarChart(chartControl1, data1, "AccountingRef", "CustomerCode",
                "Number of Orders for NZ", "Chameleon", true, true, data1.Count);

            var data2 = _context.CscOrderDatas.ToList();
            BarChartHelper.SetupBarChart(chartControl2, data2, "AccountingRef", "CustomerCode",
                "Number of Orders for CSC", "Chameleon", true, true, data2.Count);

            var data3 = _context.DSOrderDatas.ToList();
            BarChartHelper.SetupBarChart(chartControl3, data3, "AccountingRef", "CustomerCode",
                "Number of Orders for DS", "Chameleon", true, true, data3.Count);

            var data4 = _context.RubiesOrderDatas.ToList();
            BarChartHelper.SetupBarChart(chartControl4, data4, "AccountingRef", "CustomerCode",
                "Number of Orders for Rubies Under 5KG", "Chameleon", true, true, data4.Count);


        }
    }
}