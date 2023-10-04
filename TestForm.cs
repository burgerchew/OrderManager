using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Data;

namespace OrderManagerEF
{
    public partial class TestForm : DevExpress.XtraEditors.XtraForm
    {
        IConfiguration _configuration { get; set; }
        OMDbContext _context { get; set; }
        public TestForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            LoadData();
        }
        private void LoadData()
        {
            var data = _context.DSOrderDatas.ToList();

            gridControl1.DataSource = data;

            // Populate the grid control with the fetched data
            gridView1.GridControl.DataSource = data;
        }

    }
}
