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
using OrderManagerEF.Entities;

namespace OrderManagerEF.Forms
{
    public partial class EntryForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        delegate Form FormCreator(IConfiguration configuration, OMDbContext context, UserSession userSession);

        // Create a dictionary to map form names to their constructors
        private readonly Dictionary<string, FormCreator> formMap;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly UserSession _userSession;
        public EntryForm(IConfiguration configuration, OMDbContext context, UserSession userSession)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            navBarControl1.LinkClicked += NavBarControl1_LinkClicked;
            _userSession = userSession;

            // Initialize the formMap dictionary
            formMap = new Dictionary<string, FormCreator>
            {
                { "navBarItem1", (c, ctx,us) => new CSCForm(c, ctx,us) },
                { "navBarItem2", (c, ctx,u) => new DSForm(c, ctx) },
                { "navBarItem3", (c, ctx,u) => new NZForm(c, ctx) },
                { "navBarItem4", (c, ctx,u) => new SamplesForm(c, ctx) },
                { "navBarItem5", (c, ctx,u) => new PreOrdersForm(c,ctx) },
                { "navBarItem6", (c, ctx, u) => new WebstoreUnder5Form(c,ctx) },
                { "navBarItem7", (c, ctx, u) => new PrintedForm(c,ctx) },
                { "navBarItem9", (c, ctx, u) => new LabelPrintQueueForm(c,ctx) },
                { "navBarItem10", (c, ctx, u) => new PackingForm(c,ctx) },
                { "navBarItem11", (c, ctx, u) => new HoldForm(c,ctx) },
                { "navBarItem12", (c, ctx, u) => new Import1Form(c,ctx) },
                { "navBarItem13", (c, ctx, u) => new CreateShipmentForm(c,ctx) },
                { "navBarItem14", (c, ctx, u) => new CreateLabelForm1(c,ctx) },
                { "navBarItem15", (c, ctx, u) => new ArchiveLabelForm(c,ctx) },
                { "navBarItem15", (c, ctx, u) => new ReplenForm(c,ctx) },
                { "navBarItem17", (c, ctx, u) => new PickandPackForm(c,ctx) },
                { "navBarItem18", (c, ctx, u) => new MajorsForm(c,ctx) },
                { "navBarItem19", (c, ctx, u) => new UserForm(c,ctx) },
            };
        }


        private void NavBarControl1_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            // Lookup the form to open based on the clicked link's name
            if (formMap.TryGetValue(e.Link.Item.Name, out FormCreator formCreator))
            {
                Form formToOpen = formCreator(_configuration, _context, _userSession);

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