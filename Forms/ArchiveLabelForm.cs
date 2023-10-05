using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
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
    public partial class ArchiveLabelForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        // Set up a flag to keep track of whether data has been loaded yet.
        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public ArchiveLabelForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            this.VisibleChanged += ArchiveLabel_VisibleChanged;
            _configuration = configuration;
            _context = context;
        }

        private void ArchiveLabel_VisibleChanged(object sender, EventArgs e)
        {
            // Load data when the form is visible, but only if it hasn't been loaded already.
            if (this.Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }

        private void LoadData()
        {

            var data = _context.LabelArchives.ToList();
            // Bind the data to the GridControl
            gridControl1.DataSource = data;
        }
    }
}