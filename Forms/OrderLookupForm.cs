using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Reporting;

namespace OrderManagerEF.Forms
{
    public partial class OrderLookupForm : XtraForm
    {

        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public OrderLookupForm(IConfiguration configuration, OMDbContext context, string OrderRef)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            this.WindowState = FormWindowState.Maximized;


            var data = _context.ExecuteOrderLookupResult(OrderRef);
            gridControl1.DataSource = data;
        }
    }
}