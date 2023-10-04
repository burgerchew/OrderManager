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
using OrderManagerEF.Data;
using Microsoft.Extensions.Configuration;

namespace OrderManagerEF.Forms
{
    public partial class BatchForm : DevExpress.XtraEditors.XtraForm
    {
        IConfiguration _configuration { get; set; }
        OMDbContext _context { get; set; }
        public BatchForm(IConfiguration configuration,OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
        }
    }
}