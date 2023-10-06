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
using DevExpress.XtraSplashScreen;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Forms
{


    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private UserActivityLogger _activityLogger;
        private UserSession _userSession; // Service to handle the logged-in user

        public LoginForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            _userSession = userSession; // Set the user session
            textEdit1.Text = "daniel";
            textEdit2.Text = "OM123!";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            // Show the splash screen
            SplashScreenManager.ShowDefaultWaitForm("Please wait", "Logging in...");

            var user = _context.Users.SingleOrDefault(u => u.Username == textEdit1.Text);

            if (user != null && BCrypt.Net.BCrypt.Verify(textEdit2.Text, user.Password))
            {
                XtraMessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Initialize the logger with the user's ID after a successful login
                _activityLogger = new UserActivityLogger(user.Id, _context);
                _activityLogger.Log("User logged in");

                // Set the user in the session
                _userSession.SetCurrentUser(user);

                // Instantiate and show the EntryForm
                using var entryForm = new EntryForm(_configuration, _context, _userSession);
                this.Hide(); // Hide the login form

                // Close the splash screen
                SplashScreenManager.CloseForm(false);

                entryForm.ShowDialog();
                this.Close(); // Close the login form after the main form is closed
            }
            else
            {
                // Close the splash screen in case of an error
                SplashScreenManager.CloseForm(false);

                XtraMessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
        {
            using var createAccountForm = new CreateAccountForm(_configuration, _context);
            createAccountForm.ShowDialog();

            // If the user successfully created an account, you can log the activity or perform other actions
        }
    }

}