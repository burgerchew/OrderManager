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
using OrderManagerEF.Data;
using OrderManagerEF.Classes;
using System.Net.Http;

namespace OrderManagerEF.Forms
{
    public partial class DSForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private readonly ExcelExporter _excelExporter;
        private readonly BulkReportGenerator _reportGenerator;
        private FileExistenceGridViewHelper _fileExistenceGridViewHelper;
        private bool _dataLoaded;
        private HttpClient client;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly string _location = "CSC"; // Define your location
        private readonly IConfiguration _configuration;
        private readonly ReportManager _reportManager;
        private readonly PickSlipGenerator _pickSlipGenerator;
        private readonly OMDbContext _context;
        private readonly StoredProcedureService _storedProcedureService;
        public DSForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
        }
    }
}