using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using OrderManager.Classes;
using OrderManager;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderManagerEF.Classes;

namespace OrderManagerEF.Forms
{
    public partial class CSCForm : RibbonForm
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


        public CSCForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;



            _reportGenerator = new BulkReportGenerator(configuration);

            // Replace with your actual connection string
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");
            _context = context;


            _apiKeyManager = new ApiKeyManager(connectionString);


            _pickSlipGenerator = new PickSlipGenerator(configuration, context);

            _reportManager = new ReportManager(configuration);
        }
    }
}