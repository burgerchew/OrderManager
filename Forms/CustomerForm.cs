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

namespace OrderManagerEF
{
    public partial class CustomerForm : DevExpress.XtraEditors.XtraForm
    {
        private IConfiguration _configuration;
        private OMDbContext _context;

        public CustomerForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;


            // Fetch the data using the StoredProcedureService
            var customerList = _context.CustomerResults.ToList();

            // Now you need to bind the fetched data to the appropriate control.
            // Assuming you're binding to a GridControl for demonstration:
            gridControl1.DataSource = customerList; // Replace 'yourGridControl' with the actual control name if it's different
        }
    }
}