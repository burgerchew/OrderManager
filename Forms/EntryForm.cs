using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraNavBar;
using OrderManagerEF.Data;

namespace OrderManagerEF.Forms
{
    public partial class EntryForm : DevExpress.XtraEditors.XtraForm
    {

        delegate Form FormCreator(IConfiguration configuration, OMDbContext context);

        // Create a dictionary to map form names to their constructors
        private readonly Dictionary<string, FormCreator> formMap;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        public EntryForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            navBarControl1.LinkClicked += NavBarControl1_LinkClicked;
            _configuration = configuration;

            // Initialize the formMap dictionary
            formMap = new Dictionary<string, FormCreator>
            {
                { "navBarItem1", (c, ctx) => new CSCForm(c, ctx) },
                { "navBarItem3", (c, ctx) => new NZForm(c, ctx) },
     
            };
        }


        private void NavBarControl1_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            // Lookup the form to open based on the clicked link's name
            if (formMap.TryGetValue(e.Link.Item.Name, out FormCreator formCreator))
            {
                Form formToOpen = formCreator(_configuration, _context);

                if (formToOpen != null)
                {
                    // Set the main form as the MDI parent
                    formToOpen.MdiParent = this;

                    // Show the form
                    formToOpen.Show();
                }
            }
            else
            {
                // Optionally, show an error message using XtraMessage
                XtraMessageBox.Show("Invalid form name", "Error");
            }
        }

    }
}