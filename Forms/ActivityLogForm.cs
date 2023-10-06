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
    public partial class ActivityLogForm : DevExpress.XtraEditors.XtraForm
    {

        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly UserSession _userSession;
        public ActivityLogForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            _userSession = userSession;
            Load += ActivityLogForm_Load;
        }

        private void ActivityLogForm_Load(object sender, EventArgs e)
        {
            LoadUserActivities();
        }


        private void LoadUserActivities()
        {
            if (_userSession?.CurrentUser?.Id == null) return; // Safety check

            var userId = _userSession.CurrentUser.Id;

            // Fetch user activities for the current user
            var userActivities = _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.Timestamp)
                .ToList();


            // Populate the grid control with the fetched data
            gridView1.GridControl.DataSource = userActivities;


        }

    }
}