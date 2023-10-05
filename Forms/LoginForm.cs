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
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Data;
using OrderManagerEF.Classes;

namespace OrderManagerEF.Forms
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private UserActivityLogger _activityLogger;

        public LoginForm(IConfiguration configuration,OMDbContext context )
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == textEdit1.Text);

            if (user != null && BCrypt.Net.BCrypt.Verify(textEdit2.Text, user.Password))
            {
                XtraMessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Initialize the logger with the user's ID after a successful login
                _activityLogger = new UserActivityLogger(user.Id, _context);
                _activityLogger.Log("User logged in");
            }
            else
            {
                XtraMessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}