using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF.Forms
{
    public partial class ApiKeyForm : DevExpress.XtraEditors.XtraForm
    {


        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly UserSession _userSession;
        public ApiKeyForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            _userSession = userSession;
            this.Load += ApiKeyForm_Load;
        }

        private void ApiKeyForm_Load(object sender, EventArgs e)
        {
            LoadDataIntoGridView();
        }

        private void LoadDataIntoGridView()
        {
            try
            {
                // Fetch data from the database
                var apiKeys = _context.Set<StarShipITAPIKeyManager>().ToList();

                // Bind the data to the grid view
                gridControl1.DataSource = apiKeys; // Assuming your GridControl is named 'gridControl1'
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Failed to load data. Error: {ex.Message}", "Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}