using DevExpress.XtraEditors;
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
using OrderManagerEF.Data;
using Microsoft.Extensions.Configuration;

namespace OrderManagerEF.Forms
{
    public partial class CreateAccountForm : DevExpress.XtraEditors.XtraForm
    {
        IConfiguration _configuration;
        OMDbContext _context;
        public CreateAccountForm(IConfiguration configuration,OMDbContext context)
        {
            InitializeComponent();
            _configuration= configuration;
            _context = context;

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            // Retrieve input from text edits
            string username = textEdit1.Text;
            string password = textEdit2.Text;
            string confirmPassword = textEdit3.Text;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                XtraMessageBox.Show("All fields are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirmPassword)
            {
                XtraMessageBox.Show("Passwords do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the username is already taken
            if (_context.Users.Any(u => u.Username == username))
            {
                XtraMessageBox.Show("Username is already taken!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Hash the password before saving
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Create new user and save to the database
            var newUser = new User
            {
                Username = username,
                Password = hashedPassword
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            XtraMessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Close this form
            this.Close();
        }
    }
}