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
    public partial class LabelPrintQueueForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public LabelPrintQueueForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            this.VisibleChanged += new EventHandler(this.LabelPrintQueue_VisibleChanged);
            _configuration = configuration;
            _context = context;

        }

        private void LabelPrintQueue_VisibleChanged(Object sender, EventArgs e)
        {
            if (this.Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }


        private void LoadData()
        {
            // Fetch the data from the view using EF Core
            var labelPrintQueueData = _context.LabelPrintQueues.ToList();

            // Assuming you have a grid or other control to display the data
            // Set the fetched data as the DataSource
            gridControl1.DataSource = labelPrintQueueData;  // Replace 'yourGridControl' with the actual name of your control
        }



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var jobRunner = new SqlAgentJobRunner("DBSERV\\ABM", "msdb", "RestartActionQ");
                jobRunner.RunJob();
                XtraMessageBox.Show("Job started successfully!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error starting job: {ex.Message}");
            }
        }
    }
}