using DevExpress.XtraEditors;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
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

namespace OrderManagerEF
{
    public partial class PrintedForm : DevExpress.XtraEditors.XtraForm
    {

        private readonly ExcelExporter _excelExporter;
        private readonly BulkReportGenerator _reportGenerator;
        private FileExistenceGridViewHelper _fileExistenceGridViewHelper;
        private bool _dataLoaded;
        private HttpClient client;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly IConfiguration _configuration;
        private readonly ReportManager _reportManager;
        private readonly PickSlipGenerator _pickSlipGenerator;
        private readonly OMDbContext _context;
        private readonly StoredProcedureService _storedProcedureService;
        public PrintedForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
        }
    }
}